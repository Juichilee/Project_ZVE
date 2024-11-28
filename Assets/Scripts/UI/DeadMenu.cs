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
            PauseMenu.SetIsPaused(true);
        }
    }

    public void RestartLevel()
    {
        PlayerControlScript playerInst = PlayerControlScript.PlayerInstance;
        WeaponHandler weaponHandler = playerInst.GetComponent<WeaponHandler>();
        
        weaponHandler.ResetWeapons();
        PauseMenu.SetIsPaused(false);
        dna.GetComponent<DNA>().ResetPoints();
        playerStatus.CurrentHealth = playerStatus.MaxHealth;
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

    void OnEnable()
    {
        // Subscribe to the sceneLoaded event
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        // Unsubscribe from the event
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // This method is called whenever a scene is loaded.
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayerControlScript playerInst = PlayerControlScript.PlayerInstance;
        Transform spawnPointLoc = SpawnPoint.spawnInstance.gameObject.transform;
        playerInst.gameObject.transform.position = spawnPointLoc.position;
        playerInst.gameObject.transform.rotation = spawnPointLoc.rotation;
        Debug.Log("Scene loaded: " + scene.name);
    }
}
