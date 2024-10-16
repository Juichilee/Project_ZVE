using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorButton : MonoBehaviour
{
    public GameObject door;
    private Animator doorAnimator;

    public Material buttonColor;
    private Renderer buttonRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        doorAnimator = door.GetComponent<Animator>();
        buttonRenderer = this.GetComponent<Renderer>();
    }

    void OnCollisionEnter(Collision c)
    {
        buttonRenderer.material = buttonColor;
        doorAnimator.SetTrigger("ButtonHit");
    }
}
