using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAfterTrigger : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private float objectDestructionValue = 3f;

    // Start is called before the first frame update
    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                objectRigidbody.isKinematic = false;
                objectRigidbody.useGravity = true;
                Destroy(this.gameObject, objectDestructionValue);
            }
        }
    }
}
