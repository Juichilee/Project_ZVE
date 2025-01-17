using System.Collections.Generic;
using GameAI;
#if UNITY__EDITOR
using UnityEditor.Build;
#endif
using UnityEngine;
using UnityEngine.AI;

public class ZombieStateMachine : MonoBehaviour
{
    #region Constant State Strings
    public const string GlobalTransitionStateName = "GlobalTransition";
    public const string PatrolStateName = "Patrol";
    public const string ChaseStateName = "Chase";
    public const string AttackStateName = "Attack";
    public const string DeathStateName = "Death";
    #endregion
    
    [SerializeField]
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


    #region Zombie Base States
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
    #endregion

    /* ======================================================================================================
    ______________________________________________.Zombie States.____________________________________________
    ====================================================================================================== */

    #region Zombie States 
    class PatrolState : ZombieState
    {
        public override string Name => PatrolStateName;
        private List<Vector3> waypoints;
        private int currWaypointIndex = 0;
        private Vector3 currWaypointPosition;
        private int numWaypoints = 1;
        private float patrolRange = 2f;

        private float idleSoundTimer;
        private float idleSoundInterval = 8f;

        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            currWaypointPosition = Zombie.gameObject.transform.position;
            CreateWaypoints();
            idleSoundTimer = 0f;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            idleSoundTimer += Time.deltaTime;

            if (idleSoundTimer >= idleSoundInterval)
            {
                Zombie.PlayIdleSound();
                idleSoundTimer = 0f;
            }

            if (Zombie.IsInSight() || Zombie.IsInHearRange() || Zombie.TookDamageRecently() && Zombie.IsInChaseRange())
                return ParentFSM.CreateStateTransition(ChaseStateName);

            if (Zombie.ReachedTarget())
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
                Zombie.GoTo(waypoints[currWaypointIndex], Zombie.MaxSpeed * 2 / 3);
                if (Zombie.ReachedTarget())
                {
                    CreateWaypoints();
                }
        }
    }

    class ChaseState : ZombieState
    {
        private bool hasPlayedAlertSound = false;
        public override string Name => ChaseStateName;

        public override void Init(IFiniteStateMachine<ZombieFSMData> parentFSM, ZombieFSMData zombieFSMData)
        {
            base.Init(parentFSM, zombieFSMData);
        }

        public override void Enter()
        {
            base.Enter();
            hasPlayedAlertSound = false;
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            if (!hasPlayedAlertSound)
            {
                Zombie.AlertPlayerSpotted(); // Play alert sound
                hasPlayedAlertSound = true;
            }

            // Zombie.LookAtPlayer();
            Zombie.GoToPlayer();

            if (Zombie.IsInSight() && Zombie.IsInAttackRange())
            {
                return ParentFSM.CreateStateTransition(AttackStateName);
            }
            if (!Zombie.IsInSight() && !Zombie.IsInChaseRange())
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
            
            Zombie.GoToPlayer();
        }

        public override void Exit()
        {
            base.Exit();
        }

        public override StateTransitionBase<ZombieFSMData> Update()
        {
            Zombie.GoToPlayer();

            if (!Zombie.IsInAttackRange())
            {
                return ParentFSM.CreateStateTransition(ChaseStateName);
            }

            // Zombie.LookAtPlayer();
            Zombie.AttackTarget();

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
            if (!Zombie.EnemyDamageable.IsAlive && !wasDead) 
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


    public string debugState;
    
    protected void Update()
    {
        fsm.Update();
        debugState = fsm.CurrentState.Name;
    }
    #endregion
}
