using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PauseMenu))]
public class DeadMenu : MonoBehaviour
{

    public GameObject Menu;
    PauseMenu pause;
    PlayerStatus playerStatus;
    public GameObject dna;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(false);
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>();
        pause = this.GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerStatus.CurrentHealth <= 0)
        {
            Menu.SetActive(true);
            pause.enabled = false;
            Time.timeScale = 0f;
            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;
        }
    }

    public void RestartLevel()
    {
        dna.GetComponent<DNA>().ResetPoints();
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
