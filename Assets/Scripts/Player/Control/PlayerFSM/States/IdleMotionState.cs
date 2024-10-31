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
        Debug.Log("Entering Idle State");
    }
    public override void Execute()
    {
        // Idle Motion Logic
        // Used as a rest state for transitioning to other non-idle states

        // Transition to Run or other motion states based on input
        if (player.isMoving && player.isGrounded)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.Run);
        }

        // If player jumps or is not grounded, go to JumpAirMotion state
        if(player._inputJump || !player.isGrounded)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.JumpAir);
        }
    }
}
