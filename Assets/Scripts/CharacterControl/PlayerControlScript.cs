
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEditor.Experimental.GraphView;
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

    #region Controller Input Reading & Caching
    private CharacterInputController cinput;
        // classic input system only polls in Update()
    // so must treat input events like discrete button presses as
    // "triggered" until consumed by FixedUpdate()...
    // ...however constant input measures like axes can just have most recent value
    // cached.
    bool _inputActionFired = false;
    float _inputForward = 0f;
    float _inputTurn = 0f;
    #endregion

    [Header("Movement & Animation")]
    public float animationSpeed = 1f;
    public float rootMovementSpeed = 1f;
    public float rootTurnSpeed = 1f;
    public float maxVerticalSpeed = 10f;
    public float groundDrag;
    public float jumpableGroundNormalMaxAngle = 45;
    public bool closeToJumpableGround;
    public float airSpeedMultiplier= 200f; // How much faster the user moves in the air
    public float airTurnSpeed = 270f;
    public float jumpForce;
    public float jumpCooldown;
    public bool readyToJump;

    [Header("Physics")]
    // Variables to store calculated character's directional velocity
    public Vector3 localVelocity = new Vector3();
    public Vector3 prevWorldVelocity;
    public Vector3 worldVelocity;

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
    }

    private void Update()
    {
        if (cinput.enabled)
        {
            _inputForward = cinput.Forward;
            _inputTurn = cinput.Turn;

            // Note that we don't overwrite a true value already stored
            // Is only cleared to false in FixedUpdate()
            // This makes certain that the action is handled!
            _inputActionFired = _inputActionFired || cinput.Action;

        }
        // Handle drag
        if (isGrounded){
            rbody.drag = groundDrag;
        }else{
            rbody.drag = 0;
        }
        if (Input.GetKey(KeyCode.Space) && readyToJump && isGrounded)
        {
            readyToJump = false;
            Jump();
            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }


    void FixedUpdate()
    {

        // Physics calculations for rigidbody because animator velocity is inaccurate
        worldVelocity = (transform.position - prevWorldVelocity) / Time.fixedDeltaTime;
        prevWorldVelocity = transform.position;

        // Project the world velocity onto the character's local axes
        localVelocity.z = Vector3.Dot(worldVelocity, transform.forward);
        localVelocity.y = Vector3.Dot(worldVelocity, transform.up);
        localVelocity.x = Vector3.Dot(worldVelocity, transform.right);

        // Debug.Log(localVelocity);

        float normalizedVerticalSpeed = (localVelocity.y + maxVerticalSpeed) / (2 * maxVerticalSpeed);

        this.anim.speed = animationSpeed;
        // bool doButtonPress = false;
        // bool doMatchToButtonPress = false;

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

        if (isGrounded)
        {
         	//use root motion as is if on the ground		
            newRootPosition = anim.rootPosition;    
            //use rotational root motion as is
            newRootRotation = anim.rootRotation;

            //TODO Here, you could scale the difference in position and rotation to make the character go faster or slower
            newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
            newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, rootTurnSpeed);

            rbody.MovePosition(newRootPosition);
            rbody.MoveRotation(newRootRotation);    
        }
        else
        { 
            // If not grounded, still allow the player to be controlled in the air
            // Calculate the amount of rotation for this frame
            float turnAmount = _inputTurn * airTurnSpeed * Time.fixedDeltaTime;

            // Create a Quaternion for the turn (rotate around the Y-axis)
            Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);

            // Apply the rotation to the Rigidbody's current rotation
            rbody.MoveRotation(rbody.rotation * turnRotation);

            // Map user inputs (x, z) to a unit direction
            Vector3 moveDirection = transform.forward * _inputForward + transform.right * _inputTurn;
            rbody.AddForce(moveDirection.normalized * airSpeedMultiplier, ForceMode.Force);
        }
        
    }

    private void Jump()
    {
        // reset y velocity before jump
        rbody.velocity = new Vector3(rbody.velocity.x, 0f, rbody.velocity.z);
        rbody.AddForce(transform.up * jumpForce, ForceMode.Impulse);
    }
    private void ResetJump()
    {
        readyToJump = true;
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
