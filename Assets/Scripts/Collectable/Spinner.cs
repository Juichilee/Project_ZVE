using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spinner : MonoBehaviour
{
    public int type = 1;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (type == 0)
            this.transform.Rotate(Vector3.right);
        else
            this.transform.Rotate(Vector3.up);
    }
}
