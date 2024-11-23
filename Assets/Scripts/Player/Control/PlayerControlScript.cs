using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Animations.Rigging;
using Unity.VisualScripting;

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
    public Animator Anim { get => anim; private set => anim = value;}
    private Animator anim;
    public Rigidbody Rbody { get => rbody; private set => rbody = value;}
    private Rigidbody rbody;
    // NOTE: Still need to figure out some way to find and set in awake the below references
    public Camera mainCamera;
    public Transform orientation;
    public Transform spawn;
    public Transform aimTarget; 
    public LayerMask aimColliderLayerMask;
    private float aimSpeed = 50f;
    public LayerMask groundLayerMask;
    private float slopeCheckDistance = 1f; // Adjust based on player height
    public Rig headRig; 
    private MultiAimConstraint headAim;
    private PlayerStatus playerStatus;

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
    private CharacterInputController cinput;
    public float InputForward { get => _inputForward; private set => _inputForward = value; }
    private float _inputForward = 0f;
    public float InputRight { get => _inputRight; private set => _inputRight = value; }
    private float _inputRight = 0f;
    public bool InputJump { get => _inputJump; private set => _inputJump = value; }
    private bool _inputJump = false;
    public bool InputAbility1 { get => _inputAbility1; private set => _inputAbility1 = value; }
    private bool _inputAbility1 = false;
    public bool InputAbility2 { get => _inputAbility2; private set => _inputAbility2 = value; }
    private bool _inputAbility2 = false;
    public bool InputAbility3 { get => _inputAbility3; private set => _inputAbility3 = value; }
    private bool _inputAbility3 = false;
    public bool InputAimDown { get => _inputAimDown; private set => _inputAimDown = value; }
    private bool _inputAimDown = false;
    public bool InputAttack { get => _inputAttack; private set => _inputAttack = value; }
    private bool _inputAttack = false;
    public bool InputHoldAttack { get => _inputHoldAttack; private set => _inputHoldAttack = value; }
    private bool _inputHoldAttack = false;
    public bool Interact { get => _interact; private set => _interact = value; }
    private bool _interact = false;
    public bool Drop { get => _drop; private set => _drop = value; }
    private bool _drop = false;
    public bool Reload { get => _reload; private set => _reload = value; }
    private bool _reload = false;
    public Vector3 InputDir { get => _inputDir; private set => _inputDir = value; }
    private Vector3 _inputDir;
    // Can be set by outside components to force player to strafe
    public bool ForceStrafe { get => forceStrafe; set => forceStrafe = value;}
    private bool forceStrafe = false; 
    // Can be set by outside components to force input to be disabled (e.g, by ability activations)
    public bool ForceDisableInput { get => forceDisableInput; set => forceDisableInput = value;}
    private bool forceDisableInput = false;
    public bool ForceDisableJump { get => _forceDisableJump; set => _forceDisableJump = value;}
    private bool _forceDisableJump = false;
    #endregion

    #region Movement & Animation Properties
    public float speedMultiplier = 1f; // Sets the animationSpeed, rootMovementSpeed, and rootTurnSpeed
    private float animationSpeed = 1f;
    public float RootMovementSpeed { get => rootMovementSpeed; private set => rootMovementSpeed = value;}
    private float rootMovementSpeed = 1f;
    private float groundDrag = 0f;
    private float airDrag = 0.2f;
    public bool IsMoving { get => isMoving; private set => isMoving = value; }
    private bool isMoving = false;
    private float turnStrafeSpeed = 10f;
    public float upgradeMult = .1f;
    private bool canJump = true;
    public bool CanJump { get => canJump; set => canJump = value; }
    #endregion

    #region Environmental/Sensor Properties
    public Vector3 WorldVelocity { get => worldVelocity; private set => worldVelocity = value; }
    private Vector3 worldVelocity;
    public Vector3 LocalVelocity { get => localVelocity; private set => localVelocity = value; }
    private Vector3 localVelocity = Vector3.zero;
    private Vector3 prevWorldPosition;
    public bool IsGrounded { get => _isGrounded; private set => _isGrounded = value; }
    private bool _isGrounded = true;

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

        headAim = headRig.transform.Find("HeadAim").GetComponent<MultiAimConstraint>();

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
            // groundContactCount = 0;
        }
    }
    // Responsible for reading (and caching) input, updating certain global environment properties, and interrupting global states
    private void Update()
    {
        // Debug.Log("Player velocity: " + WorldVelocity);

        if (cinput.enabled)
        {
            _inputForward = cinput.Forward;
            _inputRight = cinput.Right;
            _inputAimDown = cinput.AimDown;
            _inputAttack = cinput.Attack;
            _inputHoldAttack = cinput.HoldAttack;
            _inputAbility1 = cinput.Ability1;
            _inputAbility2 = cinput.Ability2;
            _inputAbility3 = cinput.Ability3;
            _inputJump = ForceDisableJump ? false : cinput.Jump;
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

        // Update Global Animator Parameters
        anim.SetBool("isGrounded", _isGrounded);
        anim.SetBool("aimDown", _inputAimDown);
    }

    void FixedUpdate()
    {
        // Handle drag based on grounded state
        rbody.drag = _isGrounded ? groundDrag : airDrag;

        // Physics calculations for Rigidbody because animator velocity is inaccurate
        worldVelocity = (transform.position - prevWorldPosition) / Time.fixedDeltaTime;
        prevWorldPosition = transform.position;

        // Project the world velocity onto the character's local axes
        localVelocity.z = Vector3.Dot(worldVelocity, transform.forward);
        localVelocity.y = Vector3.Dot(worldVelocity, transform.up);
        localVelocity.x = Vector3.Dot(worldVelocity, transform.right);

        // Update all animation/root speed based on speed multiplier
        animationSpeed = speedMultiplier + (playerStatus.speedUpgrade * upgradeMult);
        rootMovementSpeed = speedMultiplier + (playerStatus.speedUpgrade * upgradeMult);
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

        // Smoothly transition the aimDownLerp parameter (Determines blending between strafe and turning animations)
        float targetAimDown = 0f;
        if (forceStrafe){
            targetAimDown = 1f;
        }
        float currentTurnStrafe = anim.GetFloat("TurnStrafeLerp");
        float newTurnStrafe = Mathf.Lerp(currentTurnStrafe, targetAimDown, Time.deltaTime * turnStrafeSpeed);
        anim.SetFloat("TurnStrafeLerp", newTurnStrafe);
    }

    private void HandleOrientation()
    {
        Transform mainCameraTrans = mainCamera.transform;
        Vector3 cameraForward = new Vector3(mainCameraTrans.forward.x, 0f, mainCameraTrans.forward.z);
        orientation.forward = cameraForward;

        _inputDir = orientation.forward * _inputForward + orientation.right * _inputRight;
        
        // forceStrafe is mostly set outside this script (e.g., WeaponHandler)
        if (forceStrafe)
        {
            // Strafing combat style (used for ranged attacks)
            this.transform.forward = Vector3.Slerp(this.transform.forward, cameraForward, Time.deltaTime * 25f);
        } else {
            // Regular combat style (character forward is in direction of movement keys)
            if (_inputDir != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(_inputDir);

                // Handle abrupt direction changes with a slight right rotation offset (prevent ping ponging)
                // if (Vector3.Dot(this.transform.forward, _inputDir) < 0f) // Check for abrupt change in direction
                // {
                //     targetRotation = targetRotation * Quaternion.Euler(0, -45f, 0);
                // }
                // this.transform.rotation = Quaternion.Slerp(this.transform.rotation, targetRotation, Time.deltaTime * 25f);
                this.transform.rotation = targetRotation;
            }
            // When player is facing the camera, set head aim target to forward head
            if (Vector3.Dot(this.transform.forward, cameraForward) <= 0f)
            {
                SetMultiAimSourceWeight(headAim, 0, 1f); // Set forwardHeadTarget target weight to 1
                SetMultiAimSourceWeight(headAim, 1, 0f); // Set aimTarget target weight to 0
            } else {
                SetMultiAimSourceWeight(headAim, 0, 0f); 
                SetMultiAimSourceWeight(headAim, 1, 1f);
            }
        }
    }
    public static void SetMultiAimSourceWeight(MultiAimConstraint aim, int idx, float val)
    {
        var aimSources = aim.data.sourceObjects;
        float currentWeight = aimSources[idx].weight;
        currentWeight = Mathf.Lerp(currentWeight, val, Time.deltaTime * 5f);
        aimSources.SetWeight(idx, currentWeight);
        aim.data.sourceObjects = aimSources;
    }

    private void UpdateTargets()
    {
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        int fixedDistance = 99;
        Vector3 aimTargetPos;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, fixedDistance, aimColliderLayerMask))
        {
            aimTargetPos = raycastHit.point;
        }
        else
        {
            aimTargetPos = ray.origin + ray.direction * fixedDistance; // If the ray doesn't hit anything, set target a fixed distance
        }

        // Lerp between aimtargetPos and current aimTarget.position
        aimTarget.position = Vector3.Lerp(aimTarget.position, aimTargetPos, Time.deltaTime * aimSpeed);
    }

    private void PerformGroundCheck()
    {
        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up * (radius * 0.5f);
        _isGrounded = Physics.CheckSphere(pos, radius, groundLayerMask);
    }

    // Adjusts position to factor in moving up or down a slope
    public Vector3 CalculateSlopeAdjustedPos(Vector3 position)
    {
        // Debug.DrawRay(position + Vector3.up * 0.1f, Vector3.down * (slopeCheckDistance + 0.1f), Color.red);
        RaycastHit hit;
        if (Physics.Raycast(position + Vector3.up * 0.1f, Vector3.down, out hit, slopeCheckDistance + 0.1f, groundLayerMask))
        {
            float hitY = hit.point.y;
            float positionY = position.y;

            if (hitY < positionY)
            {
                // Adjust y position down to ground level when moving down a slope
                position.y = hitY;
            }
        }
        return position;
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
