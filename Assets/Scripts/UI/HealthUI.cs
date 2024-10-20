using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthUI : MonoBehaviour
{
    public Slider hp;
    Status playerStatus;
    public TMP_Text Text;

    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Status>();
        hp.maxValue = playerStatus.maxHealth;
        hp.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        hp.value = playerStatus.currHealth;
        Text.text = " " + playerStatus.currHealth + "/" + playerStatus.maxHealth;
    }
}
