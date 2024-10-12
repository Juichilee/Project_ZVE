using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickup : MonoBehaviour
{
    // TODO: Add Reference to the script
    // public Status status;     
    public int HealAmt = 1;

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
            // TODO: Add EventManger.TriggerEvent<>
            if (c.gameObject.GetComponent<Status>().Heal(HealAmt))
                Destroy(this.gameObject);
            // TODO: status.health += 30 or status.gainHealth(30);
        }
    }
}
