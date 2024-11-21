using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class EndingText : MonoBehaviour
{
    public TMP_Text endingText;
    private string badEndingText = "Thank you for completing the game!\nYou found an escape helicopter.\nHowever, you couldn't pilot it since\nyou lost too much humanity\nand became a mindless monster.\nPlay again for a better ending!";

    private int endingTextAlpha = 0;
    private float pauseTime = 3f;
    private bool fadingIn = true;
    private bool fadedOut = false;

    private GameObject player;
    private float monsterLevel = 0f;
    private float badEndingMonsterLevel = 4f;

    public GameObject enemyGroup;

    private void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (player != null)
        {
            // Monster Level
            if (monsterLevel >= badEndingMonsterLevel)
            {
                endingText.text = badEndingText;
            } else {
                if (enemyGroup != null)
                {
                    enemyGroup.SetActive(false);
                }
            }

            SceneManager.MoveGameObjectToScene(player, SceneManager.GetActiveScene());
            Destroy(player);
        }
    }

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
            SceneManager.LoadScene("Main Menu");
        }
    }
}
