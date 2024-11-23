using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CleanupScript : MonoBehaviour
{
    private GameObject player;
    
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            Destroy(player);
        }
    }
}
