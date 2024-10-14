using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ZombieStatus : Status
{
    // Script Reference
    public RagdollOnDeath ragdollOnDeath;

    private float attackDamage= 10f;
    private float attackSpeed = 1f;

    private bool canAttack;

    void Awake()
    {
        ragdollOnDeath = GetComponent<RagdollOnDeath>();
    }

    public override void TakeDamage(float damage)
    {
        currHealth -= damage;
        if (currHealth <= 0) {
            OnDefeated();
        }
    }

    public override void OnDefeated()
    {
        ragdollOnDeath.EnableRagdoll();
    }

    public void DealDamage(Status receiverStatus) 
    {
        receiverStatus.TakeDamage(attackDamage);
    }

}
