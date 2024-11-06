using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppableBlast : MonoBehaviour
{
    public GameObject blastParticles;
    public AudioClip blowUp;
    private AudioSource audioSource;
    private float destroyDelay = 1.2f; // delay in seconds

    // Start is called before the first frame update
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
        if (blowUp != null)
        {
            audioSource.PlayOneShot(blowUp);
        }
        else {
            Debug.LogWarning("AudioClip 'blowUp' is missing!");
        }

        Instantiate(blastParticles, this.transform.position, this.transform.rotation);

        Destroy(this.gameObject, destroyDelay);
    }
}
