using System.Collections;
using UnityEngine;

public class EnemyDamageable : BasicDamageable
{
    public RagdollOnDeath ragdollOnDeath;
    public AudioClip deathSound;
    public AudioClip damageSound;
    private AudioSource audioSource;
    public DamageData damage;
    public float AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public bool CanAttack { get; private set; }


    void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void OnDamage(DamageData damageData)
    {
        if (IsInvincible || !IsAlive) return;
        CurrentHealth -= damageData.BaseDamage + PlayerControlScript.PlayerInstance.gameObject.GetComponent<PlayerStatus>().strengthUpgrade * 2;

        // Handle special damage effects
        if (damageData.StunDamage > 0) StunHealth -= damageData.StunDamage;
        if (damageData.StaggerDamage > 0) StaggerHealth -= damageData.StaggerDamage;
        if (damageData.SlowDamage > 0) SlowHealth -= damageData.SlowDamage;
        if (damageData.BleedDamage > 0) BleedHealth -= damageData.BleedDamage;

        if (damageSound != null)
        {
            audioSource.PlayOneShot(damageSound);
        }

        StartCoroutine(InvincibilityFrames());
        Debug.Log(CurrentHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        ragdollOnDeath.EnableRagdoll();
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

    }

    public void OnDefeated()
    {
        ragdollOnDeath.EnableRagdoll();
    }

    public void DealDamage(PlayerStatus receiverStatus)
    {
        receiverStatus.OnDamage(damage);
    }
}