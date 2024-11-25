using UnityEngine;

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class MovePlatform : MonoBehaviour
{
    private Animator animator;
    private Rigidbody rb;

    void Start()
    {
        animator = GetComponent<Animator>();
        rb = GetComponent<Rigidbody>();
        rb.isKinematic = true;
    }

    void OnAnimatorMove()
    {
        
        rb.MovePosition(animator.rootPosition);
        rb.MoveRotation(animator.rootRotation);
    }
}
