using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileShield : MonoBehaviour
{   
    void OnCollisionEnter(Collision c)
    {
        if (c.transform.gameObject.tag == "Projectile")
        {
            Destroy(c.transform.gameObject);
        }
    }
}
