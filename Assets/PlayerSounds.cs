using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip runningClip;
    public AudioClip jumpGrunt;
    public AudioClip landingClip;
    public AudioClip deathClip;
    public AudioClip hurtClip;
    public AudioClip damageImpactClip;

    void Start()
    {
        // Initialize the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    // This function will be called by the animation event for running sounds
    public void runningSound()
    {
        if (runningClip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(runningClip); // Play the footstep sound
        }
    }

    public void Grunt()
    {
        if (jumpGrunt != null)
        {
            audioSource.PlayOneShot(jumpGrunt);
        }
    }

    public void Landing()
    {
        if (landingClip != null)
        {
            audioSource.PlayOneShot(landingClip);
        }
    }

    public void PlayDeathSound()  // Method to play the death sound
    {
        if (deathClip != null)
        {
            audioSource.PlayOneShot(deathClip);
        }
    }

    public void PlayerHurt()  // Method to play the death sound
    {
        if (hurtClip != null)
        {
            audioSource.PlayOneShot(hurtClip);
        }
    }

    public void GotHit()
    {
        if (damageImpactClip != null)
        {
            audioSource.PlayOneShot(damageImpactClip);
        }
    }
}
