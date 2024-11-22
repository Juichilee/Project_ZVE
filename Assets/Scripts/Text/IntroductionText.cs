using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class IntroductionText : MonoBehaviour
{
    public TMP_Text introductionText;
    private int introductionTextAlpha = 0;
    private float pauseTime = 3f;
    private bool fadingIn = true;
    private bool fadedOut = false;

    void Start()
    {
        Debug.Log("Introduction will start now");
        // Time scale is 1f
        Time.timeScale = 1f;
    }

    void Update()
    {
        if (introductionTextAlpha > 150 && Input.GetKeyDown(KeyCode.Space))
        {
            fadedOut = true;
        }
    }
    
    void FixedUpdate()
    {
        if (fadingIn == true)
        {
            if (introductionTextAlpha < 255)
            {
                introductionTextAlpha = introductionTextAlpha + 1;
                introductionText.color = new Color32(255, 255, 255, (byte) introductionTextAlpha);
                if (introductionTextAlpha >= 255)
                {
                    fadingIn = false;
                }
            }
        } else {
            pauseTime = pauseTime - Time.deltaTime;
            if (introductionTextAlpha > 0 && pauseTime <= 0f)
            {
                introductionTextAlpha = introductionTextAlpha - 1;
                introductionText.color = new Color32(255, 255, 255, (byte) introductionTextAlpha);
                if (introductionTextAlpha <= 0)
                {
                    fadedOut = true;
                }
            }
        }
        
        // Load the next scene
        if (fadedOut == true)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
        }
    }
}
