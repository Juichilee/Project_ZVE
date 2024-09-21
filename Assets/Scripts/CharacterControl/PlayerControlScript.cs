
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

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

    #region Controller Input Reading & Caching
    private CharacterInputController cinput;
        // classic input system only polls in Update()
    // so must treat input events like discrete button presses as
    // "triggered" until consumed by FixedUpdate()...
    // ...however constant input measures like axes can just have most recent value
    // cached.
    bool _inputActionFired = false;
    float _inputForward = 0f;
    float _inputRight = 0f;
    float _inputTurn = 0f;
    bool _inputJump = false;
    #endregion

    [Header("Movement & Animation")]
    public float speedMultiplier = 1f; // Sets the animationSpeed, rootMovementSpeed, and rootTurnSpeed
    private float animationSpeed = 1f;
    private float rootMovementSpeed = 1f;
    private float rootTurnSpeed = 1f;
    public float maxVerticalSpeed = 10f; // Maximum falling speed for falling animation blend
    public float groundDrag = 0f;
    public float airDrag = 0.2f;
    public float jumpableGroundNormalMaxAngle = 45;
    public bool closeToJumpableGround;
    private float prev_inputForward; // Used to preserve direction and momentum before jump
    public float airTurnSpeed = 270f; 
    public float rotationSpeed = 30f;
    // Base air forward speed allowing user to move while in air (i.e., how far the player is allowed to move in air after jumping in place)
    public float baseAirForwardSpeed = 1.5f; 
    public float horizontalSpeed = 5f;
    public float jumpForce = 100f;
    public float jumpCooldown = 1f;
    public bool isJumping;
    public bool hasJumped = false;
    public bool multiJump = false;
    public bool cameraTurningPlayer = true;
    private Coroutine jumpRoutine = null;

    [Header("Physics")]
    // Variables to store calculated character's directional velocity
    public Vector3 localVelocity = new Vector3();
    private Vector3 prevWorldPosition;
    private Vector3 worldVelocity;
    public Vector3 prevVelocity = new Vector3();

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

        // if (leftFoot == null || rightFoot == null)
        //     Debug.Log("One of the feet could not be found");
        UnityEngine.Cursor.lockState = CursorLockMode.Locked;
        UnityEngine.Cursor.visible = false;
    }

    private void Update()
    {
        if (cinput.enabled)
        {
            _inputForward = cinput.Forward; // for translating forward and backward
            _inputRight = cinput.Right; // For translating side to side

            // Note that we don't overwrite a true value already stored
            // Is only cleared to false in FixedUpdate()
            // This makes certain that the action is handled!
            _inputActionFired = _inputActionFired || cinput.Action;
        }
        // Third Person Camera Based Directional Input
        Transform mainCameraTrans = mainCamera.transform;
        Vector3 viewDir = transform.position - new Vector3(mainCameraTrans.position.x, transform.position.y, mainCameraTrans.position.z);
        if(_inputForward > 0){
            orientation.forward = viewDir.normalized;
        } else {
            orientation.forward = -viewDir.normalized;
        }


        // Handle drag
        if (isGrounded){
            rbody.drag = groundDrag;
            prev_inputForward = _inputForward; // Preserve previous inputForward momentum
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

        anim.SetFloat("velx", _inputTurn);
        anim.SetFloat("velz", _inputForward);
        anim.SetFloat("vely", normalizedVerticalSpeed);
        anim.SetBool("isFalling", !isGrounded);
        // anim.SetBool("doButtonPress", doButtonPress);
        // anim.SetBool("matchToButtonPress", doMatchToButtonPress);

        // Handle jump
        if (multiJump)
        {
            _inputJump = hasJumped ? false : _inputJump;
        } else {
            _inputJump = hasJumped || !isGrounded ? false : _inputJump; // If multi-jump is not enabled, then player must be grounded before jump
        }

        if (_inputJump)
        {
            // Store prevVelocity for setting forward jump velocity in future frame
            prevVelocity = localVelocity;

            anim.SetBool("startJump", true);
            anim.SetBool("isJumping", true);
            hasJumped = true;
            if(jumpRoutine != null)
            {
                StopCoroutine(jumpRoutine);
            }
            jumpRoutine = StartCoroutine(Jump());

        } else {
            anim.SetBool("startJump", false);
        }

        // Camera Turning Input
        Vector3 inputDir = orientation.forward * _inputForward;

        if (cameraTurningPlayer) //  && inputDir != Vector3.zero  Turn on and off camera based player turning
        {
            
            Vector3 newForward = Vector3.Lerp(transform.forward, inputDir.normalized, Time.fixedDeltaTime * rotationSpeed);

            // Calculate the cross product to determine turn direction
            Vector3 crossProduct = Vector3.Cross(orientation.forward.normalized, transform.forward.normalized);

            // Check the Y component of the cross product to determine if turning left or right
            // if (crossProduct.y > 0)
            // {
            //     Debug.Log("Turning Right");
            // }
            // else if (crossProduct.y < 0)
            // {
            //     Debug.Log("Turning Left");
            // }
            // else
            // {
            //     Debug.Log("Not Turning (or very small angle)");
            // }
            _inputTurn = -Mathf.Clamp(crossProduct.y, -1f, 1f);
        }
    }

    private void ResetJump()
    {
        hasJumped = false;
    }

    private IEnumerator JumpResetDelay()
    {
        yield return new WaitForSeconds(jumpCooldown);
        ResetJump();
    }
    private IEnumerator Jump()
    {
        yield return new WaitForSeconds(0.2f); // Wait for the jump state animation to start before actual jump physics (sum of transition duration)
        // reset y velocity before jump
        rbody.velocity = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);
        rbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
        StartCoroutine(JumpResetDelay());
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
        Quaternion newRootRotation;
        AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(0);

        if (isGrounded)
        {
            Debug.Log("Root MOTION Control");
         	//use root motion as is if on the ground		
            newRootPosition = anim.rootPosition;    
            //use rotational root motion as is
            newRootRotation = anim.rootRotation;

            // Movement sideways (strafing)
            // Vector3 sideMovement = transform.right * _inputRight * horizontalSpeed * Time.fixedDeltaTime;

            //TODO Here, you could scale the difference in position and rotation to make the character go faster or slower
            newRootPosition = Vector3.LerpUnclamped(-this.transform.position, newRootPosition, rootMovementSpeed);
            newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, rootTurnSpeed);

            
            rbody.MovePosition(newRootPosition);
            rbody.MoveRotation(newRootRotation);    
        }
        else
        { 
            Debug.Log("Kinematic Control");
            // If not grounded, still allow the player to be controlled in the air
            // Calculate the amount of rotation for this frame
            float turnAmount = _inputTurn * (airTurnSpeed * speedMultiplier) * Time.fixedDeltaTime;

            // Allow the player to control forward and backward movement in air slightly
            float inputForwardBlend = Mathf.Clamp(prev_inputForward + _inputForward, -1, 1);
            
            // Create a Quaternion for the turn (rotate around the Y-axis)
            Quaternion turnRotation; 
            if(inputForwardBlend > 0){
                turnRotation = Quaternion.Euler(0f, turnAmount, 0f); // Forwards Turning
            } else {
                turnRotation = Quaternion.Euler(0f, -turnAmount, 0f); // Backwards Turning
            }

            // Apply the rotation to the Rigidbody's current rotation
            rbody.MoveRotation(rbody.rotation * turnRotation);

            float forwardVelocity = Math.Max(baseAirForwardSpeed, prevVelocity.z) * speedMultiplier; // The floor for forward velocity is baseAirForwardSpeed
            Vector3 moveDir = transform.forward.normalized * inputForwardBlend * forwardVelocity;
            rbody.MovePosition(rbody.position + moveDir * Time.fixedDeltaTime);  
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
