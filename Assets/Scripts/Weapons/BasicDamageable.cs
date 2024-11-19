using System.Collections;
using UnityEngine;

public class BasicDamageable : Damageable
{
    [SerializeField] protected int maxHealth = 100;
    [SerializeField] protected int currentHealth = 100;
    [SerializeField] protected bool isStunned;
    [SerializeField] protected float stunHealth = 50f;
    [SerializeField] protected bool isStaggered;
    [SerializeField] protected float staggerHealth = 50f;
    [SerializeField] protected bool isSlowed;
    [SerializeField] protected float slowHealth = 30f;
    [SerializeField] protected bool isBleed;
    [SerializeField] protected float bleedHealth = 20f;
    [SerializeField] protected bool isInvincible = false;
    protected float invincibilityDuration = 0.1f;

    #region Accessors
    public override int MaxHealth { get => maxHealth; set => maxHealth = value; }
    public override int CurrentHealth { get => currentHealth; set => currentHealth = value; }
    public override bool IsStunned { get => isStunned; set => isStunned = value; }
    public override float StunHealth { get => stunHealth; set => stunHealth = value; }
    public override bool IsStaggered { get => isStaggered; set => isStaggered = value; }
    public override float StaggerHealth { get => staggerHealth; set => staggerHealth = value; }
    public override bool IsSlowed { get => isSlowed; set => isSlowed = value; }
    public override float SlowHealth { get => slowHealth; set => slowHealth = value; }
    public override bool IsBleed { get => isBleed; set => isBleed = value; }
    public override float BleedHealth { get => bleedHealth; set => bleedHealth = value; }
    public override bool IsInvincible { get => isInvincible; set => isInvincible = value; }
    public override float InvincibilityDuration { get => invincibilityDuration; set => invincibilityDuration = value; }
    #endregion

    public override void OnDamage(DamageData damageData)
    {
        if (IsInvincible || !IsAlive) return;
        CurrentHealth -= damageData.BaseDamage;

        // Handle special damage effects
        if (damageData.StunDamage > 0) StunHealth -= damageData.StunDamage;
        if (damageData.StaggerDamage > 0) StaggerHealth -= damageData.StaggerDamage;
        if (damageData.SlowDamage > 0) SlowHealth -= damageData.SlowDamage;
        if (damageData.BleedDamage > 0) BleedHealth -= damageData.BleedDamage;

        StartCoroutine(InvincibilityFrames());

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log($"{gameObject.name} has died.");
        // Handle object destruction or death animations
    }
}