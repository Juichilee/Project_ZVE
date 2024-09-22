using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using UnityEngine;

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class EnemyRootMotionControl : MonoBehaviour
{
    // Component Reference
    private Animator anim;
    private Rigidbody rb;
    private CapsuleCollider cc;
    public UnityEngine.AI.NavMeshAgent aiAgent;
    public GameObject player;

    // Script Reference
    public RagdollOnDeath ragdollOnDeath;
    
    // Animation Variables
    public float animationSpeed = 1f;
    public float rootMovementSpeed = 1f;
    public float rootTurnSpeed = 1f;

    public Vector3 curVelocity;
    public Vector3 prevVelocity;
    public float maxVerticalSpeed = 10f; // Set the maximum expected speed in both direction
    private int groundContactCount = 0;
    public bool isGrounded = true;

    public bool IsGrounded
    {
        get
        {
            return groundContactCount > 0;
        }
    }

    public float health;
    public float maxHealth = 100f;
    public bool isDead = false;

    // Awake is To grab the components
    void Awake()
    {
        // Components
        anim = GetComponent<Animator>();
        anim.enabled = true;
        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        cc.enabled = true;
        player = GameObject.FindWithTag("Player");
        aiAgent = GetComponent<UnityEngine.AI.NavMeshAgent>();

        ragdollOnDeath = GetComponent<RagdollOnDeath>();
    }

    // Start is called before the first frame update
    void Start()
    {
        health = maxHealth;
    }

    // Update is called once per frame
    void Update()
    {
        if (health <= 0 && !isDead)
            Die();

        MoveToPlayer();
    }

    void FixedUpdate()
    {
        anim.speed = animationSpeed;
        
        // Check Is Grounded
        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up*(radius*0.5f);
        LayerMask groundLayer = LayerMask.GetMask("ground");
        isGrounded = IsGrounded || Physics.CheckSphere(pos, radius, groundLayer);

        // Update Animation
        anim.SetFloat("vely", aiAgent.velocity.magnitude / aiAgent.speed);
        anim.SetBool("isFalling", !isGrounded);
    }

    private void MoveToPlayer()
    {
        aiAgent.SetDestination(player.transform.position);
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
    }

    private void Die() 
    {
        isDead = true;
        anim.enabled = false;
        cc.enabled = false;
        aiAgent.isStopped = true;
    }


    //This is a physics callback
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("ground"))
        {
            ++groundContactCount;
            EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("ground"))
            --groundContactCount;
    }

    void OnAnimatorMove()
    {
        Vector3 newRootPosition;
        Quaternion newRootRotation;

        if (isGrounded)
            newRootPosition = anim.rootPosition;        
        else
            newRootPosition = new Vector3(anim.rootPosition.x, this.transform.position.y, anim.rootPosition.z);

        // Here, scale the difference in position and rotation to make the character go faster or slower
        newRootRotation = anim.rootRotation;
        newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
        newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, rootTurnSpeed);

        rb.MovePosition(newRootPosition);
        rb.MoveRotation(newRootRotation);
    }

}
