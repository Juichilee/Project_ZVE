using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieSounds : MonoBehaviour
{
    public AudioClip footstepClip;      // Assign the footstep sound in the inspector
    // public AudioClip deathSoundClip;    // Assign the death sound in the inspector
    private AudioSource audioSource;    // AudioSource for playing sounds

    private bool isDead = false;        // To prevent playing sounds after death

    void Start()
    {
        // Initialize the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // This method will be called by the animation event "ZombieWalk"
    public void ZombieWalk()
    {
        if (footstepClip != null && !audioSource.isPlaying)
        {
            Debug.Log("ZombieWalk animation event triggered.");
            audioSource.PlayOneShot(footstepClip);  // Play the footstep sound when the zombie walks
        }
    }

    // Call this method when the zombie dies to play the death sound
   /* public void PlayDeathSound()
    {
        if (!isDead && deathSoundClip != null)
        {
            audioSource.PlayOneShot(deathSoundClip); // Play the death sound
            isDead = true; // Mark the zombie as dead so no other sounds play after this
        }
    } */
}
