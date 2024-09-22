using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum RagdollState {
    anim,
    ragdoll
}

[RequireComponent(typeof(Animator), typeof(Rigidbody))]
public class RagdollOnDeath : MonoBehaviour
{
    private Animator anim;
    public RagdollState rdState; 

    // Ragdoll Members
    public Transform ragdollRoot;
    public Rigidbody[] rbs;
    private CharacterJoint[] joints;
    private Collider[] ragdollColliders;

    void Awake() 
    {
        anim = GetComponent<Animator>();
        rbs = ragdollRoot.GetComponentsInChildren<Rigidbody>();
        joints = ragdollRoot.GetComponentsInChildren<CharacterJoint>();
        ragdollColliders = ragdollRoot.GetComponentsInChildren<Collider>();
        rdState = RagdollState.anim;

    }

    // Update is called once per frame
    void Update()
    {
        if (rdState == RagdollState.anim)
            EnableAnimator();    
        else
            EnableRagdoll();
    }

    public void EnableAnimator() 
    {
        anim.enabled = true;
        foreach (CharacterJoint joint in joints)
            joint.enableCollision = false;
        foreach (Collider collider in ragdollColliders)
            collider.enabled = false;
        foreach (Rigidbody rb in rbs) 
        {
            rb.detectCollisions = false;
            rb.useGravity = false;
        }
    }

    public void EnableRagdoll()
    {
        anim.enabled = false;
        foreach (CharacterJoint joint in joints)
            joint.enableCollision = true;
        foreach (Collider collider in ragdollColliders)
            collider.enabled = true;
        foreach (Rigidbody rb in rbs) 
        {
            rb.velocity = Vector3.zero;
            rb.detectCollisions = true;
            rb.useGravity = true;
        }
    }
}
