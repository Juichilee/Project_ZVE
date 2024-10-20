
// using System.Collections;
// using System;
// using UnityEngine;

// public class AttackActionState : IActionState
// {
//     private PlayerControlScript player;

//     public AttackActionState(PlayerControlScript player)
//     {
//         this.player = player;
//     }

//     public void Enter()
//     {
//         // Initialize Attack Action
//         player.anim.SetTrigger("Attack");
//     }

//     public void Execute()
//     {
//         // Attack Action Logic
//         // For example, perform attack and then return to Idle
//         if (player.anim.GetCurrentAnimatorStateInfo(0).IsName("Attack"))
//         {
//             // Wait for attack animation to finish
//             if (player.anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
//             {
//                 player.ActionStateMachine.ChangeState(new IdleActionState(player));
//             }
//         }
//     }

//     public void Exit()
//     {
//         // Cleanup Attack Action
//     }
// }

