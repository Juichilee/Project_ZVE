using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenu : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip pauseSound;

    public GameObject settingsMenu;
    public GameObject pauseMenu;
    public GameObject mainUI;
    public GameObject crosshair;
    private static bool isPaused;

    public static bool GetIsPaused()
    {
        return isPaused;
    }

    public static void SetIsPaused(bool pause)
    {
        isPaused = pause;
    }

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
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
        audioSource.PlayOneShot(pauseSound, 0.3f);
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
