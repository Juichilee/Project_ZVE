using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class TestDamageable : BasicDamageable
{
    public Material origMaterial;
    public Material damageMaterial;
    public MeshRenderer meshRenderer;

    void Awake()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        origMaterial = meshRenderer.material;
        if(!damageMaterial)
        {
            Debug.LogError("Test Damageable needs a damageMaterial");
        }
    }

    public override void OnDamage(DamageData damageData)
    {
        if (IsInvincible || !IsAlive) return;
        CurrentHealth -= damageData.BaseDamage + GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>().strengthUpgrade * 2;

        meshRenderer.material = damageMaterial;

        // Handle special damage effects
        if (damageData.StunDamage > 0) StunHealth -= damageData.StunDamage;
        if (damageData.StaggerDamage > 0) StaggerHealth -= damageData.StaggerDamage;
        if (damageData.SlowDamage > 0) SlowHealth -= damageData.SlowDamage;
        if (damageData.BleedDamage > 0) BleedHealth -= damageData.BleedDamage;

        StartCoroutine(InvincibilityFrames());
        Debug.Log(CurrentHealth);
        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    void Update()
    {
        if (!IsInvincible)
        {
            meshRenderer.material = origMaterial;
        }
    }
}
