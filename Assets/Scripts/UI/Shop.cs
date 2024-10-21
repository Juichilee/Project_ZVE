using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class Shop : MonoBehaviour
{
    public static int HealthCost = 2;
    public static int SpeedCost = 2;
    public static int StrengthCost = 2;
    public int costIncrease = 2;
    Status playerStatus;
    public TMP_Text HealthText;
    public TMP_Text SpeedText;
    public TMP_Text StrengthText;

    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Status>();
    }

    // Update is called once per frame
    void Update()
    {
        HealthText.text = HealthCost.ToString();
        SpeedText.text = SpeedCost.ToString();
        StrengthText.text = StrengthCost.ToString();
    }

    public void Health()
    {
        if(DNA.GetPoints() >= HealthCost)
        {
            DNA.Addpoints(-1 * HealthCost);
            playerStatus.maxHealth += 20;
            HealthCost += costIncrease;
        }
    }

    public void Speed()
    {
        if (DNA.GetPoints() >= SpeedCost)
        {
            DNA.Addpoints(-1 * SpeedCost);
            playerStatus.speedUpgrade += 1;
            SpeedCost += costIncrease;
        }
    }

    public void Strength()
    {
        if (DNA.GetPoints() >= StrengthCost)
        {
            DNA.Addpoints(-1 * StrengthCost);
            playerStatus.strengthUpgrade += 1;
            StrengthCost += costIncrease;
        }
    }
}
