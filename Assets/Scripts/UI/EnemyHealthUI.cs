using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider hp;
    GameObject enemy;
    ZombieStatus enemyStatus;
    
    // Start is called before the first frame update
    void Start()
    {
        enemy = this.transform.parent.gameObject;
        enemyStatus = enemy.GetComponent<ZombieStatus>();
        hp.maxValue = enemyStatus.maxHealth;
        hp.minValue = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hp.value = enemyStatus.currHealth;
        this.transform.LookAt(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform);
        
    }
}
