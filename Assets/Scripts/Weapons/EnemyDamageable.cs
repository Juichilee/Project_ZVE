using System.Collections;
using UnityEngine;

public class EnemyDamageable : BasicDamageable
{
        // Debugging purposes
    public Material origMaterial;
    public Material onDamageMaterial;
    public MeshRenderer meshRenderer;
    public RagdollOnDeath ragdollOnDeath;
    public AudioClip deathSound;
    private AudioSource audioSource;
    public DamageData damage;
    public float AttackDamage { get; private set; }
    public float AttackSpeed { get; private set; }
    public bool CanAttack { get; private set; }


    void Awake()
    {
        //meshRenderer = GetComponent<MeshRenderer>();
        //origMaterial = meshRenderer.material;
    }

    public override void OnDamage(DamageData damageData)
    {
        if (IsInvincible || !IsAlive) return;
        CurrentHealth -= damageData.BaseDamage;

        // Handle special damage effects
        if (damageData.StunDamage > 0) StunHealth -= damageData.StunDamage;
        if (damageData.StaggerDamage > 0) StaggerHealth -= damageData.StaggerDamage;
        if (damageData.SlowDamage > 0) SlowHealth -= damageData.SlowDamage;
        if (damageData.BleedDamage > 0) BleedHealth -= damageData.BleedDamage;

        // Visualize taking damage
        //if(meshRenderer != null) meshRenderer.material = onDamageMaterial;

        StartCoroutine(InvincibilityFrames());
        Debug.Log(CurrentHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public void Update(){
        //if (!IsInvincible){
        //    meshRenderer.material = origMaterial;
        //}
    }

    public override void Die()
    {
        Debug.Log("dieing now");
        ragdollOnDeath.EnableRagdoll();
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

    }

    public void OnDefeated()
    {
        ragdollOnDeath.EnableRagdoll();
        if (deathSound != null)
        {
            audioSource.PlayOneShot(deathSound);
        }

    }

    public void DealDamage(PlayerStatus receiverStatus)
    {
        receiverStatus.OnDamage(damage);
    }

}