using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    //public string BackgroundSceneToLoad;
    private GameObject player;

    // Start is called before the first frame update
    void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        DNA.SetPoints(0);
        player = GameObject.FindGameObjectWithTag("Player");
        if(player != null)
        {
            SceneManager.MoveGameObjectToScene(player, SceneManager.GetActiveScene());
            Destroy(player);
        }

    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game will quit now");
    }
}
