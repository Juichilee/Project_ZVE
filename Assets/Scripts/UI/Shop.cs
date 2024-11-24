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
    public AudioClip gunPurchase;
    public AudioClip unstableUpgrade;
    private GameObject exchange;

    public GameObject gunPrefab;
    public static int HealthCost = 2;
    public static int SpeedCost = 2;
    public static int StrengthCost = 2;
    public static int GunCost = 5;
    public static int SwordCost = 5;
    public static int SlamCost = 5;
    public static int ScreamCost = 5;
    public static int UnstableCost = 2;
    public int costIncrease = 2;
    PlayerStatus playerStatus;
    public TMP_Text HealthText;
    public TMP_Text SpeedText;
    public TMP_Text StrengthText;
    public TMP_Text GunText;
    public TMP_Text SwordText;
    public TMP_Text SlamText;
    public TMP_Text ScreamText;
    public TMP_Text UnstableText;
    public Slider MonsterLevel;
    public int MaxHumanity = 5;

    // Start is called before the first frame update
    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>();
        MonsterLevel.maxValue = MaxHumanity;
        MonsterLevel.minValue = 0;
        HealthCost = 2 + costIncrease * playerStatus.hpUpgrade;
        SpeedCost = 2 + costIncrease * playerStatus.speedUpgrade;
        StrengthCost = 2 + costIncrease * playerStatus.strengthUpgrade;
        UnstableCost = 2 + costIncrease * playerStatus.monsterPoints;
        exchange = GameObject.Find("MutationExchange");
    }

    // Update is called once per frame
    void Update()
    {
        if(HealthText != null) HealthText.text = HealthCost.ToString();
        if (SpeedText != null) SpeedText.text = SpeedCost.ToString();
        if (StrengthText != null) StrengthText.text = StrengthCost.ToString();
        if (GunText != null) GunText.text = GunCost.ToString();
        if (SwordText != null) SwordText.text = SwordCost.ToString();
        if (SlamText != null) SlamText.text = SlamCost.ToString();
        if (ScreamText != null) ScreamText.text = ScreamCost.ToString();
        if (UnstableText != null) UnstableText.text = UnstableCost.ToString();
        MonsterLevel.value = MaxHumanity - playerStatus.monsterPoints;
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

    public void Gun()
    {
        if (DNA.GetPoints() >= GunCost)
        {
            if (gunPurchase != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(gunPurchase);
            }
            DNA.Addpoints(-1 * GunCost);
            Object.Instantiate(gunPrefab, position: new Vector3(exchange.transform.position.x + 2, exchange.transform.position.y, exchange.transform.position.z), rotation: exchange.transform.rotation);
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

    public void UnlockSword()
    {
        if (DNA.GetPoints() >= SwordCost && !playerStatus.sword)
        {
            if (unstableUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(unstableUpgrade);
            }
            DNA.Addpoints(-1 * UnstableCost);
            playerStatus.sword = true;
            playerStatus.monsterPoints += 1;
        }
    }

    public void UnlockSlam()
    {
        if (DNA.GetPoints() >= SwordCost && !playerStatus.slam)
        {
            if (unstableUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(unstableUpgrade);
            }
            DNA.Addpoints(-1 * UnstableCost);
            playerStatus.slam = true;
            playerStatus.monsterPoints += 1;
        }
    }

    public void UnlockScream()
    {
        if (DNA.GetPoints() >= SwordCost && !playerStatus.scream)
        {
            if (unstableUpgrade != null && !audioSource.isPlaying)
            {
                audioSource.PlayOneShot(unstableUpgrade);
            }
            DNA.Addpoints(-1 * UnstableCost);
            playerStatus.scream = true;
            playerStatus.monsterPoints += 1;
        }
    }
}


