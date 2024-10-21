using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{

    public float maxHealth = 20f;
    public float currHealth;
    public int speedUpgrade = 0;
    public int strengthUpgrade = 0;
    // TODO: public float iframes;

    private bool isDead = false;

    // public UnityEvent onDefeated;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        if (isDead) return;  // Prevent damage after death

        currHealth -= damage;

        if (currHealth <= 0)
        {
            OnDefeated();
        }
    }

    public Boolean Heal(float healAmt)
    {
        if (currHealth == maxHealth)
            return false;
        else if (currHealth + healAmt >= maxHealth)
            currHealth = maxHealth;
        else
            currHealth += healAmt;
        return true;
    }

    public virtual void OnDefeated()
    {
        if (!isDead)
        {
            isDead = true;  // Mark the zombie as dead to prevent further actions

    /*        // Play death sound
            if (deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }*/
        }
    }
}
