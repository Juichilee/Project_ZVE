using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponHolderUI : MonoBehaviour
{
    public GameObject gun;
    public GameObject knife;
    private WeaponHandler weaponHandler;
    private Image image;
    public int CurrentSlot = 0;
    public Color normalColor = Color.cyan;
    public Color selectedColor = Color.blue;
    // Start is called before the first frame update
    void Start()
    {
        weaponHandler = GameObject.FindGameObjectWithTag("Player").GetComponent<WeaponHandler>();
        image = GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        Weapon current = weaponHandler.GetWeapon(CurrentSlot);
        string currentName = "";
        if (current != null) currentName = current.WeaponName;
        switch (currentName)
        {
            case "Pistol":
                gun.SetActive(true);
                knife.SetActive(false);
                break;
            case "Knife":
                gun.SetActive(false);
                knife.SetActive(true);
                break;
            default:
                gun.SetActive(false);
                knife.SetActive(false);
                break;
        }

        if (weaponHandler.GetCurrentWeaponIndex() == CurrentSlot) image.color = selectedColor;
        else image.color = normalColor;

    }
}
