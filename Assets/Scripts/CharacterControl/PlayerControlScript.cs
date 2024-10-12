
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.EventSystems;

#if UNITY_EDITOR
using UnityEditor;
#endif

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterInputController))]
public class PlayerControlScript : MonoBehaviour
{
    private Animator anim;	
    private Rigidbody rbody;
    public Camera mainCamera;
    public Transform orientation;
    public MeleeAttack meleeAttack;

    #region Controller Input Reading & Caching
    private CharacterInputController cinput;
    bool _inputActionFired = false;
    float _inputForward = 0f;
    float _inputRight = 0f;
    float _inputTurn = 0f;
    bool _inputJump = false;
    bool _inputAimDown = false;
    #endregion

    [Header("Movement & Animation")]
    public float speedMultiplier = 1f; // Sets the animationSpeed, rootMovementSpeed, and rootTurnSpeed
    public float animationSpeed = 1f;
    public float rootMovementSpeed = 1f;
    public float rootTurnSpeed = 1f;
    public float maxVerticalSpeed = 10f; // Maximum falling speed for falling animation blend
    public float groundDrag = 0f;
    public float airDrag = 0.2f;
    public float jumpForce = 10f;
    public float jumpCooldown = 1f;
    public bool hasJumped = false;
    public bool multiJump = false;
    public bool isMoving = false;
    public Vector3 inputDir;

    [Header("Physics")]
    // Variables to store calculated character's directional velocity
    public Vector3 localVelocity = new Vector3();
    private Vector3 prevWorldPosition;
    private Vector3 worldVelocity;
    // public Vector3 prevVelocity = new Vector3();
    public float maxMidairControlSpeed = 10f;
    public float midairControlForce = 10f;

    [Header("Ground Check")]
    private int groundContactCount = 0;
    public bool isGrounded = true;
    public float playerHeight;
    public LayerMask whatIsGround;

