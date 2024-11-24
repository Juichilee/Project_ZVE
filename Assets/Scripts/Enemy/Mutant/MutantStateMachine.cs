using System.Collections.Generic;
using GameAI;
#if UNITY__EDITOR
using UnityEditor.Build;
#endif
using UnityEngine;
using UnityEngine.AI;

public class MutantStateMachine : MonoBehaviour
{
    #region Constant State Strings
    public const string GlobalTransitionStateName = "GlobalTransition";
    public const string PatrolStateName = "Patrol";
    public const string ChaseStateName = "Chase";
    public const string AttackStateName = "Attack";
    public const string ChargeStateName = "Charge";
    public const string DeathStateName = "Death";
    #endregion
    
    [SerializeField]
    FiniteStateMachine<MutantFSMData> fsm;
    public MutantScript Mutant { get; private set; }
    struct MutantFSMData
    {
        public MutantStateMachine MutantFSM { get; private set; }
        public MutantScript Mutant { get; private set; }

        public MutantFSMData(MutantStateMachine mutantFSM, MutantScript mutant) 
        {
            MutantFSM = mutantFSM;
            Mutant = mutant; 
        }
    }


    #region Mutant Base States
    abstract class MutantStateBase
    {
        public virtual string Name => "Mutant";

        protected IFiniteStateMachine<MutantFSMData> ParentFSM;
        protected MutantStateMachine MutantFSM;
        protected MutantScript Mutant;

        public virtual void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            ParentFSM = parentFSM;
            MutantFSM = mutantFSMData.MutantFSM;
            Mutant = mutantFSMData.Mutant;
        }

        public virtual void Exit(bool globalTransition) { }
        public virtual void Exit() {Exit(false); }

        public virtual StateTransitionBase<MutantFSMData> Update()
        {
            return null;
        }

    }

    abstract class MutantState : MutantStateBase, IState<MutantFSMData>
    {
        public virtual void Enter() {}
    }

    abstract class MutantState<S0> : MutantStateBase, IState<MutantFSMData, S0>
    {
        public virtual void Enter(S0 s) {}
    }

    abstract class MutantState<S0, S1> : MutantStateBase, IState<MutantFSMData, S0, S1>
    {
        public virtual void Enter(S0 s0, S1 s1) {}
    }
    #endregion

    /* ======================================================================================================
    ______________________________________________.Mutant States.____________________________________________
    ====================================================================================================== */

    #region Mutant States 
    class PatrolState : MutantState
    {
        public override string Name => PatrolStateName;
        private List<Vector3> waypoints;
        private int currWaypointIndex = 0;
        private int numWaypoints = 3;
        private float patrolRange = 20f; 

        public override void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            base.Init(parentFSM, mutantFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            CreateWaypoints();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<MutantFSMData> Update()
        {
            if (Mutant.IsInSight())
            {
                float random = Random.value;
                if (random <= 0.5)
                    return ParentFSM.CreateStateTransition(ChaseStateName);
                return ParentFSM.CreateStateTransition(ChargeStateName);
            }

            if (Mutant.ReachedTarget())
                currWaypointIndex = (currWaypointIndex + 1) % waypoints.Count;
            GoToWaypoint();   
            return null;
        }

        private void CreateWaypoints()
        {
            waypoints = new List<Vector3>();
            for (int i = 0; i < numWaypoints; i++)
            {
                Vector3 randomDirection = Random.insideUnitSphere * patrolRange; 
                randomDirection += Mutant.transform.position;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
                    waypoints.Add(hit.position);
            } 

        }

        private void GoToWaypoint()
        {
            if (waypoints.Count > 0)
                Mutant.GoTo(waypoints[currWaypointIndex], Mutant.MaxSpeed * 1 / 3);
        }
    }

    class ChaseState : MutantState
    {
        public override string Name => ChaseStateName;

        public override void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            base.Init(parentFSM, mutantFSMData);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<MutantFSMData> Update()
        {
            Mutant.GoToPlayer();

            if (Mutant.IsInAttackRange())
            {
                return ParentFSM.CreateStateTransition(AttackStateName);
            }
            if (!Mutant.IsInSight())
                return ParentFSM.CreateStateTransition(PatrolStateName);
            return null;
        }
    }

    class AttackState : MutantState
    {
        public override string Name => AttackStateName;


        public override void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            base.Init(parentFSM, mutantFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            
            Mutant.GoToPlayer();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<MutantFSMData> Update()
        {
            Mutant.GoToPlayer();

            if (!Mutant.IsInAttackRange())
            {
                return ParentFSM.CreateStateTransition(ChaseStateName);
            }

            Mutant.AttackTarget();

            return null;
        }
    }

    class ChargeState : MutantState
    {
        public override string Name => ChargeStateName;


        public override void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            base.Init(parentFSM, mutantFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            
            Mutant.Scream();
            Mutant.GoToPlayer();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<MutantFSMData> Update()
        {
            Mutant.ChargeToPlayer();
            if (Mutant.IsInAttackRange())
            {
                return ParentFSM.CreateStateTransition(AttackStateName);
            }

            return null;
        }
    }
    class DeathState : MutantState
    {
        public override string Name => DeathStateName;

        public override void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            base.Init(parentFSM, mutantFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            Mutant.Die();
            Mutant.SpawnPickUp();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<MutantFSMData> Update()
        {
            return null;
        }
    }

    class GlobalTransitionState : MutantState
    {
        public override string Name => GlobalTransitionStateName;
        bool wasDead = false;

        public override void Init(IFiniteStateMachine<MutantFSMData> parentFSM, MutantFSMData mutantFSMData)
        {
            base.Init(parentFSM, mutantFSMData);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override StateTransitionBase<MutantFSMData> Update()
        {
            // Transition to DeathState
            if (!Mutant.EnemyDamageable.IsAlive && !wasDead) 
            {
                wasDead = true;
                return ParentFSM.CreateStateTransition(DeathStateName);
            }
            
            return null;
        }
    }
    #endregion

    #region FSM
    private void Awake()
    {
        Mutant = GetComponent<MutantScript>();
        if (Mutant == null) 
            Debug.LogError("No Mutant Script");
    }

    protected void Start()
    {
        var mutantFSMData = new MutantFSMData(this, Mutant);
        fsm = new FiniteStateMachine<MutantFSMData>(mutantFSMData);

        fsm.SetGlobalTransitionState(new GlobalTransitionState());
        fsm.AddState(new PatrolState(), true);
        fsm.AddState(new ChaseState());
        fsm.AddState(new AttackState());
        fsm.AddState(new ChargeState());
        fsm.AddState(new DeathState());
    }


    public string debugState;
    
    protected void Update()
    {
        fsm.Update();
        debugState = fsm.CurrentState.Name;
    }
    #endregion
}
