using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    public AudioClip pickupSound;  // The sound to play when picked up
    public int healthAmount = 30;  // Amount of health the player will gain

    private Collider pickupCollider;
    private MeshRenderer pickupRenderer;

    void Awake()
    {
        // Get the collider and renderer components
        pickupCollider = GetComponent<Collider>();
        pickupRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.transform.gameObject.CompareTag("Player"))
        {
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
