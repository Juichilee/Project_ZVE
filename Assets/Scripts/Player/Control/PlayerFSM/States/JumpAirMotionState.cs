using System.Collections;
using System;
using UnityEngine;
public class JumpAirMotionState : BaseState
{
    public override Enum stateType => MotionStateType.JumpAir;
    private PlayerControlScript player;

    private const float jumpForce = 200f;
    private const float horizontalBoost = 200f;
    private const float jumpCooldown = 1f; // Controls when player can change to different states after a jump
    private const float jumpResetCooldown = 1f; // Controls when player can jump again
    private bool hasJumped = false;
    private bool canJump = true;
    private bool multiJump = false;
    private const float maxVerticalSpeed = 10f; // Maximum falling speed for falling animation blend
    private const float maxMidairControlSpeed = 10f;
    private const float midairControlForce = 2f;

    public JumpAirMotionState(PlayerControlScript player)
    {
        this.player = player;
    }

    public override void Execute()
    {
        // Map local velocity to a value between -1 and 1 (0 = rest or grounded)
        float normalizedVerticalSpeed = Mathf.Clamp(player.LocalVelocity.y / maxVerticalSpeed, -1, 1);
        player.Anim.SetFloat("vely", normalizedVerticalSpeed);

        // Transition to other states only when player is grounded and hasn't started jump
        if (player.IsGrounded && !hasJumped)
        {
            if (!player.IsMoving)
            {
                player.MotionStateMachine.ChangeState(MotionStateType.Idle);
            } else if (player.IsMoving) {
                player.MotionStateMachine.ChangeState(MotionStateType.Run);
            }
        }
    }

    public override void FixedExecute()
    {
        HandleJumpInput();
    }

    private void HandleJumpInput()
    {
        bool jumpInput = false;
        if (!multiJump)
        {
            jumpInput = player.IsGrounded && player.InputJump; // If multi-jump is not enabled, then player must be grounded before jump
        }

        if (canJump && !hasJumped && jumpInput)
        {
            player.Anim.SetTrigger("startJump");
            hasJumped = true;
            canJump = false;
        }

        if (!player.IsGrounded)
        {
            MidAirControl();
        }
    }

    private IEnumerator JumpResetDelay()
    {
        yield return new WaitForSeconds(jumpCooldown);
        hasJumped = false;
        yield return new WaitForSeconds(jumpResetCooldown);
        canJump = true;
        // Update PlayerControlScript canJump
        player.CanJump = canJump;
    }

    // PlayerJump() is called by animation event from the jump animation
    public void PlayerJump()
    {
        Vector3 verticalForce = Vector3.up * jumpForce;
        player.Rbody.velocity = new Vector3(player.Rbody.velocity.x, 0f, player.Rbody.velocity.z); // reset y velocity before jump
        Vector3 horizontalForce = player.InputDir * horizontalBoost;
        Vector3 totalForce = (verticalForce + horizontalForce) * player.speedMultiplier * 2; 
        player.Rbody.AddForce(totalForce, ForceMode.Impulse);
        player.StartCoroutine(JumpResetDelay()); // Reference StartCoroutine that belongs to player MonoBehavior class
    }

    void MidAirControl()
    {
        if (player.IsMoving)
        {
            Vector3 horizontalForce = player.InputDir * midairControlForce * player.speedMultiplier;
            player.Rbody.AddForce(horizontalForce, ForceMode.Impulse);

            // Get horizontal velocity
            Vector3 horizontalVelocity = new Vector3(player.Rbody.velocity.x, 0f, player.Rbody.velocity.z);

            // Clamp horizontal speed using Vector3.ClampMagnitude
            horizontalVelocity = Vector3.ClampMagnitude(horizontalVelocity, maxMidairControlSpeed * player.speedMultiplier);

            // Update Rigidbody's velocity
            player.Rbody.velocity = new Vector3(horizontalVelocity.x, player.Rbody.velocity.y, horizontalVelocity.z);
        }
    }
}