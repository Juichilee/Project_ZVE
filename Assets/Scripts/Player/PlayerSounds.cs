using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip runningClip;
    public AudioClip jumpGrunt;
    public AudioClip specialJumpGrunt;
    public AudioClip slamDunk;
    public AudioClip doTheRoar;
    public AudioClip mutantRoar;
    public AudioClip landingClip;
    public AudioClip deathClip;
    public AudioClip hurtClip;
    public AudioClip damageImpactClip;

    private PlayerControlScript playerControlScript;

    void Start()
    {
        // Initialize the AudioSource component
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerControlScript = GetComponent<PlayerControlScript>();
        if (playerControlScript == null)
        {
            Debug.LogWarning("PlayerControlScript not found on Player.");
        }
    }

    // This function will be called by the animation event for running sounds
    public void runningSound()
    {
        if (runningClip != null && playerControlScript.IsGrounded && !audioSource.isPlaying)
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

    public void JumpAttackGrunt()
    {
        if (specialJumpGrunt != null)
        {
            audioSource.PlayOneShot(specialJumpGrunt);
        }
    }

    public void SlamSound()
    {
        if (slamDunk != null)
        {
            audioSource.PlayOneShot(slamDunk);
        }
    }

    public void PlayerRoar()
    {
        if (doTheRoar != null)
        {
            audioSource.PlayOneShot(doTheRoar);
        }
    }

    public void PlayerMutantRoar()
    {
        if (mutantRoar != null)
        {
            audioSource.PlayOneShot(mutantRoar);
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
