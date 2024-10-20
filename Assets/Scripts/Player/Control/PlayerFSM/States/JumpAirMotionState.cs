using System.Collections;
using System;
using UnityEngine;
public class JumpAirMotionState : BaseState
{
    public override Enum stateType => MotionStateType.JumpAir;
    private PlayerControlScript player;

    public float jumpForce = 10f;
    public float jumpCooldown = 1f;
    private bool hasJumped = false;
    public bool multiJump = false;
    private float maxVerticalSpeed = 10f; // Maximum falling speed for falling animation blend
    public float maxMidairControlSpeed = 10f;
    public float midairControlForce = 10f;

    public JumpAirMotionState(PlayerControlScript player)
    {
        this.player = player;
    }

    public override void Execute()
    {
        // Map local velocity to a value between 0 and 1 (0.5 = rest or grounded)
        float normalizedVerticalSpeed = (player.localVelocity.y + maxVerticalSpeed) / (2 * maxVerticalSpeed);
        player.anim.SetFloat("vely", normalizedVerticalSpeed);

        // Transition to other states only when player is grounded and hasn't started jump
        if (player.isGrounded && !hasJumped)
        {
            if (!player.isMoving)
            {
                player.MotionStateMachine.ChangeState(MotionStateType.Idle);
            } else if (player.isMoving) {
                player.MotionStateMachine.ChangeState(MotionStateType.Run);
            }
        }

        // if (player.isGrounded && player.isMoving)
        // {
        //     player.MotionStateMachine.ChangeState(MotionStateType.Run);
        // }

        // if (player.cinput.Dodge)
        // {
        //     player.MotionStateMachine.ChangeState(new DodgeMotionState(player));
        // }
    }

    public override void FixedExecute()
    {
        HandleJumpInput();
    }

    // public void Exit()
    // {
    //     // Cleanup Run Motion
    //     // player.anim.SetBool("isRunning", false);
    // }

    private void HandleJumpInput()
    {
        if (!multiJump)
        {
            player._inputJump = !hasJumped && player.isGrounded && player._inputJump; // If multi-jump is not enabled, then player must be grounded before jump
        }

        if (player._inputJump)
        {
            player.anim.SetBool("startJump", true);
            hasJumped = true;
        }

        if (!player.isGrounded)
        {
            MidAirControl();
        }
    }

    private void ResetJump()
    {
        hasJumped = false;
        player.anim.SetBool("startJump", false);
    }

    private IEnumerator JumpResetDelay()
    {
        yield return new WaitForSeconds(jumpCooldown);
        ResetJump();
    }

    // PlayerJump() is called by animation event from the jump animation
    public void PlayerJump()
    {
        Vector3 verticalForce = Vector3.up * jumpForce;
        player.rbody.velocity = new Vector3(player.rbody.velocity.x, 0f, player.rbody.velocity.z); // reset y velocity before jump
        Vector3 horizontalForce = player._inputDir * jumpForce;
        Vector3 totalForce = verticalForce + horizontalForce;
        player.rbody.AddForce(totalForce, ForceMode.VelocityChange);
        player.StartCoroutine(JumpResetDelay()); // Reference StartCoroutine that belongs to player MonoBehavior class
    }

    void MidAirControl()
    {
        if (player.isMoving)
        {
            Vector3 horizontalForce = player._inputDir * midairControlForce;
            player.rbody.AddForce(horizontalForce, ForceMode.Impulse);

            // Get horizontal velocity
            Vector3 horizontalVelocity = new Vector3(player.rbody.velocity.x, 0f, player.rbody.velocity.z);

            // Clamp horizontal speed using Vector3.ClampMagnitude
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxMidairControlSpeed);

            // Update Rigidbody's velocity
            player.rbody.velocity = new Vector3(horizontalVelocity.x, player.rbody.velocity.y, horizontalVelocity.z);
        }
    }
}