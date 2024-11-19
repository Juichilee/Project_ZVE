using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : BasicDamageable
{
    public int speedUpgrade = 0;
    public int strengthUpgrade = 0;
    public int hpUpgrade = 0;
    public int monsterPoints = 0;
    private PlayerSounds playerSounds;
    private int startSpeed;
    private int startStrength;
    private int startHp;
    private int startMonster;

    // TODO: public float iframes;

    private bool isDead = false;

    void Awake()
    {
        maxHealth = 200 + 20 * hpUpgrade;
        currentHealth = 200 + 20 * hpUpgrade;

        playerSounds = GetComponent<PlayerSounds>();
        if (playerSounds == null)
        {
            Debug.LogWarning("PlayerSounds component not found.");
        }

        startSpeed = speedUpgrade;
        startStrength = strengthUpgrade;
        startHp = hpUpgrade;
        startMonster = monsterPoints;
    }

    private void FixedUpdate()
    {
        maxHealth = 200 + 20 * hpUpgrade;
        if(currentHealth <= 0)
        {
            speedUpgrade = startSpeed;
            strengthUpgrade = startStrength;
            hpUpgrade = startHp;
            monsterPoints = startMonster;
        }
    }

    public override void OnDamage(DamageData damageData)
    {
        Debug.Log("I GOT HIT");

        if (IsInvincible || !IsAlive) return;
        CurrentHealth -= damageData.BaseDamage;

        // Handle special damage effects
        if (damageData.StunDamage > 0) StunHealth -= damageData.StunDamage;
        if (damageData.StaggerDamage > 0) StaggerHealth -= damageData.StaggerDamage;
        if (damageData.SlowDamage > 0) SlowHealth -= damageData.SlowDamage;
        if (damageData.BleedDamage > 0) BleedHealth -= damageData.BleedDamage;
        
        if (playerSounds != null)
        {
            playerSounds.PlayerHurt();
            playerSounds.GotHit();
        }

        StartCoroutine(InvincibilityFrames());

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    public override void Die()
    {
        if (isDead) return; // returns dead status

        isDead = true;
        Debug.Log($"The Player has died.");
        if (playerSounds != null)
        {
            playerSounds.PlayDeathSound();
        }
    }

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
}
