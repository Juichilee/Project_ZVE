using System.Collections;
using System.Collections.Generic;
using Unity.Collections;
using UnityEngine;

public class ZombieStatus : Status
{
    // Script Reference
    public RagdollOnDeath ragdollOnDeath;
    public AudioClip deathSound;
    private AudioSource audioSource;

    public float AttackDamage { get; private set; } 
    public float AttackSpeed { get; private set; } 
    public bool CanAttack { get; private set; } 

    void Awake()
    {
        ragdollOnDeath = GetComponent<RagdollOnDeath>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void Start()
    {
        maxHealth = 20f;
        currHealth = maxHealth;
        AttackDamage = 10f;
        AttackSpeed = 3f;
        CanAttack = false;
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
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

    }

    public void DealDamage(Status receiverStatus) 
    {
        receiverStatus.TakeDamage(AttackDamage);
    }

}
