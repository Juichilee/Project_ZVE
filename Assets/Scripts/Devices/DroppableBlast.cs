using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableBlast : MonoBehaviour
{
    public GameObject blastParticles;
    public AudioClip blowUp;
    private Renderer objectRenderer;
    private GameObject instantiatedBlastParticles;
    private AudioSource audioSource;
    private float destroyDelay = 1.2f; // delay in seconds

    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    void OnCollisionEnter(Collision c)
    {
        Debug.Log("Collision detected with: " + c.transform.tag);

        if (blowUp != null)
        {
            Debug.Log("Explosion sound triggered.");
            audioSource.PlayOneShot(blowUp);
        }
        else {
            Debug.LogWarning("AudioClip 'blowUp' is missing!");
        }

        instantiatedBlastParticles = Instantiate(blastParticles, this.transform.position, this.transform.rotation);
        Destroy(instantiatedBlastParticles, destroyDelay);

        objectRenderer.enabled = false;
        // delay so that we can hear audio
        Destroy(this.gameObject, destroyDelay);
    }
}
