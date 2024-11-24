using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlayerStatus : BasicDamageable
{
    public int baseHealth = 100;
    public int speedUpgrade = 0;
    public int strengthUpgrade = 0;
    public int hpUpgrade = 0;
    public bool sword;
    public bool slam;
    public bool scream;
    public int monsterPoints = 0;
    private int startSpeed;
    private int startStrength;
    private int startHp;
    private int startMonster;
    private bool startSword;
    private bool startSlam;
    private bool startScream;
    // TODO: public float iframes;

    private bool isDead = false;

    #region Sound
    private PlayerSounds playerSounds;
    #endregion

    void Awake()
    {
        maxHealth = baseHealth + 20 * hpUpgrade;
        currentHealth = baseHealth + 20 * hpUpgrade;

        playerSounds = GetComponent<PlayerSounds>();
        if (playerSounds == null)
        {
            Debug.LogWarning("PlayerSounds component not found.");
        }

        startSpeed = speedUpgrade;
        startStrength = strengthUpgrade;
        startHp = hpUpgrade;
        startMonster = monsterPoints;
        startSword = sword;
        startSlam = slam;
        startScream = scream;
    }

    private void FixedUpdate()
    {
        maxHealth = baseHealth + 20 * hpUpgrade;
        if(currentHealth <= 0)
        {
            speedUpgrade = startSpeed;
            strengthUpgrade = startStrength;
            hpUpgrade = startHp;
            monsterPoints = startMonster;
            sword = startSword;
            slam = startSlam;
            scream = startScream;
        }
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
        Debug.Log($"You died.");
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
