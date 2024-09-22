using System;
using System.Collections;
using System.Collections.Generic;
using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{

    public float maxHealth = 2f;
    public float currHealth;

    // public UnityEvent onDefeated;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        currHealth -= damage;
        // if (currHealth <= 0) {
        //     OnDefeated();
        // }
    }

    public void OnDefeated()
    {
        this.gameObject.SetActive(false);
    }
}
