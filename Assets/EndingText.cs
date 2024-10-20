using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class EndingText : MonoBehaviour
{
    public TMP_Text endingText;
    private int endingTextAlpha = 0;
    private float pauseTime = 3f;
    private bool fadingIn = true;
    private bool fadedOut = false;

    void FixedUpdate()
    {
        if (fadingIn == true)
        {
            if (endingTextAlpha < 255)
            {
                endingTextAlpha = endingTextAlpha + 1;
                endingText.color = new Color32(255, 255, 255, (byte) endingTextAlpha);
                if (endingTextAlpha >= 255)
                {
                    fadingIn = false;
                }
            }
        } else {
            pauseTime = pauseTime - Time.deltaTime;
            if (endingTextAlpha > 0 && pauseTime <= 0f)
            {
                endingTextAlpha = endingTextAlpha - 1;
                endingText.color = new Color32(255, 255, 255, (byte) endingTextAlpha);
                if (endingTextAlpha <= 0)
                {
                    fadedOut = true;
                }
            }
        }

        if (fadedOut == true)
        {
            Debug.Log("The game was completed");
        }
    }
}
