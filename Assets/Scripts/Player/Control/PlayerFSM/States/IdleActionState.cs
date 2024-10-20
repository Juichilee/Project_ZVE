using System.Collections;
using System;
using UnityEngine;
public class IdleActionState : BaseState
{
    public override Enum stateType => ActionStateType.Idle;
    private PlayerControlScript player;

    public IdleActionState(PlayerControlScript player)
    {
        this.player = player;
    }

    // public void Enter()
    // {
    //     // Initialize Idle Action
    // }

    // public void Execute()
    // {
        
    // }

    // public void Exit()
    // {
    //     // Cleanup Idle Action

    // }
}
