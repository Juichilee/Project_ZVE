using System;
using System.Collections;
using System.Collections.Generic;
//using Unity.PlasticSCM.Editor.WebApi;
using UnityEngine;
using UnityEngine.Events;

public class Status : MonoBehaviour
{

    public float maxHealth = 2f;
    public float currHealth;
    public AudioClip deathSound;        
    private AudioSource audioSource;

    private bool isDead = false;

    // public UnityEvent onDefeated;

    // Start is called before the first frame update
    void Start()
    {
        currHealth = maxHealth;

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void TakeDamage(float damage)
    {
        if (isDead) return;  // Prevent damage after death

        currHealth -= damage;

        if (currHealth <= 0)
        {
            OnDefeated();
        }
    }

    public void OnDefeated()
    {
        if (!isDead)
        {
            isDead = true;  // Mark the zombie as dead to prevent further actions

            // Play death sound
            if (deathSound != null)
            {
                audioSource.PlayOneShot(deathSound);
            }
        }
    }
}
