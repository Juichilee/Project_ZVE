using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // TODO: Add Reference to the script
    // public Status status;     

    void Awake() {

        // TODO: Add Reference to the script
        // status = GetComponent<Status>();
    }

    void OnTriggerEnter(Collider c)
    {
        if (c.transform.gameObject.CompareTag("Player"))
        {
            // TODO: Add EventManger.TriggerEvent<>
            c.gameObject.GetComponent<Status>().currHealth += 1;
            Destroy(this.gameObject);
            // TODO: status.health += 30 or status.gainHealth(30);
        }
    }
}
