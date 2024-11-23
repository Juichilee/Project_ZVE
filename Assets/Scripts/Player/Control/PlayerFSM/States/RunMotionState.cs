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
        // Debug.Log("Entering Run State");
    }

    public override void Execute()
    {
        // Transition to Idle if no longer moving
        if (!player.IsMoving)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.Idle);
        }

        // If player jumps or is not grounded, go to JumpAirMotion state
        if(player.InputJump || !player.IsGrounded)
        {
            player.MotionStateMachine.ChangeState(MotionStateType.JumpAir);
        }

        // Run Motion Logic
        player.Anim.SetFloat("velz", player.InputForward);
        player.Anim.SetFloat("velStrafe", player.InputRight);
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

        // Use root motion as is if on the ground
        newRootPosition = player.Anim.rootPosition;

        // Convert the new root position from world to local space relative to the player's transform
        Vector3 localRootPosition = player.transform.InverseTransformPoint(newRootPosition);

        // Set the x-component to zero to remove local x movement to prevent forward root animation deviations from affecting forward movement
        // I.e., forward anims like walk may not be perfectly straight, so this code removes any root anim on local x-axis to make it straight
        if (Mathf.Abs(player.InputForward) > 0.05f && Mathf.Abs(player.InputRight) <= 0.05f)
        {
            localRootPosition.x = 0;
        }
        
        // Convert the modified local position back to world space
        newRootPosition = player.transform.TransformPoint(localRootPosition);

        newRootPosition = player.CalculateSlopeAdjustedPos(newRootPosition);

        // Scale the difference in position and rotation to make the character go faster or slower
        newRootPosition = Vector3.LerpUnclamped(player.transform.position, newRootPosition, player.RootMovementSpeed);

        player.Rbody.MovePosition(newRootPosition);
        
    }
}