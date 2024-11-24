using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class DNA : MonoBehaviour
{
    public static int DNAPoints = 0;
    int startPoints = 0;
    public TMP_Text dnaText;
    public GameObject PanelPrompt;

    // Start is called before the first frame update
    void Start()
    {
        startPoints = DNAPoints;
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (SceneManager.GetActiveScene().name == "Level1Scene")
        {
            DNAPoints = 0;
        }
    }
    // Update is called once per frame
    void Update()
    {
        dnaText.text = "" + DNAPoints;
        if(PanelPrompt != null)
        {
            if (DNAPoints >= Shop.SwordCost || DNAPoints >= Shop.SlamCost || DNAPoints >= Shop.ScreamCost || DNAPoints >= Shop.UnstableCost)
            {
                PanelPrompt.SetActive(true);
            }
            else PanelPrompt.SetActive(false);
        }
    }

    public static void Addpoints(int points)
    {
        DNAPoints += points;
    }

    public static int GetPoints()
    {
        return DNAPoints;
    }

    public static void SetPoints(int points)
    {
        DNAPoints = points;
    }

    public void ResetPoints()
    {
        DNAPoints = startPoints;
    }
}
