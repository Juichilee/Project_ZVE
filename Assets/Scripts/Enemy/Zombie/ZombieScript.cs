using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyDamageable), typeof(AISensor))]
public class ZombieScript : EnemyBase, IAttacker, IWeaponHolder
{
    #region Component Reference
    private AudioSource sound;
    public EnemyDamageable EnemyDamageable { get; private set; }
    public AISensor aiSensor;
    public MeleeClawWeapon weapon;
    private PlayerControlScript playerInstance;
    public PlayerVelocityTracker playerVelocityTracker;
    // TODO: Replace this once Enemy Factory is Implemented
    protected EnemiesRemaining enemiesRemaining;
    #endregion
    
    #region Pickup Prefabs 
    public Rigidbody healthPrefab;
    public Rigidbody ammoPrefab;
    public Rigidbody dnaPrefab;
    public Rigidbody currPickup;
    public Rigidbody currPickup2;
    public float extraPickupProb = .4f;
    public float pickupHealthProb = .6f;
    public float pickupAmmoProb = .4f;
    #endregion

    #region Animation Speed Variables
    public float MaxSpeed { get; private set; }
    // Player Body Reference
    private Transform playerBodyTransform;
    private float zombieLookAtSpeed = 2f;
    #endregion

    #region Sound
    private ZombieSounds zombieSounds; // Reference to ZombieSounds
    #endregion

    void Awake()
    {
        // Animator
        anim = GetComponent<Animator>();
        anim.enabled = true;
        anim.applyRootMotion = true;
        
        // RigidBody
        rb = GetComponent<Rigidbody>();

        // Capsule Collider
        cc = GetComponent<CapsuleCollider>();
        cc.enabled = true;
        
        // NavMeshAgent
        aiAgent = GetComponent<NavMeshAgent>();
        aiAgent.updatePosition = false;
        aiAgent.updateRotation = true;
        aiAgent.updateUpAxis = false;

        // Enemy Damageable TODO: Remove Later
        EnemyDamageable = GetComponent<EnemyDamageable>();
        GameObject enemiesRemainingGO = GameObject.Find("EnemiesRemaining");
        if (enemiesRemainingGO)
            enemiesRemaining = enemiesRemainingGO.GetComponent<EnemiesRemaining>();
        
        // AI Sensor
        aiSensor = GetComponent<AISensor>();
        
        // Weapon
        weapon = GetComponentInChildren<MeleeClawWeapon>();
        weapon.WeaponName = "Zombie Hand";
        weapon.WeaponHolder = this;
        weapon.WeaponHolderAnim = anim;
        weapon.transform.localScale = Vector3.one;

        // Sound
        sound = GetComponent<AudioSource>();
        if (sound == null)
        {
            sound = gameObject.AddComponent<AudioSource>();
        }
        zombieSounds = GetComponent<ZombieSounds>();
        if (zombieSounds == null)
        {
            Debug.LogWarning("ZombieSounds component not found on Zombie.");
        }

    }

    protected override void Start() 
    {
        base.Start();
        playerInstance = PlayerControlScript.PlayerInstance;
        playerBodyTransform = playerInstance.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1");
        playerVelocityTracker = playerInstance.GetComponent<PlayerVelocityTracker>();
    }

    void FixedUpdate()
    {
        anim.speed = animationSpeed;

        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up * (radius * 0.5f);
        LayerMask groundLayer = LayerMask.GetMask("ground");
        isGrounded = IsGrounded || Physics.CheckSphere(pos, radius, groundLayer);
        anim.SetBool("isFalling", !isGrounded);

        MaxSpeed = aiAgent.velocity.magnitude / aiAgent.speed;
    }

    #region Sounds
    // This method is called by the animation event 'ZombieWalk'
    public void ZombieWalk()
    {
        if (zombieSounds != null && !sound.isPlaying)
        {
            zombieSounds.ZombieWalk();
        }
    }

    public void ZombieAttacking()
    {
        if (zombieSounds != null && !sound.isPlaying)
        {
            zombieSounds.ZombieAttack();
        }
    }

    public void ZombieNear()
    {
        if (zombieSounds != null && !sound.isPlaying)
        {
            zombieSounds.ZombieCloseToPlayer();
        }
    }

    public void AlertPlayerSpotted()
    {
        if (zombieSounds != null)
        {
            zombieSounds.ZombieAlert();
        }
    }

    public void PlayIdleSound()
    {
        if (zombieSounds != null)
        {
            zombieSounds.ZombieIdle();
        }
    }
    #endregion

    #region Movement
    public float maxLookAheadTime = 0.25f;
    public float MovementPredictionThreshold = 0.25f;

