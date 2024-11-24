using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TMP_Text))]
public class DisplayEnemiesRemaining : MonoBehaviour
{
    private TMP_Text goalText;
    public GameObject enemiesRemaining;

    private EnemiesRemaining enemiesRemainingScript;
    private int enemiesRemainingNumber;
    private string beforeNumberPart = "Goal: Defeat ";
    private string afterNumberPart = " enemies to open this level's exit door";
    private string afterNumberOnePart = " enemy to open this level's exit door";
    private string openedExitDoorText = "Goal: Reach this level's opened exit door";

    // Start is called before the first frame update
    void Start()
    {
        goalText = GetComponent<TMP_Text>();

        if (enemiesRemaining == null)
        {
            enemiesRemaining = GameObject.Find("EnemiesRemaining");
        }
        
        if (enemiesRemaining != null)
        {
            enemiesRemainingScript = enemiesRemaining.GetComponent<EnemiesRemaining>();
            enemiesRemainingNumber = enemiesRemainingScript.obtainEnemiesRemaining();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (enemiesRemainingScript)
        {
            enemiesRemainingNumber = enemiesRemainingScript.obtainEnemiesRemaining();
        }
        if (enemiesRemainingNumber < 0)
        {
            enemiesRemainingNumber = 0;
        }
        if (enemiesRemainingNumber == 0)
        {
            goalText.text = openedExitDoorText;
        } else {
            if (enemiesRemainingNumber == 1)
            {
                goalText.text = beforeNumberPart + enemiesRemainingNumber.ToString() + afterNumberOnePart;
            } else {
                goalText.text = beforeNumberPart + enemiesRemainingNumber.ToString() + afterNumberPart;
            }
            
        }
    }
}
