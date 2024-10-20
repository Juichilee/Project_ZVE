using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NextLevelGateway : MonoBehaviour
{
    private string nextLevelName = "Level2";

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }
}
