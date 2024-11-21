using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyDamageable), typeof(AISensor))]
public class SoldierScript : EnemyBase, IAttacker, IWeaponHolder
{
    #region Component Reference
    public EnemyDamageable EnemyDamageable { get; private set; }
    public AISensor aiSensor;
    public RangedWeapon weapon;
    // TODO: Replace this once Enemy Factory is Implemented
    protected EnemiesRemaining enemiesRemaining;
    #endregion
    
    #region Pickup Prefabs 
    public Rigidbody healthPrefab;
    public Rigidbody ammoPrefab;
    public Rigidbody currPickup;
    public float pickupHealthProb = .5f;
    public float pickupAmmoProb = .5f;
    #endregion

    #region Animation Speed Variables
    public float MaxSpeed;
    // Player Body Reference
    private Transform playerBodyTransform;
    #endregion

    #region Sound
    public AudioClip footstepClip;
    public AudioClip attackSound;
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
        aiAgent.updatePosition = true;

        // Enemy Damageable TODO: Remove Later
        EnemyDamageable = GetComponent<EnemyDamageable>();
        GameObject enemiesRemainingGO = GameObject.Find("EnemiesRemaining");
        if (enemiesRemainingGO)
            enemiesRemaining = enemiesRemainingGO.GetComponent<EnemiesRemaining>();
        
        // AI Sensor
        aiSensor = GetComponent<AISensor>();
        
        // Weapon
        // weapon = GetComponentInChildren<RangedWeapon>();
        // weapon.WeaponName = "Zombie Hand";
        // weapon.WeaponHolder = this;
        // weapon.WeaponHolderAnim = anim;
        // weapon.transform.SetLocalPositionAndRotation(weapon.Hold.localPosition, weapon.Hold.localRotation);
        // weapon.transform.localScale = Vector3.one;

        // Sound
    }

    protected override void Start() 
    {
        base.Start();
        attackRange = 20f;
        PlayerControlScript player = PlayerControlScript.PlayerInstance;
        playerBodyTransform = player.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1");
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

    #region Movement
    public float maxLookAheadTime = 0.5f;

    public override bool GoTo(Vector3 position, float speed = 0)
    {
        anim.SetFloat("vely", speed, 0.3f, Time.deltaTime);

        return base.GoTo(position, speed);
    }

    public bool GoToPlayer()
    {
        PlayerControlScript player = PlayerControlScript.PlayerInstance;

        Vector3 currPos = this.transform.position;
        Vector3 playerPos = player.transform.position;

        float distance = Vector3.Distance(currPos, playerPos);

        if (distance > attackRange)
        {
            float speed = aiAgent.speed;
            float lookAheadTime = Mathf.Clamp(distance / speed, 0 , maxLookAheadTime);

            Vector3 velocity = player.GetComponent<PlayerControlScript>().WorldVelocity;
            Vector3 predictedPosition = playerPos + velocity * lookAheadTime;
            
            if (NavMesh.Raycast(playerPos, predictedPosition, out NavMeshHit hit, NavMesh.AllAreas))
                predictedPosition = hit.position;
            return GoTo(predictedPosition, MaxSpeed);
        }
        return GoTo(playerPos, MaxSpeed);
    }
    #endregion

    #region Death
    public override void Die() 
    {
        base.Die();
        aiSensor.enabled = false;
        if (enemiesRemaining)
            enemiesRemaining.oneEnemyDefeated();
    }

    public override void SpawnPickUp()
    {
        float random = Random.value;
        Debug.Log(random);
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
    #endregion
    
    #region Attack
    private float attackRange;

    public bool IsInAttackRange()
    {
        Vector3 playerPosition = PlayerControlScript.PlayerInstance.transform.position;
        return Vector3.Distance(this.transform.position, playerPosition) <= attackRange;
    }

    public bool IsInSight()
    {
        return aiSensor.IsInSight(PlayerControlScript.PlayerInstance.gameObject);
    }

    public void AttackTarget()
    {
        // TODO: Talk to Juichi on how to shoot the gun
        anim.SetTrigger("attack1");
        // weapon.Attack();
    }

    #endregion

    #region WeaponHolder
    public Transform GetWeaponHolderRootTransform()
    {
        return this.transform.root;
    }
    #endregion
}