    public bool GetGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }

    void Awake()
    {

        anim = GetComponent<Animator>();

        if (anim == null)
            Debug.Log("Animator could not be found");

        rbody = GetComponent<Rigidbody>();

        if (rbody == null)
            Debug.Log("Rigid body could not be found");

        cinput = GetComponent<CharacterInputController>();
        if (cinput == null)
            Debug.Log("CharacterInput could not be found");
    }


    // Use this for initialization
    void Start()
    {
		//example of how to get access to certain limbs
        // leftFoot = this.transform.Find("mixamorig:Hips/mixamorig:LeftUpLeg/mixamorig:LeftLeg/mixamorig:LeftFoot");
        // rightFoot = this.transform.Find("mixamorig:Hips/mixamorig:RightUpLeg/mixamorig:RightLeg/mixamorig:RightFoot");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        if (cinput.enabled)
        {
            _inputForward = cinput.Forward;
            _inputRight = cinput.Right;
            _inputActionFired = _inputActionFired || cinput.Action;
            _inputAimDown = cinput.AimDown;
        }
        
        Transform mainCameraTrans = mainCamera.transform;
        Vector3 viewDir = this.transform.position - new Vector3(mainCameraTrans.position.x, this.transform.position.y, mainCameraTrans.position.z);
        viewDir = viewDir.normalized;
        orientation.forward = viewDir;
        inputDir = orientation.forward * _inputForward + orientation.right * _inputRight;

        if (!_inputAimDown) 
        {
            // Regular combat style (character forward is in direction of movement keys)
            if (inputDir != Vector3.zero)
            {
                this.transform.forward = Vector3.Slerp(this.transform.forward, inputDir, Time.deltaTime * 20f);
            }
        } else {
            Vector3 cameraForward = new Vector3(mainCameraTrans.forward.x, 0f, mainCameraTrans.forward.z);
            this.transform.forward = Vector3.Slerp(this.transform.forward, cameraForward, Time.deltaTime * 5f);
        }
        
        // Check if the player is moving based on input
        isMoving = Mathf.Abs(_inputForward) > 0.05f || Mathf.Abs(_inputRight) > 0.05f;

        // Handle drag
        if (isGrounded){
            rbody.drag = groundDrag;
        }else{
            rbody.drag = airDrag;
        }

        _inputJump = cinput.Jump;
    }

    void FixedUpdate()
    {
        // Physics calculations for rigidbody because animator velocity is inaccurate
        worldVelocity = (transform.position - prevWorldPosition) / Time.fixedDeltaTime;
        prevWorldPosition = transform.position;

        // Project the world velocity onto the character's local axes
        localVelocity.z = Vector3.Dot(worldVelocity, transform.forward);
        localVelocity.y = Vector3.Dot(worldVelocity, transform.up);
        localVelocity.x = Vector3.Dot(worldVelocity, transform.right);

        // Map local velocity to a value between 0 and 1 (0.5 at rest). 
        float normalizedVerticalSpeed = (localVelocity.y + maxVerticalSpeed) / (2 * maxVerticalSpeed);

        // Update all animation/root speed based on speed multiplier
        animationSpeed = speedMultiplier;
        rootMovementSpeed = speedMultiplier;
        rootTurnSpeed = speedMultiplier;
        // Update animation speed if necessary
        this.anim.speed = animationSpeed;

        // Check if isGrounded

        // isGrounded = GetGrounded || CharacterCommon.CheckGroundNear(this.transform.position, jumpableGroundNormalMaxAngle, 0.5f, 1f, out closeToJumpableGround, whatIsGround);
        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        //get the position (assuming its right at the bottom) and move it up by almost the whole radius
        Vector3 pos = transform.position + Vector3.up*(radius*0.5f);
        //returns true if the sphere touches something on that layer
        LayerMask groundLayer = LayerMask.GetMask("ground");
        isGrounded = GetGrounded || Physics.CheckSphere(pos, radius, groundLayer);

        // Smooth turning based on camera direction while moving
        Vector3 crossProduct = Vector3.Cross(orientation.forward.normalized, transform.forward.normalized);
        Vector3 characterForward = this.transform.forward;
        Vector3 cameraForwardAdjusted = mainCamera.transform.forward;

        float dotProduct = Vector3.Dot(characterForward.normalized, cameraForwardAdjusted.normalized);

        if (dotProduct >= 0) {
            _inputTurn = -crossProduct.y;
        }

        // Handle jump
        if (multiJump)
        {
            _inputJump = hasJumped ? false : _inputJump;
        } else {
            _inputJump = hasJumped || !isGrounded ? false : _inputJump; // If multi-jump is not enabled, then player must be grounded before jump
        }

        if (_inputJump)
        {
            anim.SetBool("startJump", true);
            hasJumped = true;
        } 

        if (!isGrounded)
        {
            MidAirControl();
        }

        if (_inputActionFired)
        {
            _inputActionFired = false;
            meleeAttack.startAttack();
        }

        anim.SetFloat("velx", _inputTurn);
        anim.SetFloat("velz", _inputForward);
        anim.SetFloat("vely", normalizedVerticalSpeed);
        anim.SetBool("isFalling", !isGrounded);
        anim.SetFloat("velStrafe", _inputRight);
        anim.SetBool("isMoving", isMoving);

        // Smooth transition speed, adjust this to control how fast it transitions
        float smoothSpeed = 5f;

        // Get the current value of the parameter
        float currentAimDown = anim.GetFloat("aimDown");

        // Set the target value (1 if aiming down, 0 if not)
        float targetAimDown = _inputAimDown ? 1f : 0f;

        // Smoothly transition the value
        float newAimDownValue = Mathf.Lerp(currentAimDown, targetAimDown, Time.deltaTime * smoothSpeed);

        // Set the new value to the animator
        anim.SetFloat("aimDown", newAimDownValue);
        // anim.SetBool("doButtonPress", doButtonPress);
        // anim.SetBool("matchToButtonPress", doMatchToButtonPress);
    }

    private void ResetJump()
    {
        hasJumped = false;
        anim.SetBool("startJump", false);
    }

    private IEnumerator JumpResetDelay()
    {
        yield return new WaitForSeconds(jumpCooldown);
        ResetJump();
    }
    // PlayerJump() is called by animation event and executes during jump frame
    public void PlayerJump()
    {
        Vector3 verticalForce = Vector3.up * jumpForce;
        if(!_inputAimDown)
        {
            // reset y velocity before jump
            rbody.velocity = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);
            Vector3 horizontalForce = inputDir * 5f;
            Vector3 totalForce = verticalForce + horizontalForce;
            rbody.AddForce(totalForce, ForceMode.VelocityChange);
        } else {
            // reset y velocity before jump
            rbody.velocity = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);
            Vector3 horizontalForce = inputDir * 5f;
            Vector3 totalForce = verticalForce + horizontalForce;
            rbody.AddForce(totalForce, ForceMode.VelocityChange);
        }

        StartCoroutine(JumpResetDelay());
    }

    void MidAirControl()
    {
        if (isMoving)
        {
            Vector3 horizontalForce = inputDir * midairControlForce;
            rbody.AddForce(horizontalForce, ForceMode.Impulse);

            // Get horizontal velocity
            Vector3 horizontalVelocity = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);

            // Clamp horizontal speed using Vector3.ClampMagnitude
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxMidairControlSpeed);

            // Update Rigidbody's velocity
            rbody.velocity = new Vector3(horizontalVelocity.x, rbody.velocity.y, horizontalVelocity.z);
        }

    }

    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {

            ++groundContactCount;
            EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);

        }
						
    }

    private void OnCollisionExit(Collision collision)
    {

        if (collision.transform.gameObject.tag == "ground")
        {
            --groundContactCount;
        }

    }

    void OnAnimatorMove()
    {
        Vector3 newRootPosition;
        // AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(0);

        if (isGrounded)
        {
         	//use root motion as is if on the ground		
            newRootPosition = anim.rootPosition;    

            //TODO Here, you could scale the difference in position and rotation to make the character go faster or slower
            newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
            
            rbody.MovePosition(newRootPosition);
        }
    }

    // void OnAnimatorIK(int layerIndex)
    // {
    //     AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(0);
    //     if(astate.IsName("ButtonPress")){
    //         float buttonWeight = anim.GetFloat("buttonClose"); // Setting weight to 1.0 for IK pose vs. current animation pose causes snapping. Need to animate button Weight
    //         // Set the look target position, if one has been assigned
    //         if(buttonObject != null){
    //             anim.SetLookAtWeight(buttonWeight);
    //             anim.SetLookAtPosition(buttonObject.transform.position);
    //             anim.SetIKPositionWeight(AvatarIKGoal.RightHand, buttonWeight);
    //             anim.SetIKPosition(AvatarIKGoal.RightHand, buttonObject.transform.position);
    //         }
    //     }
    //     else
    //     {
    //         // If not in button press state, reset the position and look at weight IK for the right hand
    //         anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
    //         anim.SetLookAtWeight(0);
    //     }
    // }
}
