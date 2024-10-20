using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip runningClip;
    public AudioClip meleeSoundClip;
    public AudioClip jumpGrunt;
    public AudioClip landingClip;
    public AudioClip gunshot;
    public AudioClip gunClick;

    private bool hasPlayedGunReady = false;

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

    public void Shots()
    {
        if (gunshot != null)
        {
            audioSource.PlayOneShot(gunshot);
        }
    }

    public void GunReady()
    {
        if (!hasPlayedGunReady && gunClick != null)
        {
            audioSource.PlayOneShot(gunClick);
            hasPlayedGunReady = true;
        }
    }

    public void ResetGunReadySound()
    {
        hasPlayedGunReady = false;
    }
}
