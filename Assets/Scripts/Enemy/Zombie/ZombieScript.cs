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
public class ZombieScript : MonoBehaviour, IMovable, IKillable, IAttacker
{
    #region Component Reference
    private Animator anim; 
    private Rigidbody rb; 
    private CapsuleCollider cc;
    private NavMeshAgent aiAgent; 
    public EnemyDamageable EnemyDamageable { get; private set; }
    public AISensor aiSensor { get; private set; }
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
    public float animationSpeed;
    public float rootMovementSpeed;
    public float rootTurnSpeed;
    #endregion

    #region Animation Properties 
    public float ZombieMaxSpeed { get; private set; }
    private int groundContactCount = 0;
    public bool isGrounded = true;
    public bool IsGrounded { get { return groundContactCount > 0; } }
    public bool isDead = false;
    #endregion

    // Awake is To grab the components
    void Awake()
    {
        // Components
        anim = GetComponent<Animator>();
        anim.enabled = true;
        anim.applyRootMotion = true;
        
        aiAgent = GetComponent<NavMeshAgent>();
        aiAgent.updatePosition = false;
        aiAgent.updateRotation = true;

        rb = GetComponent<Rigidbody>();
        cc = GetComponent<CapsuleCollider>();
        cc.enabled = true;
        EnemyDamageable = GetComponent<EnemyDamageable>();
        aiSensor = GetComponent<AISensor>();
    }

    void Start() 
    {
        animationSpeed = 1f;
        rootMovementSpeed = 1f;
        rootTurnSpeed = 1f;
        attackRange = 2f;
        chaseRange = 10f;
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
        ZombieMaxSpeed = aiAgent.velocity.magnitude / aiAgent.speed;
        anim.SetBool("isFalling", !isGrounded);
    }

    #region Movement

    public float maxLookAheadTime = 0.5f;

    public bool GoTo(Vector3 position, float speed = 0f)
    {   
        anim.SetFloat("vely", speed);
        
        if (NavMesh.SamplePosition(position, out NavMeshHit nmh, aiAgent.height * 3, NavMesh.AllAreas))
        {
            if (aiAgent.SetDestination(nmh.position))
                return true;
        }
        return false;
    }

    public bool GoToPlayer()
    {
        PlayerControlScript player = PlayerControlScript.PlayerInstance;

        Vector3 currPos = this.transform.position;
        Vector3 playerPos = player.transform.position;

        float distance = Vector3.Distance(currPos, playerPos);
        float speed = aiAgent.speed;
        float lookAheadTime = Mathf.Clamp(distance / speed, 0 , maxLookAheadTime);

        Vector3 velocity = player.GetComponent<PlayerControlScript>().WorldVelocity;
        Vector3 predictedPosition = playerPos + velocity * lookAheadTime;
        
        if (NavMesh.Raycast(playerPos, predictedPosition, out NavMeshHit hit, NavMesh.AllAreas))
            predictedPosition = hit.position;

        return GoTo(predictedPosition, ZombieMaxSpeed);
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
    }

    #endregion

    #region Death

    public void Die() 
    {
        isDead = true;
        Stop();
        aiSensor.enabled = false;
    }

    public void SpawnPickUp()
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
    
    #region Range Variables
    public float attackRange;
    public float chaseRange;
    #endregion


    public bool IsInAttackRange()
    {
        Vector3 playerPosition = PlayerControlScript.PlayerInstance.transform.position;
        return Vector3.Distance(this.transform.position, playerPosition) <= attackRange;
    }

    public bool IsInSight()
    {
        return aiSensor.IsInSight(PlayerControlScript.PlayerInstance.gameObject);
    }

    public void GainAgro()
    {
        anim.SetTrigger("isAttacking");
    }

    public void AttackTarget()
    {
        anim.SetTrigger("isAttacking");
        
    }

    #endregion

    
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
        
        newRootPosition.y = aiAgent.nextPosition.y;

        // Here, scale the difference in position and rotation to make the character go faster or slower
        newRootRotation = anim.rootRotation;
        newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
        newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, rootTurnSpeed);

        rb.MovePosition(newRootPosition);
        rb.MoveRotation(newRootRotation);
        aiAgent.nextPosition = newRootPosition;
    }

}
