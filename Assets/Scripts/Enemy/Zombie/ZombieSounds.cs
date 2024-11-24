using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSounds : MonoBehaviour
{
    // public AudioClip thudSound;
    public AudioClip footstepClip;
    public AudioClip attackSound;
    public AudioClip alertSound;  // Sound for spotting the player
    public AudioClip idleSound;   // Sound for idle state

    private AudioSource audioSource;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }
    public void ZombieWalk()
    {
        if (footstepClip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(footstepClip, 0.8f);
        }
    }

    public void ZombieAttack()
    {
        if (attackSound != null && !audioSource.isPlaying)
        {
            Debug.Log("ZombieAttack event triggered.");
            audioSource.PlayOneShot(attackSound, 0.5f);
        }
    }

    public void ZombieAlert()
    {
        if (alertSound != null && !audioSource.isPlaying)
        {
            Debug.Log("ZombieAlert event triggered.");
            audioSource.PlayOneShot(alertSound, 1.0f);
        }
    }

    public void ZombieIdle()
    {
        if (idleSound != null && !audioSource.isPlaying)
        {
            Debug.Log("ZombieIdle event triggered.");
            audioSource.PlayOneShot(idleSound);
        }
    }

    /*public void Thud()
    {
        if (thudSound != null)
        {
            Debug.Log("Thud event triggered.");
            audioSource.PlayOneShot(thudSound);
        }
    }*/
}

