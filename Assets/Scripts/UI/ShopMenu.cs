using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PauseMenu))]
public class ShopMenu : MonoBehaviour
{

    public GameObject Menu;
    PauseMenu pause;
    public bool shopping = false;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(false);
        pause = this.GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Shop"))
        {
            if (shopping)
            {
                Continue();
            }
            else
            {
                Shop();
            }
        }
    }

    public void Continue(){
        Time.timeScale = 1f;
        Menu.SetActive(false);
        pause.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        shopping = false;
        PauseMenu.SetIsPaused(false);
    }

    public void Shop()
    {
        Time.timeScale = 0f;
        Menu.SetActive(true);
        pause.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        shopping = true;
        PauseMenu.SetIsPaused(true);
    }

}
