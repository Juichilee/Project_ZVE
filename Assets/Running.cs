using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Running : MonoBehaviour
{
    public AudioClip running;      // Reference to the running AudioClip
    private AudioSource audioSource;   // Reference to the AudioSource
    public float speed = 5f;       // Player movement speed

    private void Start()
    {
        // Get the AudioSource component from the player
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        // Get input from arrow keys (horizontal and vertical)
        float moveHorizontal = Input.GetAxis("Horizontal");
        float moveVertical = Input.GetAxis("Vertical");

        // Check if the player is moving
        if (moveHorizontal != 0 || moveVertical != 0)
        {
            // Move the player
            Vector3 movement = new Vector3(moveHorizontal, 0.0f, moveVertical);
            transform.Translate(movement * speed * Time.deltaTime);

            // Play the running sound if it's not already playing
            if (!audioSource.isPlaying)
            {
                audioSource.clip = running;
                audioSource.Play();
            }
        }
        else
        {
            // Stop playing the running sound when player stops moving
            if (audioSource.isPlaying)
            {
                audioSource.Stop();
            }
        }
    }
}
