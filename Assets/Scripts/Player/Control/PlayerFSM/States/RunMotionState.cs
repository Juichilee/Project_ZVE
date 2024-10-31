using System.Collections;
using System;
using UnityEngine;

public class RunMotionState : BaseState
{
    public override Enum stateType => MotionStateType.Run;
    private PlayerControlScript player;

    public RunMotionState(PlayerControlScript player)
    {
        this.player = player;
    }

    public override void Enter()
    {
        Debug.Log("Entering Run State");
    }

    public override void Execute()
    {
        // Transition to Idle if no longer moving
        if (!player.isMoving)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.Idle);
        }

        // If player jumps or is not grounded, go to JumpAirMotion state
        if(player._inputJump || !player.isGrounded)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.JumpAir);
        }

        // Run Motion Logic
        player.anim.SetFloat("velz", player._inputForward);
        player.anim.SetFloat("velStrafe", player._inputRight);
        // Handle other motions like Dodge or Jump
        // if (player.cinput.Dodge)
        // {
        //     player.MotionStateMachine.ChangeState(new DodgeMotionState(player));
        // }

        // if (player.cinput.Jump && player.isGrounded)
        // {
        //     player.MotionStateMachine.ChangeState(MotionStateType.JumpAir);
        // }
    }

    // RunMotionState's movement is based off of root motion, hence OnAnimatorMove()
    public override void OnAnimatorMove()
    {
        Vector3 newRootPosition;

        if (player.isGrounded)
        {
            // Use root motion as is if on the ground
            newRootPosition = player.anim.rootPosition;

            // Scale the difference in position and rotation to make the character go faster or slower
            newRootPosition = Vector3.LerpUnclamped(player.transform.position, newRootPosition, player.rootMovementSpeed);

            player.rbody.MovePosition(newRootPosition);
        }
    }
}