using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    public AudioClip runningClip; // Assign the footstep sound in the inspector
    private AudioSource audioSource;

    public AudioClip meleeSoundClip;
    public AudioClip jumpGrunt;
    public AudioClip landingClip;

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

    // Method to play melee sound (called from MeleeAttack script)
    public void PlayMeleeSound()
    {
        if (meleeSoundClip != null)
        {
            audioSource.PlayOneShot(meleeSoundClip);
        }
    }

    public void grunt()
    {
        if (jumpGrunt != null)
        {
            audioSource.PlayOneShot(jumpGrunt);
        }
    }

    public void Landing()
    {
        Debug.Log("Landing sound triggered!");

        if (landingClip != null)
        {
            audioSource.PlayOneShot(landingClip);
        }
    }
}
