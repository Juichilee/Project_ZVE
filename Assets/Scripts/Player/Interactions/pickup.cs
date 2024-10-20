using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pickup : MonoBehaviour
{
    private void OnTriggerEnter(Collider c)
    {
        if (c.CompareTag("Player"))
        {
            Destroy(this.gameObject);
        }

    }
}