    public override bool GoTo(Vector3 position, float speed = 0)
    {
        anim.SetFloat("vely", speed, 0.3f, Time.deltaTime);

        return base.GoTo(position, speed);
    }

    // public void LookAtPlayer()
    // {
    //     Vector3 playerLookDir = (playerInstance.transform.position - transform.position).normalized;
    //     this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(playerLookDir), Time.deltaTime * zombieLookAtSpeed);
    // }

    public bool GoToPlayer()
    {

        Vector3 currPos = this.transform.position;
        Vector3 playerPos = playerInstance.transform.position;

        float distance = Vector3.Distance(currPos, playerPos);

        float speed = aiAgent.speed;
        float lookAheadTime = Mathf.Clamp(distance / speed, 0 , maxLookAheadTime);

        Vector3 velocity = playerVelocityTracker.AverageVelocity;
        Vector3 predictedPosition = playerPos + velocity * lookAheadTime;

        Vector3 directionToPrediction = (predictedPosition - this.transform.position).normalized;
        Vector3 directionToPlayer = (playerInstance.transform.position - this.transform.position).normalized;

        float dot = Vector3.Dot(directionToPrediction, directionToPlayer);

        if (dot < MovementPredictionThreshold)
            predictedPosition = playerInstance.transform.position;

        if (NavMesh.Raycast(playerPos, predictedPosition, out NavMeshHit hit, NavMesh.AllAreas))
            predictedPosition = hit.position;
        return GoTo(predictedPosition, MaxSpeed);
    }
    #endregion

    #region Death
    public override void Die() 
    {
        base.Die();
        aiSensor.enabled = false;
        DisableHitbox();
        if (enemiesRemaining)
            enemiesRemaining.oneEnemyDefeated();
    }

    public override void SpawnPickUp()
    {
        float random = Random.value;

        if (random <= extraPickupProb)
        {
            random = Random.value;

            if (random <= pickupHealthProb)
            {
                currPickup = Instantiate(healthPrefab, transform);
                currPickup.transform.localPosition = new Vector3(-.25f, 1f, -.25f);
                currPickup.isKinematic = true;
            }
            else if (random > pickupHealthProb && random <= pickupHealthProb + pickupAmmoProb)
            {
                currPickup = Instantiate(ammoPrefab, transform);
                currPickup.transform.localPosition = new Vector3(-.25f, 1f, -.25f);
                currPickup.isKinematic = true;
            }

        }
        currPickup2 = Instantiate(dnaPrefab, transform);
        currPickup2.transform.localPosition = new Vector3(.25f, 1f, .25f);
        currPickup2.isKinematic = true;
    } 
    #endregion
    
    #region Attack
    private float attackRange = 1f;

    public bool IsInAttackRange()
    {
        Vector3 playerPosition = PlayerControlScript.PlayerInstance.transform.position;
        return Vector3.Distance(this.transform.position, playerPosition) <= attackRange;
    }

    public bool IsInSight()
    {
        return aiSensor.IsInSight(PlayerControlScript.PlayerInstance.gameObject);
    }

    public bool IsInChaseRange()
    {
        return aiSensor.IsInChase(PlayerControlScript.PlayerInstance.gameObject);
    }

    public bool IsInHearRange()
    {
        return aiSensor.IsInHear(PlayerControlScript.PlayerInstance.gameObject);
    }

    public bool TookDamageRecently()
    {
        return EnemyDamageable.TookDamageRecently;
    }

    public void AttackTarget()
    {
        anim.SetTrigger("attack1");
        weapon.Attack();
        ZombieNear();
    }


    public void EnableHitbox()
    {
        weapon.EnableHitbox();
    }

    public void DisableHitbox()
    {
        weapon.DisableHitbox();
    }
    #endregion

    #region WeaponHolder
    public Transform GetWeaponHolderRootTransform()
    {
        return this.transform.root;
    }
    #endregion

    private void OnAnimatorIK(int layerIndex)
    {
        if(anim) 
        {
            AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(layerIndex);
            if(astate.IsName("Attack"))
            {
                float aimWeight = 0.1f;

                // Set the look target position, if one has been assigned
                if(playerBodyTransform != null)
                {
                    anim.SetLookAtWeight(aimWeight);
                    anim.SetLookAtPosition(playerBodyTransform.position);
                    anim.SetIKPositionWeight(AvatarIKGoal.RightHand, aimWeight);
                    anim.SetIKPosition(AvatarIKGoal.RightHand, playerBodyTransform.position);
                }
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetLookAtWeight(0);

            }
        }
    } 

}
