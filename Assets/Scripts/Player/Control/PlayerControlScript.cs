﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Animations.Rigging;

// Require necessary components
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterInputController))]
[RequireComponent(typeof(PlayerStatus))]

public class PlayerControlScript : MonoBehaviour
{
    // Singleton instance of player
    public static PlayerControlScript PlayerInstance { get; private set;}

    #region Player Components & State Management
    public Animator anim;
    public Rigidbody rbody;
    public Camera mainCamera;
    public Transform orientation;
    public Transform spawn;
    public Transform aimTarget;
    public LayerMask aimColliderLayerMask;
    public Rig headRig;

    /* State Machines:
        PlayerControlScript handles 3 states machines that each store a separate state.
        - The ActionStateType state machine handle player actions, such as attacking, pick up, etc.
        - The MotionStateType state machine handle player movement, such as running, jumping, etc.
        - GlobalState is defined using an enum and determines what global state the player is in,
        such as Normal, Staggered, etc., forcefully interrupting the action or movement states. 
     */
    public StateMachine<ActionStateType, BaseState> ActionStateMachine { get; private set; }
    public StateMachine<MotionStateType, BaseState> MotionStateMachine { get; private set; }
    public GlobalStateManager GlobalStateManager { get; private set; }
    #endregion

    #region Controller Input Reading & Caching
    // Cached input readings to be used by player state and component classes
    public CharacterInputController cinput;
    public float _inputForward = 0f;
    public float _inputRight = 0f;
    public bool _inputJump = false;
    public bool _inputAimDown = false;
    public bool _inputAttack = false;
    public bool _interact = false;
    public bool _drop = false;
    public bool _reload = false;
    public Vector3 _inputDir;
    public bool forceStrafe; // Can be set by outside components to force player to strafe
    #endregion

    #region Movement & Animation Properties
    public float speedMultiplier = 1f; // Sets the animationSpeed, rootMovementSpeed, and rootTurnSpeed
    public float animationSpeed = 1f;
    public float rootMovementSpeed = 1f;
    public float rootTurnSpeed = 1f;
    public float groundDrag = 0f;
    public float airDrag = 0.2f;
    public bool isMoving = false;
    public float turnStrafeSpeed = 10f;
    public float upgradeMult = .1f;
    #endregion

    PlayerStatus playerStatus;

    #region Environmental/Sensor Properties
    public Vector3 WorldVelocity { get; private set; }
    public Vector3 localVelocity;
    private Vector3 prevWorldPosition;
    private int groundContactCount = 0;
    public bool isGrounded = true;

