using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel3 : MonoBehaviour
{
    private string nextSceneName = "EndingScene";

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                SceneManager.LoadScene(nextSceneName);
            }
        }
    }
}
