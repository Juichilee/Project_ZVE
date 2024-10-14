using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class HPText : MonoBehaviour
{

    Status playerStatus;
    TMP_Text Text;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Status>();
        Text = this.GetComponent<TMP_Text>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        Text.text = " " + playerStatus.currHealth + "/" + playerStatus.maxHealth;
    }
}
