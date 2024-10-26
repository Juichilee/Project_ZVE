using System.Collections;
using UnityEngine;

public class EnemyDamageable : BasicDamageable
{
        // Debugging purposes
    public Material origMaterial;
    public Material onDamageMaterial;
    public MeshRenderer meshRenderer;
    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        origMaterial = meshRenderer.material;
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
        meshRenderer.material = onDamageMaterial;

        StartCoroutine(InvincibilityFrames());

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public new void Update(){
        if (!IsInvincible){
            meshRenderer.material = origMaterial;
        }
    }

    public override void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Handle object destruction or death animations
    }
}