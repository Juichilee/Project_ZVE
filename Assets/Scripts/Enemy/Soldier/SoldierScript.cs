using Unity.VisualScripting;
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
    private PlayerControlScript playerInstance;
    #endregion
    
    #region Pickup Prefabs 
    public Rigidbody healthPrefab;
    public Rigidbody ammoPrefab;
    public Rigidbody weaponPrefab;
    public Rigidbody currPickup;
    public Rigidbody currWeapon;
    public float pickupHealthProb = .5f;
    public float pickupAmmoProb = .5f;
    #endregion

    #region Animation Speed Variables
    public float MaxSpeed = 1;
    private Transform playerBodyTransform;
    private float soldierLookAtSpeed = 3f;
    #endregion

    #region Sound
    public AudioClip footstepClip;
    public AudioClip attackSound;
    #endregion

    #region Attack Variables
    private int attackRange = 15;
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

        // Enemy Damageable TODO: Remove Later
        EnemyDamageable = GetComponent<EnemyDamageable>();
        GameObject enemiesRemainingGO = GameObject.Find("EnemiesRemaining");
        if (enemiesRemainingGO)
            enemiesRemaining = enemiesRemainingGO.GetComponent<EnemiesRemaining>();
        
        // AI Sensor
        aiSensor = GetComponent<AISensor>();
        
        // Weapon
        weapon = GetComponentInChildren<RangedWeapon>();
        weapon.WeaponName = "Soldier AR";
        weapon.WeaponHolder = this;
        weapon.WeaponHolderAnim = anim;
        weapon.transform.SetLocalPositionAndRotation(weapon.Hold.localPosition, weapon.Hold.localRotation);
        weapon.transform.localScale = Vector3.one;

        // Sound
    }

    protected override void Start() 
    {
        base.Start();
        playerInstance = PlayerControlScript.PlayerInstance;
        playerBodyTransform = playerInstance.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1");
    }

    void FixedUpdate()
    {
        anim.speed = animationSpeed;

        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up * (radius * 0.5f);
        LayerMask groundLayer = LayerMask.GetMask("ground");
        isGrounded = IsGrounded || Physics.CheckSphere(pos, radius, groundLayer);
        anim.SetBool("isFalling", !isGrounded);

        // MaxSpeed = aiAgent.velocity.magnitude / aiAgent.speed;
    }

    #region Movement
    public float maxLookAheadTime = 0.5f;

    public override bool GoTo(Vector3 position, float speed = 0)
    {
        anim.SetFloat("vely", speed, 0.3f, Time.deltaTime);

        return base.GoTo(position, speed);
    }

    public void LookAtPlayer()
    {
        Vector3 playerLookDir = (playerInstance.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(playerLookDir), Time.deltaTime * soldierLookAtSpeed);
    }

    public bool GoToPlayer()
    {
        PlayerControlScript player = playerInstance;

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
        weapon.gameObject.SetActive(false);
        if (enemiesRemaining)
            enemiesRemaining.oneEnemyDefeated();
    }

    public override void SpawnPickUp()
    {
        float random = Random.value;
        Debug.Log(random);

        // if (weapon)
        // {
        //     currWeapon = Instantiate(weaponPrefab, transform);
        //     currWeapon.transform.localPosition = new Vector3(.25f, 1f, .25f);
        //     currWeapon.isKinematic = true;
        // }

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
    public bool IsInAttackRange()
    {
        Vector3 playerPosition = playerInstance.transform.position;
        bool inAttackRange = Vector3.Distance(this.transform.position, playerPosition) <= attackRange;
        if (!inAttackRange)
        {
            anim.ResetTrigger("attack");
        }
        return inAttackRange;
    }

    public bool IsInSight()
    {
        return aiSensor.IsInSight(playerInstance.gameObject);
    }

    public void AttackTarget()
    {
        if (weapon.CurrentClip == 0)
            weapon.Reload();

        anim.ResetTrigger("attack");
        anim.SetTrigger("attack");
    }

    public void FireWeapon()
    {
        Transform enemyTarget = playerInstance.enemyTarget;
        if (!enemyTarget)
        {
            enemyTarget = playerInstance.gameObject.transform;
        }

        weapon.UpdateWeaponAim(enemyTarget);
        weapon.Attack();
    }

    #endregion

    #region WeaponHolder
    public Transform GetWeaponHolderRootTransform()
    {
        return this.transform.root;
    }
    #endregion

}
