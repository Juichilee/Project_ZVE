using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    public GameObject settingsMenu;
    public GameObject pauseMenu;
    public GameObject mainUI;
    public GameObject crosshair;
    private static bool isPaused;

    public static bool GetIsPaused()
    {
        return isPaused;
    }

    // Start is called before the first frame update
    void Start()
    {
        crosshair = GameObject.FindWithTag("crosshair");
        Unpause();
    }

    private void Update()
    {
        if (Input.GetButtonDown("Pause"))
        {
            if (isPaused)
                Unpause();
            else
                Pause();
        }
    }

    public void StartGame()
    {
        SceneManager.LoadScene(1);
    }

    public void Pause()
    {
        Time.timeScale = 0f;
        crosshair.SetActive(false);
        pauseMenu.SetActive(true);
        settingsMenu.SetActive(false);
        mainUI.SetActive(false);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        isPaused = true;
    }

    public void Unpause()
    {
        Time.timeScale = 1f;
        crosshair.SetActive(true);
        pauseMenu.SetActive(false);
        settingsMenu.SetActive(false);
        mainUI.SetActive(true);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        isPaused = false;
    }

    public void MainMenu()
    {
        DNA.SetPoints(0);
        SceneManager.LoadScene("Main Menu");
    }

    public void QuitGame()
    {
        Application.Quit();
        Debug.Log("Game would quit now");
    }

    
}