    public bool GetGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }
    #endregion

    void Awake()
    {
        if (PlayerInstance != null && PlayerInstance != this)
        {
            // If another instance exists, destroy this one to enforce the singleton pattern
            Destroy(gameObject);
        }
        else
        {
            // Set the instance to this object
            PlayerInstance = this;

            // Make this object persistent across scenes
            DontDestroyOnLoad(gameObject);
        }
        
        // Get player Status componenent
        playerStatus = GetComponent<PlayerStatus>();

        // Get Required Player Components and Cache
        anim = GetComponent<Animator>();
        if (anim == null)
            Debug.LogError("Animator could not be found");

        rbody = GetComponent<Rigidbody>();
        if (rbody == null)
            Debug.LogError("Rigidbody could not be found");

        cinput = GetComponent<CharacterInputController>();
        if (cinput == null)
            Debug.LogError("CharacterInputController could not be found");

        // Get/Check Player Objects
        if (mainCamera == null)
        {
            Debug.LogError("Player Script requires a camera object");
        }

        if(orientation == null){
            Debug.LogError("Player Script requires an orientation transform");
        }


        // Initialize State Machines
        ActionStateMachine = new StateMachine<ActionStateType, BaseState>();
        MotionStateMachine = new StateMachine<MotionStateType, BaseState>();
        GlobalStateManager = new GlobalStateManager(this);

        // Register Action States
        ActionStateMachine.RegisterState(ActionStateType.Idle, new IdleActionState(this));
        // ActionStateMachine.RegisterState(ActionStateType.Attack, new AttackActionState(this));
        // ActionStateMachine.RegisterState(ActionStateType.Pickup, new PickupActionState(this));
        // ActionStateMachine.RegisterState(ActionStateType.Interact, new InteractActionState(this));

        // Register Motion States
        MotionStateMachine.RegisterState(MotionStateType.Idle, new IdleMotionState(this));
        MotionStateMachine.RegisterState(MotionStateType.Run, new RunMotionState(this));
        // MotionStateMachine.RegisterState(MotionStateType.Dodge, new DodgeMotionState(this));
        MotionStateMachine.RegisterState(MotionStateType.JumpAir, new JumpAirMotionState(this));

        // Initialize with Idle States
        ActionStateMachine.ChangeState(ActionStateType.Idle);
        MotionStateMachine.ChangeState(MotionStateType.Idle);
    }

    void Start()
    {
        // Example of how to get access to certain limbs
        // leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        // rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    // Make sure to reset necessary player attributes to prevent bugs
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject[] respawnObject = GameObject.FindGameObjectsWithTag("Respawn");

        if (respawnObject.Length != 0)
        {
            spawn = GameObject.FindGameObjectsWithTag("Respawn")[0].transform;
            this.gameObject.transform.position = spawn.position;
            groundContactCount = 0;
        }
    }
    // Responsible for reading (and caching) input, updating certain global environment properties, and interrupting global states
    private void Update()
    {
        if (cinput.enabled)
        {
            _inputForward = cinput.Forward;
            _inputRight = cinput.Right;
            _inputAimDown = cinput.AimDown;
            _inputAttack = cinput.Attack;
            _inputJump = cinput.Jump;
            _interact = cinput.Interact;
            _drop = cinput.Drop;
            _reload = cinput.Reload;
            // _inputJump is handled in FixedUpdate to sync with physics
        }

        isMoving = Mathf.Abs(_inputForward) > 0.05f || Mathf.Abs(_inputRight) > 0.05f;

        // Handles player orientation with respect to the camera and the player aiming down
        HandleOrientation();

        // Update the aim target world position based on screen raycast
        UpdateTargets();

        // Modifies the current GlobalState based on outside parameters
        HandleGlobalState();

        if (GlobalStateManager.GetCurrentState() == GlobalState.Normal)
        {
            ActionStateMachine.Update();
            MotionStateMachine.Update();
        }
    }

    void FixedUpdate()
    {
        // Handle drag based on grounded state
        rbody.drag = isGrounded ? groundDrag : airDrag;

        // Physics calculations for Rigidbody because animator velocity is inaccurate
        WorldVelocity = (transform.position - prevWorldPosition) / Time.fixedDeltaTime;
        prevWorldPosition = transform.position;

        // Project the world velocity onto the character's local axes
        localVelocity.z = Vector3.Dot(WorldVelocity, transform.forward);
        localVelocity.y = Vector3.Dot(WorldVelocity, transform.up);
        localVelocity.x = Vector3.Dot(WorldVelocity, transform.right);

        // Update all animation/root speed based on speed multiplier
        animationSpeed = speedMultiplier + (playerStatus.speedUpgrade * upgradeMult);
        rootMovementSpeed = speedMultiplier + (playerStatus.speedUpgrade * upgradeMult);
        rootTurnSpeed = speedMultiplier + (playerStatus.speedUpgrade * upgradeMult);
        anim.speed = animationSpeed;

        // Ground Check
        PerformGroundCheck();

        // Update State Machines based on Global State
        if (GlobalStateManager.GetCurrentState() == GlobalState.Normal)
        {
            ActionStateMachine.FixedUpdate();
            MotionStateMachine.FixedUpdate();
        }
        else
        {
            // Handle Global State-specific updates if necessary
        }

        // Update Global Animator Parameters
        anim.SetBool("isGrounded", isGrounded);
        anim.SetBool("isMoving", isMoving);
        anim.SetBool("aimDown", _inputAimDown);

        // Smoothly transition the aimDownLerp parameter (Determines blending between strafe and turning animations)
        float targetAimDown = 0f;
        if (_inputAimDown || forceStrafe){
            targetAimDown = 1f;
        }
        float currentTurnStrafe = anim.GetFloat("TurnStrafeLerp");
        float newTurnStrafe = Mathf.Lerp(currentTurnStrafe, targetAimDown, Time.deltaTime * turnStrafeSpeed);
        anim.SetFloat("TurnStrafeLerp", newTurnStrafe);
    }

    public void SetForceStrafe(bool newForceStrafe)
    {
        forceStrafe = newForceStrafe;
    }

    private void HandleOrientation()
    {
        Transform mainCameraTrans = mainCamera.transform;
        Vector3 cameraForward = new Vector3(mainCameraTrans.forward.x, 0f, mainCameraTrans.forward.z);
        _inputDir = orientation.forward * _inputForward + orientation.right * _inputRight;
        orientation.forward = cameraForward;

        if (_inputAimDown || forceStrafe)
        {
            // Strafing combat style (used for ranged attacks)
            this.transform.forward = Vector3.Slerp(this.transform.forward, cameraForward, Time.deltaTime * 25f);
            // orientation.forward = cameraForward;

        } else {
            // Vector3 viewDir = this.transform.position - new Vector3(mainCameraTrans.position.x, this.transform.position.y, mainCameraTrans.position.z);
            // viewDir = viewDir.normalized;
            // orientation.forward = cameraForward;

            // Regular combat style (character forward is in direction of movement keys)
            if (_inputDir != Vector3.zero)
            {

                Quaternion targetRotation = Quaternion.LookRotation(_inputDir);

                // Handle abrupt direction changes with a slight right rotation offset (prevent ping ponging)
                if (Vector3.Dot(this.transform.forward, _inputDir) < -0.90f) // Check if facing almost opposite directions
                {
                    targetRotation = targetRotation * Quaternion.Euler(0, -45f, 0);
                }

                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 5f);
            }
            // Prevents the head from rotating wierdly when player is facing towards camera
            if (Vector3.Dot(this.transform.forward, cameraForward) <= 0f)
            {
                Debug.Log("Setting HEad Rig Weight to 0");
                SetHeadRigWeight(0);
            } else {
                Debug.Log("Setting HEad Rig Weight to 1");
                SetHeadRigWeight(1);
            }
        }
    }
    float velocity = 0;
    float smoothTime = 0.25f;  // The time it takes to reach the target smoothly
    private void SetHeadRigWeight(float desiredWeight)
    {
        
        
        headRig.weight = Mathf.SmoothDamp(headRig.weight, desiredWeight, ref velocity, smoothTime);
    }

    private void UpdateTargets()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        int fixedDistance = 99;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, fixedDistance, aimColliderLayerMask))
        {
            aimTarget.position = raycastHit.point;
        }
        else
        {
            aimTarget.position = ray.origin + ray.direction * fixedDistance;
        }
    }

    private void PerformGroundCheck()
    {
        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up * (radius * 0.5f);
        LayerMask groundLayer = LayerMask.GetMask("ground");
        isGrounded = GetGrounded || Physics.CheckSphere(pos, radius, groundLayer);
    }

    // Physics callback for collision
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("ground"))
        {
            ++groundContactCount;
            Debug.Log("COLLIDING WITH: " + collision.gameObject.name);
            Debug.Log("GroundedContactCount: " + groundContactCount);
            // Trigger landing event if necessary
            // EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.CompareTag("ground"))
        {
            --groundContactCount;
            Debug.Log("EXIT COLLIDING WITH: " + collision.gameObject.name);
            Debug.Log("GroundedContactCount: " + groundContactCount);
        }
    }

    void OnAnimatorMove()
    {
        // Used by RunMotionState for root motion running
        MotionStateMachine.OnAnimatorMove();
    }

    // Methods to handle global state changes
    private void HandleGlobalState()
    {
        // if (/* condition for staggered */ false)
        // {
        //     GlobalStateManager.ChangeState(GlobalState.Staggered);
        // }
        // if (/* condition for death */ false)
        // {
        //     GlobalStateManager.ChangeState(GlobalState.Dead);
        // }
    }

    #region Event and Animation Event Callback Handling
    // Animation Event Callbacks since functions from non-MonoBehavior classes do not get recognized
    public void PlayerJump()
    {
        // Check if current state is in jumpair before calling its specific PlayerJump() function
        if(MotionStateMachine.currentState is JumpAirMotionState) // 'is' keyword prevents invalid casts during runtime
        {
            JumpAirMotionState jumpState = (JumpAirMotionState)MotionStateMachine.currentState;
            jumpState.PlayerJump();
        }
    }
    #endregion

}

public enum GlobalState
{
    Normal,
    Staggered,
    Dead
}

public class GlobalStateManager
{
    private GlobalState currentState;
    private PlayerControlScript player;

    public GlobalStateManager(PlayerControlScript player)
    {
        this.player = player;
        currentState = GlobalState.Normal;
    }

    public void ChangeState(GlobalState newState)
    {
        if (currentState == GlobalState.Dead)
            return; // Dead is final state

        currentState = newState;

        switch (currentState)
        {
            case GlobalState.Normal:
                // Resume normal states
                break;
            case GlobalState.Staggered:
                // Handle staggered logic
                // player.ActionStateMachine.ChangeState(new IdleActionState(player));
                // player.MotionStateMachine.ChangeState(new IdleMotionState(player));
                // player.anim.SetTrigger("Staggered");
                break;
            case GlobalState.Dead:
                // Handle death logic
                // player.ActionStateMachine.ChangeState(new DeadActionState(player));
                // player.MotionStateMachine.ChangeState(new DeadMotionState(player));
                // player.anim.SetTrigger("Dead");
                break;
        }
    }

    public GlobalState GetCurrentState()
    {
        return currentState;
    }
}
