using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PauseScript : MonoBehaviour
{
    private bool isPaused = true; 

    void Start()
    {
        Time.timeScale = 0f;
    }

    void Update()
    {
        if (Input.GetKeyUp(KeyCode.P))
        {
            isPaused = !isPaused;
            Time.timeScale = isPaused ? 0f : 1f;
        }
    }
}
