using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[RequireComponent(typeof(PauseMenu))]
public class DeadMenu : MonoBehaviour
{

    public GameObject Menu;
    PauseMenu pause;
    Status playerStatus;
    public GameObject dna;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(false);
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Status>();
        pause = this.GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if(playerStatus.currHealth <= 0)
        {
            Menu.SetActive(true);
            pause.enabled = false;
            //Time.timeScale = 0f;
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
