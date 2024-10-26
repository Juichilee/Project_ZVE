using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlastAfterContact : MonoBehaviour
{
    public GameObject blastParticles;
    private string planeTag = "ground";

    void OnCollisionEnter(Collision c)
    {
        if (c.transform.gameObject.tag != planeTag)
        {
            Instantiate(blastParticles, this.transform.position, this.transform.rotation);
            Destroy(this.gameObject);
        }
    }
}
