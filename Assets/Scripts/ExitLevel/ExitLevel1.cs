using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel1 : MonoBehaviour
{
    private string nextLevelName = "Level2Scene";

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                c.transform.root.position = new Vector3(-3, 0, -28);
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }
}
