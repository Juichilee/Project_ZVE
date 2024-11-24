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
                c.transform.root.position = new Vector3(0, 0, 60);
                SceneManager.LoadScene(nextLevelName);
            }
        }
    }
}
