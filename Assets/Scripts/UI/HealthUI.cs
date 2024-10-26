using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
public class HealthUI : MonoBehaviour
{
    public Slider hp;
    PlayerStatus playerStatus;
    public TMP_Text Text;

    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<PlayerStatus>();
        hp.maxValue = playerStatus.MaxHealth;
        hp.minValue = 0;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    void FixedUpdate()
    {
        hp.maxValue = playerStatus.MaxHealth;
        hp.value = playerStatus.CurrentHealth;
        Text.text = " " + playerStatus.CurrentHealth + "/" + playerStatus.MaxHealth;
    }
}
