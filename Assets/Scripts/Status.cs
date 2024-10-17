using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{

    public float maxHealth = 20f;
    public float currHealth;
    // TODO: public float iframes;

    // public UnityEvent onDefeated;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    public virtual void TakeDamage(float damage)
    {
        currHealth -= damage;
        // if (currHealth <= 0) {
        //     OnDefeated();
        // }
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
        this.gameObject.SetActive(false);
    }
}
