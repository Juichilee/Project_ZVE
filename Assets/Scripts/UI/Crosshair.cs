using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Crosshair : MonoBehaviour
{
    CharacterInputController characterInput;
    public GameObject crosshairUI;

    // Start is called before the first frame update
    void Start()
    {
        characterInput = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<CharacterInputController>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        crosshairUI.SetActive(characterInput.AimDown);
    }
}
