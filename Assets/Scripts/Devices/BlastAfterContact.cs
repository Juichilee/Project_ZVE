using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAfterContact : MonoBehaviour
{
    public GameObject blastParticles;
    public AudioClip blowUp;
    private AudioSource audioSource;
    private string planeTag = "ground";
    private string playerTag = "Player"; 
    private float destroyDelay = 1.2f; // delay in seconds

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision c)
    {
        Debug.Log("Collision detected with: " + c.transform.tag);

        // collision with the player
        if (c.transform.CompareTag(playerTag))
        {
            if (blowUp != null)
            {
                Debug.Log("Explosion sound triggered.");
                audioSource.PlayOneShot(blowUp);
            }
            else
            {
                Debug.LogWarning("AudioClip 'blowUp' is missing!");
            }

            Instantiate(blastParticles, this.transform.position, this.transform.rotation);

            // delay so that we can hear audio
            Destroy(this.gameObject, destroyDelay);
        }
        else if (c.transform.gameObject.tag != planeTag)
        {
            if (blowUp != null)
            {
                Debug.Log("Explosion sound triggered.");
                audioSource.PlayOneShot(blowUp);
            }
            Instantiate(blastParticles, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject, destroyDelay);
        }
    }
}
