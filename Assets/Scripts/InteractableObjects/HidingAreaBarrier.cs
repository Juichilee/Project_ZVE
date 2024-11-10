using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HidingAreaBarrier : MonoBehaviour
{
    private Renderer objectRenderer;
    
    // Start is called before the first frame update
    void Start()
    {
        objectRenderer = GetComponent<Renderer>();
        objectRenderer.enabled = false;
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                objectRenderer.enabled = true;
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                objectRenderer.enabled = false;
            }
        }
    }
}
