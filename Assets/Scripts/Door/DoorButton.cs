using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public AudioClip press;
    private AudioSource audioSource;

    public GameObject door;
    private Animator doorAnimator;
    private bool buttonHit;
    private bool hasPlayedSound; // Flag to track if the sound has already been played

    public Material buttonColor;
    private Renderer buttonRenderer;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        doorAnimator = door.GetComponent<Animator>();
        buttonRenderer = this.GetComponent<Renderer>();
    }

    // When the door is already opened
    void Update()
    {
        buttonHit = doorAnimator.GetBool("ButtonHit");
        if (buttonHit)
        {
            buttonRenderer.material = buttonColor;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.gameObject.tag == "Player" && !buttonHit && !hasPlayedSound)
        {
            // Play the sound only if it hasn't been played yet
            audioSource.PlayOneShot(press);
            hasPlayedSound = true; // Mark the sound as played
            buttonRenderer.material = buttonColor;
            doorAnimator.SetTrigger("ButtonHit");
        }
    }
}
