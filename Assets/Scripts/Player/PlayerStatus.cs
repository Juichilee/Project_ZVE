using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : BasicDamageable
{
    public int speedUpgrade = 0;
    public int strengthUpgrade = 0;
    public int monsterPoints = 0;
    // TODO: public float iframes;

    private bool isDead = false;

    void Awake()
    {
        maxHealth = 200;
        currentHealth = 200;
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

        StartCoroutine(InvincibilityFrames());

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        Debug.Log($"The Player has died.");
        // Handle object destruction or death animations
    }

    // public UnityEvent onDefeated;

    // Start is called before the first frame update
    // public virtual void TakeDamage(int damage)
    // {
    //     if (isDead) return;  // Prevent damage after death

    //     CurrentHealth -= damage;

    //     if (currHealth <= 0)
    //     {
    //         OnDefeated();
    //     }
    // }

    public Boolean Heal(int healAmt)
    {
        if (CurrentHealth == maxHealth)
            return false;
        else if (CurrentHealth + healAmt >= maxHealth)
            CurrentHealth = maxHealth;
        else
            CurrentHealth += healAmt;
        return true;
    }

    // public virtual void OnDefeated()
    // {
    //     if (!isDead)
    //     {
    //         isDead = true;  // Mark the zombie as dead to prevent further actions

    // /*        // Play death sound
    //         if (deathSound != null)
    //         {
    //             audioSource.PlayOneShot(deathSound);
    //         }*/
    //     }
    // }
}
