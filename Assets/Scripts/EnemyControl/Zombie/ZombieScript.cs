using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Misc;
using UnityEngine;
using UnityEngine.AI;

//require some things the bot control needs
[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]
public class ZombieScript : MonoBehaviour
{
    // Component Reference
    private Animator anim; 
    private Rigidbody rb; 
    private CapsuleCollider cc;
    private NavMeshAgent aiAgent; 
    public GameObject player { get; private set; }
    public ZombieStatus status { get; private set; }
    
    // Pickup Prefabs 
    public Rigidbody pickupPrefab;
    public Rigidbody currPickup;

    // Animation Speed Variables
    public float animationSpeed;
    public float rootMovementSpeed;
    public float rootTurnSpeed;

    // Animation Properties 
    public Vector3 curVelocity;
    public Vector3 prevVelocity;
    public float maxVerticalSpeed = 10f; // Set the maximum expected speed in both direction
    private int groundContactCount = 0;
    public bool isGrounded = true;
    public bool IsGrounded { get { return groundContactCount > 0; } }
    public bool isDead = false;

    private float timeOfLastAttack = 0;
    private bool hasReachedAttackDist = false;

    // Range Variables
    public float chaseRange;
    public float attackRange;


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
        aiAgent = GetComponent<NavMeshAgent>();
        status = GetComponent<ZombieStatus>();
    }

    void Start() {
        animationSpeed = 1f;
        rootMovementSpeed = 1f;
        rootTurnSpeed = 1f;
        chaseRange = 10f;
        attackRange = 2f;
    }

    void FixedUpdate()
    {
        anim.speed = animationSpeed;
        DEBUGdistance = Vector3.Distance(this.transform.position, player.transform.position); 
        
        // Check Is Grounded
        float radius = GetComponent<CapsuleCollider>().radius * 0.9f;
        Vector3 pos = transform.position + Vector3.up*(radius*0.5f);
        LayerMask groundLayer = LayerMask.GetMask("ground");
        isGrounded = IsGrounded || Physics.CheckSphere(pos, radius, groundLayer);

        // Update Animation
        anim.SetFloat("vely", aiAgent.velocity.magnitude / aiAgent.speed);
        anim.SetBool("isFalling", !isGrounded);
    }

    public bool IsChaseRange()
    {
        return Vector3.Distance(this.transform.position, player.transform.position) <= chaseRange;
    }

    public float DEBUGdistance; 


    public bool IsAttackRange()
    {
        return Vector3.Distance(this.transform.position, player.transform.position) <= attackRange;
    }

    /* ======================================================================================================
    ______________________________________________.Movement._________________________________________________
    ====================================================================================================== */

    private bool targetSet = false;
    private Vector3 goToTarget;
    private float maxTargetDistDiff = 0.1f;
    
    public bool GoTo(Vector3 target) 
    {
        Vector3 direction = player.transform.position - transform.position;
        direction.y = 0;
        transform.rotation = Quaternion.LookRotation(direction, Vector3.up);

        if (targetSet && aiAgent.hasPath && !aiAgent.isPathStale) 
        {
            if (Vector3.Distance(goToTarget, target) < maxTargetDistDiff)
                return true;
        }

        if (NavMesh.SamplePosition(target, out NavMeshHit nmh, aiAgent.height * 3, NavMesh.AllAreas))
        {
            if (aiAgent.SetDestination(nmh.position))
            {
                targetSet = true;
                goToTarget = target;
                return true;
            }
        }
        return false;
    }

    public void GoToPlayer()
    {
        GoTo(player.transform.position);
    }

    public bool ReachedTarget()
    {
        return !aiAgent.pathPending && 
                aiAgent.pathStatus == NavMeshPathStatus.PathComplete && 
                aiAgent.remainingDistance <= aiAgent.stoppingDistance;
    }

    public void Stop()
    {
        aiAgent.ResetPath();
        aiAgent.isStopped = true;
        targetSet = false;
    }

    public void Die() 
    {
        isDead = true;
        Stop();
    }

    public void SpawnPickUp()
    {
        currPickup = Instantiate(pickupPrefab, transform);
        currPickup.transform.localPosition = new Vector3(0f, 1f, 0f);
        currPickup.isKinematic = true;
    } 


    /* ======================================================================================================
    ______________________________________________.Attack.___________________________________________________
    ====================================================================================================== */
    public void ResetTimeOfLastAttack()
    {
        timeOfLastAttack = Time.time;
    }

    public void LoseAgro()
    {
        anim.SetBool("isAttacking", false);
        if (hasReachedAttackDist)
            hasReachedAttackDist = false;
    }

    public bool isCooldown()
    {
        return Time.time <= timeOfLastAttack + status.AttackSpeed;
    }

    public void AttackTarget()
    {
        anim.SetBool("isAttacking", true);
        
        if (!hasReachedAttackDist)
        {
            hasReachedAttackDist = true;
            timeOfLastAttack = Time.time;
        }

        if (Time.time >= timeOfLastAttack + status.AttackSpeed)
        {
            Status playerStatus = player.GetComponent<Status>();
            status.DealDamage(playerStatus);
            timeOfLastAttack = Time.time;
        }

    }


    /* ======================================================================================================
    ______________________________________________.On Collision._____________________________________________
    ====================================================================================================== */
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
