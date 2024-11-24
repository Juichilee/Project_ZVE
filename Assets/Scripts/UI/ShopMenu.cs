using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PauseMenu))]
public class ShopMenu : MonoBehaviour
{

    public GameObject Menu;
    public GameObject ExchangeMenu;
    PauseMenu pause;
    public bool shopping = false;
    public bool exShopping = false;
    MutationExchange exchange;

    // Start is called before the first frame update
    void Start()
    {
        Menu.SetActive(false);
        pause = this.GetComponent<PauseMenu>();
    }

    // Update is called once per frame
    void Update()
    {
        exchange = GameObject.Find("ExchangeTrigger").GetComponent<MutationExchange>();
        if (exchange == null)
        {
            Debug.LogError("MutationExchange menu needs an MutationExchange location");
        }
        if (Input.GetKeyDown(KeyCode.E) && exchange.getInTrigger())
        {
            if (exShopping && !shopping)
            {
                ExContinue();
            }
            else if (!shopping)
            {
                ExShop();
            }
        }
        if (Input.GetButtonDown("Shop"))
        {
            if (shopping && !exShopping)
            {
                Continue();
            }
            else if(!exShopping)
            {
                Shop();
            }
        }
    }

    public void Continue()
    {
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

    public void ExContinue()
    {
        Time.timeScale = 1f;
        ExchangeMenu.SetActive(false);
        pause.enabled = true;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        exShopping = false;
        PauseMenu.SetIsPaused(false);
    }

    public void ExShop()
    {
        Time.timeScale = 0f;
        ExchangeMenu.SetActive(true);
        pause.enabled = false;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        exShopping = true;
        PauseMenu.SetIsPaused(true);
    }

}
