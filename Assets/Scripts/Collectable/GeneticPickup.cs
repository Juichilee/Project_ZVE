using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticPickup : MonoBehaviour
{
    // TODO: Add Reference to the script
    // public Status status;     
    public int PointAmt = 1;

    void Awake() {

        // TODO: Add Reference to the script
        // status = GetComponent<Status>();
    }

    private void FixedUpdate()
    {
        this.transform.Rotate(Vector3.up);
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.transform.gameObject.CompareTag("Player"))
        {
            // TODO: add point counter to player
            DNA.Addpoints(PointAmt);
            Destroy(this.gameObject);
        }
    }
}
