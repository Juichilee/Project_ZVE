using System.Collections.Generic;
using GameAI;
#if UNITY__EDITOR
using UnityEditor.Build;
#endif
using UnityEngine;
using UnityEngine.AI;

public class SoldierStateMachine : MonoBehaviour
{
    #region Constant State Strings
    public const string GlobalTransitionStateName = "GlobalTransition";
    public const string PatrolStateName = "Patrol";
    public const string ChaseStateName = "Chase";
    public const string AttackStateName = "Attack";
    public const string DeathStateName = "Death";
    #endregion
    
    [SerializeField]
    FiniteStateMachine<SoldierFSMData> fsm;
    public SoldierScript Soldier { get; private set; }
    struct SoldierFSMData
    {
        public SoldierStateMachine SoldierFSM { get; private set; }
        public SoldierScript Soldier { get; private set; }

        public SoldierFSMData(SoldierStateMachine soldierFSM, SoldierScript soldier) 
        {
            SoldierFSM = soldierFSM;
            Soldier = soldier; 
        }
    }


    #region Soldier Base States
    abstract class SoldierStateBase
    {
        public virtual string Name => "Soldier";

        protected IFiniteStateMachine<SoldierFSMData> ParentFSM;
        protected SoldierStateMachine SoldierFSM;
        protected SoldierScript Soldier;

        public virtual void Init(IFiniteStateMachine<SoldierFSMData> parentFSM, SoldierFSMData soldierFSMData)
        {
            ParentFSM = parentFSM;
            SoldierFSM = soldierFSMData.SoldierFSM;
            Soldier = soldierFSMData.Soldier;
        }

        public virtual void Exit(bool globalTransition) { }
        public virtual void Exit() {Exit(false); }

        public virtual StateTransitionBase<SoldierFSMData> Update()
        {
            return null;
        }

    }

    abstract class SoldierState : SoldierStateBase, IState<SoldierFSMData>
    {
        public virtual void Enter() {}
    }

    abstract class SoldierState<S0> : SoldierStateBase, IState<SoldierFSMData, S0>
    {
        public virtual void Enter(S0 s) {}
    }

    abstract class SoldierState<S0, S1> : SoldierStateBase, IState<SoldierFSMData, S0, S1>
    {
        public virtual void Enter(S0 s0, S1 s1) {}
    }
    #endregion

    /* ======================================================================================================
    ______________________________________________.Soldier States.____________________________________________
    ====================================================================================================== */

    #region Soldier States 
    class PatrolState : SoldierState
    {
        public override string Name => PatrolStateName;
        private List<Vector3> waypoints;
        private int currWaypointIndex = 0;
        private Vector3 currWaypointPosition;
        private int numWaypoints = 1;
        private float patrolRange = 2f; 

        public override void Init(IFiniteStateMachine<SoldierFSMData> parentFSM, SoldierFSMData soldierFSMData)
        {
            base.Init(parentFSM, soldierFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            currWaypointPosition = Soldier.gameObject.transform.position;
            Soldier.SetShootingStoppingDistance(false);
            CreateWaypoints();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<SoldierFSMData> Update()
        {

            if (Soldier.IsInSight() || Soldier.IsInHearRange() || Soldier.TookDamageRecently() && Soldier.IsInChaseRange())
            {
                Soldier.PlayAlert();
                return ParentFSM.CreateStateTransition(ChaseStateName);
            }
            if (Soldier.ReachedTarget())
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
                randomDirection += currWaypointPosition;

                if (NavMesh.SamplePosition(randomDirection, out NavMeshHit hit, patrolRange, NavMesh.AllAreas))
                {
                    waypoints.Add(hit.position);
                    currWaypointPosition = hit.position;
                } else {
                    waypoints.Add(currWaypointPosition);
                }
            }
        }

        private void GoToWaypoint()
        {
            if (waypoints.Count > 0)
                Soldier.GoTo(waypoints[currWaypointIndex], Soldier.MaxSpeed * 2 / 3);
                if (Soldier.ReachedTarget())
                {
                    CreateWaypoints();
                }
        }
    }

    class ChaseState : SoldierState
    {
        public override string Name => ChaseStateName;

        public override void Init(IFiniteStateMachine<SoldierFSMData> parentFSM, SoldierFSMData soldierFSMData)
        {
            base.Init(parentFSM, soldierFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            Soldier.SetShootingStoppingDistance(true);
            Soldier.ResetAttack();
            Soldier.PlayAlert();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<SoldierFSMData> Update()
        {
            Soldier.GoToPlayer();
            Soldier.LookAtPlayer();

            if (Soldier.IsInSight() && Soldier.IsInAttackRange())
            {
                return ParentFSM.CreateStateTransition(AttackStateName);
            }
            if (!Soldier.IsInSight() && !Soldier.IsInChaseRange())
                return ParentFSM.CreateStateTransition(PatrolStateName);
            return null;
        }
    }

    class AttackState : SoldierState
    {
        public override string Name => AttackStateName;


        public override void Init(IFiniteStateMachine<SoldierFSMData> parentFSM, SoldierFSMData soldierFSMData)
        {
            base.Init(parentFSM, soldierFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            Soldier.PlayAttack();
            
            Soldier.GoToPlayer();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<SoldierFSMData> Update()
        {

            Soldier.GoToPlayer();

            if (!Soldier.IsInAttackRange())
            {
                Soldier.OutOfRange();
                return ParentFSM.CreateStateTransition(ChaseStateName);
            }

            Soldier.LookAtPlayer();

            Soldier.AttackTarget();

            return null;
        }
    }

    class DeathState : SoldierState
    {
        public override string Name => DeathStateName;

        public override void Init(IFiniteStateMachine<SoldierFSMData> parentFSM, SoldierFSMData soldierFSMData)
        {
            base.Init(parentFSM, soldierFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            Soldier.Die();
            Soldier.SpawnPickUp();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<SoldierFSMData> Update()
        {
            return null;
        }
    }

    class GlobalTransitionState : SoldierState
    {
        public override string Name => GlobalTransitionStateName;
        bool wasDead = false;

        public override void Init(IFiniteStateMachine<SoldierFSMData> parentFSM, SoldierFSMData soldierFSMData)
        {
            base.Init(parentFSM, soldierFSMData);
        }

        public override void Enter()
        {
            base.Enter();
        }

        public override StateTransitionBase<SoldierFSMData> Update()
        {
            // Transition to DeathState
            if (!Soldier.EnemyDamageable.IsAlive && !wasDead) 
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
        Soldier = GetComponent<SoldierScript>();
        if (Soldier == null) 
            Debug.LogError("No Soldier Script");
    }

    protected void Start()
    {
        var soldierFSMData = new SoldierFSMData(this, Soldier);
        fsm = new FiniteStateMachine<SoldierFSMData>(soldierFSMData);

        fsm.SetGlobalTransitionState(new GlobalTransitionState());
        fsm.AddState(new PatrolState(), true);
        fsm.AddState(new ChaseState());
        fsm.AddState(new AttackState());
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
