using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockingDevice : MonoBehaviour
{
    private Animator blockingDeviceAnimator;
    
    // Start is called before the first frame update
    void Start()
    {
        blockingDeviceAnimator = this.GetComponent<Animator>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player" || c.attachedRigidbody.gameObject.tag == "Enemy")
            {
                blockingDeviceAnimator.SetTrigger("Unblocking");
            }
        }
    }

    void OnTriggerExit(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player" || c.attachedRigidbody.gameObject.tag == "Enemy")
            {
                blockingDeviceAnimator.ResetTrigger("Unblocking");
            }
        }
    }
}
