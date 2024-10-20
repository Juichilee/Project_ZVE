using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // TODO: Add Reference to the script
    // public Status status;     
    public int HealAmt = 1;
    private AudioSource audioSource;
    public AudioClip pickupSound;  // The sound to play when picked up
    public int healthAmount = 30;  // Amount of health the player will gain

    private Collider pickupCollider;
    private MeshRenderer pickupRenderer;

    void Awake()
    {
        // Get the collider and renderer components
        pickupCollider = GetComponent<Collider>();
        pickupRenderer = GetComponent<MeshRenderer>();

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    private void FixedUpdate()
    {
        this.transform.Rotate(Vector3.up);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.transform.gameObject.CompareTag("Player"))
        {
            // TODO: Add EventManger.TriggerEvent<>
            if (c.gameObject.GetComponent<Status>().Heal(HealAmt))
                Destroy(this.gameObject);
            // TODO: status.health += 30 or status.gainHealth(30);
            // Increase the player's health
            c.gameObject.GetComponent<Status>().currHealth += healthAmount;

            // Play the pickup sound from the player's AudioSource
            AudioSource playerAudio = c.gameObject.GetComponent<AudioSource>();
            if (playerAudio != null && pickupSound != null)
            {
                playerAudio.PlayOneShot(pickupSound);
            }

            // Disable the collider and hide the pickup object
            pickupCollider.enabled = false;
            if (pickupRenderer != null)
            {
                pickupRenderer.enabled = false;
            }

            // Destroy the health pickup object
            Destroy(gameObject);
        }
    }
}
