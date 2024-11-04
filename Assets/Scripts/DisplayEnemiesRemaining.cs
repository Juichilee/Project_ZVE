using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DisplayEnemiesRemaining : MonoBehaviour
{
    public TMP_Text goalText;
    public GameObject enemiesRemaining;

    private EnemiesRemaining enemiesRemainingScript;
    private int enemiesRemainingNumber;
    private string beforeNumberPart = "Goal: Defeat ";
    private string afterNumberPart = " enemies to open this level's exit door";
    private string openedExitDoorText = "Goal: Reach this level's opened exit door";
    
    // Start is called before the first frame update
    void Start()
    {
        if (enemiesRemaining == null)
        {
            enemiesRemaining = GameObject.Find("EnemiesRemaining");
        }
        
        enemiesRemainingScript = enemiesRemaining.GetComponent<EnemiesRemaining>();
        enemiesRemainingNumber = enemiesRemainingScript.obtainEnemiesRemaining();
    }

    // Update is called once per frame
    void Update()
    {
        enemiesRemainingNumber = enemiesRemainingScript.obtainEnemiesRemaining();
        if (enemiesRemainingNumber < 0)
        {
            enemiesRemainingNumber = 0;
        }
        if (enemiesRemainingNumber == 0)
        {
            goalText.text = openedExitDoorText;
        } else {
            goalText.text = beforeNumberPart + enemiesRemainingNumber.ToString() + afterNumberPart;
        }
    }
}
