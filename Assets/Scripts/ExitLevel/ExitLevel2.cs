using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel2 : MonoBehaviour
{
    private string nextLevelName = "Level3Scene";

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