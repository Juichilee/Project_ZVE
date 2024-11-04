using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(EnemyDamageable), typeof(AISensor))]
public class ScientistScript : EnemyBase
{
    #region Component Reference
    public EnemyDamageable EnemyDamageable { get; private set; }
    public AISensor aiSensor;
    public MeleeClawWeapon weapon;
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
    public float MaxSpeed { get; private set; }
    public float fleeDisplacementDist;
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
        aiAgent.updatePosition = false;
        aiAgent.updateRotation = true;

        // AI Sensor
        aiSensor = GetComponent<AISensor>();
    }

    protected override void Start() 
    {
        base.Start();
        PlayerControlScript player = PlayerControlScript.PlayerInstance;
        playerBodyTransform = player.transform.Find("mixamorig:Hips/mixamorig:Spine/mixamorig:Spine1");
        fleeDisplacementDist = 5f;
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
    // TODO: Flee
    public virtual bool Flee(Vector3 chaserPos)
    {
        Vector3 scientistPos = this.transform.position;
        Vector3 normDir = (chaserPos - scientistPos).normalized;
        normDir.y = 0;
        normDir = Quaternion.AngleAxis(45, Vector3.up) * normDir; 
        Vector3 newPos = scientistPos - normDir * fleeDisplacementDist;
        return GoTo(newPos, MaxSpeed);
    }

    #endregion

    #region Death
    public override void Die()
    {
        base.Die();
        aiSensor.enabled = false;
    }
    #endregion



}
