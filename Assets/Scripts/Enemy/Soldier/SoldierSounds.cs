using UnityEngine;

public class SoldierSounds : MonoBehaviour
{
    private AudioSource audioSource;

    public AudioClip footstepClip;
    public AudioClip attackSound;
    public AudioClip alertSound; // When the soldier spots the player
    public AudioClip outOfRange;

    private float lastOutOfRangeSoundTime = 0f;
    public float outOfRangeCooldown = 5f;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public void PlayFootstepSound()
    {
        if (footstepClip != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(footstepClip);
        }
    }

    public void PlayAttackSound()
    {
        if (attackSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(attackSound);
        }
    }

    public void PlayAlertSound()
    {
        if (alertSound != null && !audioSource.isPlaying)
        {
            audioSource.PlayOneShot(alertSound);
        }
    }

    public void PlayerOutOfRange()
    {
        if (outOfRange != null && Time.time - lastOutOfRangeSoundTime > outOfRangeCooldown)
        {
            audioSource.PlayOneShot(outOfRange);
            lastOutOfRangeSoundTime = Time.time;
        }
    }
}
