using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthUI : MonoBehaviour
{

    Status playerStatus;
    float StartValue;
    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Status>();
        StartValue = this.gameObject.transform.localScale.x;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        this.gameObject.transform.localScale = new Vector3(playerStatus.currHealth / playerStatus.maxHealth * StartValue, this.gameObject.transform.localScale.y, this.gameObject.transform.localScale.z);
    }
}
