using System.Collections;
using System;
using UnityEngine;
public class IdleMotionState : BaseState
{
    public override Enum stateType => MotionStateType.Idle;
    private PlayerControlScript player;

    public IdleMotionState(PlayerControlScript player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        // Debug.Log("Entering Idle State");
    }
    public override void Execute()
    {
        // Idle Motion Logic
        // Used as a rest state for transitioning to other non-idle states

        // Transition to Run or other motion states based on input
        if (player.IsMoving && player.IsGrounded)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.Run);
        }

        // If player jumps or is not grounded, go to JumpAirMotion state
        if(!player.IsGrounded || player.CanJump && player.InputJump)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.JumpAir);
        }

        player.Anim.SetFloat("velz", player.InputForward);
        player.Anim.SetFloat("velStrafe", player.InputRight);
    }

    public override void OnAnimatorMove()
    {
        Vector3 newRootPosition;

        // Use root motion as is if on the ground
        newRootPosition = player.Anim.rootPosition;

        // Scale the difference in position and rotation to make the character go faster or slower
        newRootPosition = Vector3.LerpUnclamped(player.transform.position, newRootPosition, player.RootMovementSpeed);

        player.Rbody.MovePosition(newRootPosition);
    }
}
