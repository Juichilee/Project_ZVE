using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    private AudioSource audioSource;
    public AudioClip healthUpgrade;
    public AudioClip speedUpgrade;
    public AudioClip strengthUpgrade;
    public AudioClip unstableUpgrade;

    public static int HealthCost = 2;
    public static int SpeedCost = 2;
    public static int StrengthCost = 2;
    public static int UnstableCost = 2;
    public int costIncrease = 2;
    PlayerStatus playerStatus;
    public TMP_Text HealthText;
    public TMP_Text SpeedText;
    public TMP_Text StrengthText;
    public TMP_Text UnstableText;
    public Slider MonsterLevel;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>();
        MonsterLevel.maxValue = 5;
        MonsterLevel.minValue = 0;
        HealthCost = 2 + costIncrease * playerStatus.hpUpgrade;
        SpeedCost = 2 + costIncrease * playerStatus.speedUpgrade;
        StrengthCost = 2 + costIncrease * playerStatus.strengthUpgrade;
        UnstableCost = 2 + costIncrease * playerStatus.monsterPoints;
    }

    // Update is called once per frame
    void Update()
    {
        HealthText.text = HealthCost.ToString();
        SpeedText.text = SpeedCost.ToString();
        StrengthText.text = StrengthCost.ToString();
        UnstableText.text = UnstableCost.ToString();
        MonsterLevel.value = playerStatus.monsterPoints;
    }

    public void Health()
    {
        if(DNA.GetPoints() >= HealthCost)
        {
            if (healthUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(healthUpgrade);
            }
            DNA.Addpoints(-1 * HealthCost);
            playerStatus.hpUpgrade += 1;
            HealthCost += costIncrease;
        }
    }

    public void Speed()
    {
        if (DNA.GetPoints() >= SpeedCost)
        {
            if (speedUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(speedUpgrade);
            }
            DNA.Addpoints(-1 * SpeedCost);
            playerStatus.speedUpgrade += 1;
            SpeedCost += costIncrease;
        }
    }

    public void Strength()
    {
        if (DNA.GetPoints() >= StrengthCost)
        {
            if (strengthUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(strengthUpgrade);
            }
            DNA.Addpoints(-1 * StrengthCost);
            playerStatus.strengthUpgrade += 1;
            StrengthCost += costIncrease;
        }
    }

    public void UnstableMutation()
    {
        if (DNA.GetPoints() >= UnstableCost)
        {
            if (unstableUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(unstableUpgrade);
            }
            DNA.Addpoints(-1 * UnstableCost);
            randomUpgrade();
            randomUpgrade();
            UnstableCost += costIncrease;
            playerStatus.monsterPoints += 1;
        }
    }

    void randomUpgrade()
    {
        int randomvalue = Random.Range(0, 2);
        switch (randomvalue)
        {
            case 0:
                playerStatus.strengthUpgrade += 1;
                StrengthCost += costIncrease;
                break;
            case 1:
                playerStatus.speedUpgrade += 1;
                SpeedCost += costIncrease;
                break;
            case 2:
                playerStatus.MaxHealth += 20;
                HealthCost += costIncrease;
                break;
            default:
                break;
        }
    }
}


