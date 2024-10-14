using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using GameAI;
using Unity.VisualScripting;
using UnityEditor.Build;
using UnityEngine;
using UnityEngine.UIElements;

public enum ZombieState 
{
    Patrol,
    Chase,
    Attack
}

public class ZombieStateMachine : MonoBehaviour
{
    public const string GlobalTransitionStateName = "GlobalTransition";
    public const string PatrolStateName = "Patrol";
    public const string ChaseStateName = "Chase";
    public const string AttackStateName = "Attack";
    public const string DeathStateName = "Death";
    
    FiniteStateMachine<ZombieFSMData> fsm;
    public ZombieScript Zombie { get; private set; }
    struct ZombieFSMData
    {
        public ZombieStateMachine ZombieFSM { get; private set; }
        public ZombieScript Zombie { get; private set; }

        public ZombieFSMData(ZombieStateMachine zombieFSM, ZombieScript zombie) 
        {
            ZombieFSM = zombieFSM;
            Zombie = zombie; 
        }
    }


    /* ======================================================================================================
    _______________________________________________Zombie State Bases________________________________________
    ====================================================================================================== */
    abstract class ZombieStateBase
    {
        public virtual string Name => "Zombie";

        protected IFiniteStateMachine<ZombieFSMData> ParentFSM;
        protected ZombieStateMachine ZombieFSM;
        protected ZombieScript Zombie;

        public virtual void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            ParentFSM = parentFSM;
            ZombieFSM = zombieFSMData.ZombieFSM;
            Zombie = zombieFSMData.Zombie;
        }

        public virtual void Exit(bool globalTransition) { }
        public virtual void Exit() {Exit(false); }

        public virtual StateTransitionBase<ZombieFSMData> Update()
        {
            return null;
        }

    }

    abstract class ZombieState : ZombieStateBase, IState<ZombieFSMData>
    {
        public virtual void Enter() {}
    }

    abstract class ZombieState<S0> : ZombieStateBase, IState<ZombieFSMData, S0>
    {
        public virtual void Enter(S0 s) {}
    }

    abstract class ZombieState<S0, S1> : ZombieStateBase, IState<ZombieFSMData, S0, S1>
    {
        public virtual void Enter(S0 s0, S1 s1) {}
    }


    /* ======================================================================================================
    _______________________________________________Zombie States_____________________________________________
    ====================================================================================================== */

    class PatrolState : ZombieState
    {
        public override string Name => PatrolStateName;
        // Waypoints
        private int waypointCounter = 0;


        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            if (Zombie.IsChaseRange())
                return ParentFSM.CreateStateTransition(ChaseStateName);

            return null;
        }
    }

    class ChaseState : ZombieState
    {
        public override string Name => ChaseStateName;

        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();

            Zombie.GoToPlayer();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            Zombie.GoToPlayer();

            if (Zombie.IsAttackRange())
                return ParentFSM.CreateStateTransition(AttackStateName);
            if (!Zombie.IsChaseRange())
                return ParentFSM.CreateStateTransition(PatrolStateName);
            return null;
        }
    }

    class AttackState : ZombieState
    {
        public override string Name => AttackStateName;

        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            if (!Zombie.IsAttackRange())
                return ParentFSM.CreateStateTransition(ChaseStateName);
            return null;
        }
    }

    class DeathState : ZombieState
    {
        public override string Name => DeathStateName;

        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();

            // TODO: Spawn Collectable and Enable Ragdoll
            Zombie.Die();
            Zombie.SpawnPickUp();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            return null;
        }
    }

    class GlobalTransitionState : ZombieState
    {
        public override string Name => GlobalTransitionStateName;
        bool wasDead = false;

        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            // Transition to DeathState
            if (Zombie.status.currHealth <= 0 && !wasDead) 
            {
                wasDead = true;
                return ParentFSM.CreateStateTransition(DeathStateName);
            }
            
            return null;
        }
    }


    /* ======================================================================================================
    _______________________________________________FSM_______________________________________________________
    ====================================================================================================== */

    private void Awake()
    {
        Zombie = GetComponent<ZombieScript>();
        if (Zombie == null) 
            Debug.LogError("No Zombie Script");
    }

    protected void Start()
    {
        var zombieFSMData = new ZombieFSMData(this, Zombie);
        fsm = new FiniteStateMachine<ZombieFSMData>(zombieFSMData);

        fsm.SetGlobalTransitionState(new GlobalTransitionState());
        fsm.AddState(new PatrolState(), true);
        fsm.AddState(new ChaseState());
        fsm.AddState(new AttackState());
        fsm.AddState(new DeathState());
    }

    protected void Update()
    {
        fsm.Update();
    }


}
