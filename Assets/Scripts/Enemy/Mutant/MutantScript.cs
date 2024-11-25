using System.Collections;
using System.Linq.Expressions;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyDamageable), typeof(AISensor))]
public class MutantScript : EnemyBase, IAttacker, IWeaponHolder
{
    #region Component Reference
    private AudioSource audioSource;
    public EnemyDamageable EnemyDamageable { get; private set; }
    public AISensor aiSensor;
    public MeleeClawWeapon weapon;
    private PlayerControlScript playerInstance;
    public PlayerVelocityTracker playerVelocityTracker;
    protected EnemiesRemaining enemiesRemaining;
    #endregion
    
    #region Pickup Prefabs 
    public Rigidbody healthPrefab;
    public Rigidbody ammoPrefab;
    public Rigidbody dnaPrefab;
    public Rigidbody currPickup;
    public Rigidbody currPickup2;
    public float pickupHealthProb = .5f;
    public float pickupAmmoProb = .5f;
    #endregion

    #region Animation Speed Variables
    private Vector2 smoothDeltaPosition = Vector2.zero;
    private Vector2 velocity = Vector2.zero;
    public float MaxSpeed { get; private set; }
    // Player Body Reference
    private Transform playerBodyTransform;
    private float mutantLookAtSpeed = 2f;
    private bool canCharge = true;
    private int chargeMinCooldown = 10;
    private int chargeMaxCooldown = 20;
    private float chargeSpeed = 1.5f;
    #endregion

    #region Sound
    public AudioClip footstepClip;
    public AudioClip attackSound;
    public AudioClip roarSound;
    public AudioClip idleSound;
    #endregion

    // Start is called before the first frame update
    void Awake()
    {
        // Animator
        anim = GetComponent<Animator>();
        anim.enabled = true;
        anim.applyRootMotion = true;

        // Sound
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

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
        weapon = GetComponentInChildren<MeleeClawWeapon>();
        weapon.WeaponName = "Zombie Hand";
        weapon.WeaponHolder = this;
        weapon.WeaponHolderAnim = anim;
        weapon.transform.localScale = Vector3.one;
    }

    protected override void Start()
    {
        base.Start();
        attackRange = 2f;
        playerInstance = PlayerControlScript.PlayerInstance;
        playerBodyTransform = playerInstance.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1");
        playerVelocityTracker = playerInstance.GetComponent<PlayerVelocityTracker>();
    }

    void Update() 
    {
        Vector3 worldDeltaPosition = aiAgent.nextPosition - this.transform.position;

        float dx = Vector3.Dot (this.transform.right, worldDeltaPosition);
        float dy = Vector3.Dot (this.transform.forward, worldDeltaPosition);
        Vector2 deltaPosition = new Vector2 (dx, dy);
        float smooth = Mathf.Min(1.0f, Time.deltaTime/0.15f);
        smoothDeltaPosition = Vector2.Lerp (smoothDeltaPosition, deltaPosition, smooth);

        // Update velocity if time advances
        if (Time.deltaTime > 1e-5f)
            velocity = smoothDeltaPosition / Time.deltaTime;
    }

    void FixedUpdate()
    {
        if (inCharge)
        {
            anim.speed = chargeSpeed;
        } else {
            anim.speed = animationSpeed;
        }

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
        if (footstepClip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(footstepClip); //temp drop to half volume b/c everything's loud
        }
    }

    public void ZombieAttack()
    {
        if (attackSound != null)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void ZombieRoar()
    {
        if (roarSound != null)
        {
            audioSource.PlayOneShot(roarSound);
        }
    }

    public void MutantIdleBreath()
    {
        if (roarSound != null)
        {
            audioSource.PlayOneShot(idleSound);
        }
    }

    #endregion
    #region Movement
    public float maxLookAheadTime = 0.5f;
    public float MovementPredictionThreshold = 0.25f;

    public override bool GoTo(Vector3 position, float speed = 0)
    {
        // anim.SetFloat("velx", velocity.x * speed);
        anim.SetFloat("vely", velocity.y * speed, 0.3f, Time.deltaTime);

        return base.GoTo(position, speed);
    }

    public void LookAtPlayer()
    {
        Vector3 playerLookDir = (playerInstance.transform.position - transform.position).normalized;
        this.transform.rotation = Quaternion.Slerp(this.transform.rotation, Quaternion.LookRotation(playerLookDir), Time.deltaTime * mutantLookAtSpeed);
    }

    public bool GoToPlayer(float speedModifier = 1)
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
        return GoTo(predictedPosition, MaxSpeed * speedModifier);
    }

    bool inCharge = false;
    public bool ChargeToPlayer()
    {
        inCharge = true;
        return GoToPlayer(chargeSpeed);
    }

    public void ChargeReset()
    {
        inCharge = false;
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
        currPickup2 = Instantiate(dnaPrefab, transform);
        currPickup2.transform.localPosition = new Vector3(.25f, 1f, .25f);
        currPickup2.isKinematic = true;
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

    public bool CanCharge()
    {
        return canCharge;
    }

    Coroutine chargeCoroutine = null;
    IEnumerator ChargeCoolDownCoroutine()
    {
        canCharge = false;
        int coolDown = Random.Range(chargeMinCooldown, chargeMaxCooldown);
        yield return new WaitForSeconds(coolDown);
        canCharge = true;
        chargeCoroutine = null;
    }
    
    public void StartChargeCooldown()
    {
        if (chargeCoroutine == null)
        {
            chargeCoroutine = StartCoroutine(ChargeCoolDownCoroutine());
        }
    }

    public void AttackTarget()
    {
        anim.SetTrigger("attack1");
        weapon.Attack();
    }

    float aimVelocity = 0.0f;

    public void EnableHitbox()
    {
        aimWeight = Mathf.SmoothDamp(aimWeight, 1.0f, ref aimVelocity, 0.3f);
        weapon.EnableHitbox();
    }

    public void DisableHitbox()
    {
        aimWeight = Mathf.SmoothDamp(aimWeight, 0.0f, ref aimVelocity, 0.1f);
        weapon.DisableHitbox();
    }
    #endregion

    #region WeaponHolder
    public Transform GetWeaponHolderRootTransform()
    {
        return this.transform.root;
    }
    #endregion

    float aimWeight = 0f;

    private void OnAnimatorIK(int layerIndex)
    {
        if(anim) 
        {
            AnimatorStateInfo astate = anim.GetCurrentAnimatorStateInfo(layerIndex);
            if(astate.IsName("Attack"))
            {
                // Set the look target position, if one has been assigned
                if(playerBodyTransform != null)
                {
                    anim.SetLookAtWeight(aimWeight);
                    anim.SetLookAtPosition(playerBodyTransform.position);
                    anim.SetIKPositionWeight(AvatarIKGoal.LeftHand, aimWeight);
                    anim.SetIKPosition(AvatarIKGoal.LeftHand, playerBodyTransform.position);
                }
            }
            else
            {
                anim.SetIKPositionWeight(AvatarIKGoal.RightHand, 0);
                anim.SetLookAtWeight(0);

            }
        }
    } 

    public void Scream() 
    {
        anim.SetTrigger("scream");
    }



}
