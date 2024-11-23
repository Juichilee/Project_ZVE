using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FallAfterCollision : MonoBehaviour
{
    private Rigidbody objectRigidbody;
    private float objectDestructionValue = 3f;

    // Start is called before the first frame update
    void Start()
    {
        objectRigidbody = GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.gameObject.tag == "Player" || c.transform.gameObject.tag == "Projectile")
        {
            objectRigidbody.isKinematic = false;
            objectRigidbody.useGravity = true;
            Destroy(this.gameObject, objectDestructionValue);
        }
    }
}
