using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(Animator), typeof(Rigidbody), typeof(CapsuleCollider))]
[RequireComponent(typeof(NavMeshAgent))]
public class EnemyBase : MonoBehaviour, IMovable, IKillable 
{
    #region Component Reference
    protected Animator anim; 
    protected Rigidbody rb; 
    protected CapsuleCollider cc;
    protected NavMeshAgent aiAgent;
    #endregion

    #region Animation Properties 
    protected int groundContactCount = 0;
    public bool isGrounded = true;
    public bool IsGrounded { get { return groundContactCount > 0; } }
    public bool isDead = false;
    public float animationSpeed;
    public float rootMovementSpeed;
    public float rootTurnSpeed;
    #endregion


    protected virtual void Start()
    {
        // animationSpeed = 1f;
        // rootMovementSpeed = 1f;
        // rootTurnSpeed = 1f;
    }

    
    #region Movement
    public virtual bool GoTo(Vector3 position, float speed = 0f)
    {
        if (NavMesh.SamplePosition(position, out NavMeshHit nmh, aiAgent.height * 3, NavMesh.AllAreas))
        {
            if (aiAgent.SetDestination(nmh.position))
                return true;
        }
        return false;
    }

    public virtual bool ReachedTarget()
    {
        return !aiAgent.pathPending &&
                aiAgent.pathStatus == NavMeshPathStatus.PathComplete &&
                aiAgent.remainingDistance <= aiAgent.stoppingDistance;
    }

    public virtual void Stop()
    {
        aiAgent.ResetPath();
        aiAgent.isStopped = true;
    }
    #endregion



    #region Death
    public virtual void Die()
    {
        isDead = true;
        Stop();
    }

    public virtual void SpawnPickUp()
    {}
    #endregion



    #region Callbacks
    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.gameObject.CompareTag("ground"))
        {
            ++groundContactCount;
            EventManager.TriggerEvent<PlayerLandsEvent, Vector3, float>(collision.contacts[0].point, collision.impulse.magnitude);
        }
    }

    void OnCollisionExit(Collision collision)
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

        newRootRotation = anim.rootRotation;
        newRootPosition = Vector3.LerpUnclamped(this.transform.position, newRootPosition, rootMovementSpeed);
        newRootRotation = Quaternion.LerpUnclamped(this.transform.rotation, newRootRotation, rootTurnSpeed);

        rb.MovePosition(newRootPosition);
        rb.MoveRotation(newRootRotation);
        aiAgent.nextPosition = newRootPosition;
    }
    #endregion
}
