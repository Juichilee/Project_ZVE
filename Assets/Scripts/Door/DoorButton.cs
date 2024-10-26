using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public GameObject door;
    private Animator doorAnimator;
    private bool buttonHit;

    public Material buttonColor;
    private Renderer buttonRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = door.GetComponent<Animator>();
        buttonRenderer = this.GetComponent<Renderer>();
    }
    
    // When the door is already opened
    void Update()
    {
        buttonHit = doorAnimator.GetBool("ButtonHit");
        if (buttonHit == true)
        {
            buttonRenderer.material = buttonColor;
        }
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.gameObject.tag == "Player") {
            buttonRenderer.material = buttonColor;
            doorAnimator.SetTrigger("ButtonHit");
        }
    }
}
