using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ExitLevel : MonoBehaviour
{
 //   private string nextLevelName = "Level2Scene";

    void OnTriggerEnter(Collider c)
    {
        if (c.attachedRigidbody != null)
        {
            if (c.attachedRigidbody.gameObject.tag == "Player")
            {
                if(SceneManager.GetActiveScene().name == "TutorialScene")
                {
                    GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<WeaponHandler>().DropAllWeapons();
                }
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
            }
        }
    }
}
