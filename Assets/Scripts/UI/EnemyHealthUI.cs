using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthUI : MonoBehaviour
{
    public Slider hp;
    GameObject enemy;
    EnemyDamageable enemyStatus;
    
    // Start is called before the first frame update
    void Start()
    {
        enemy = this.transform.parent.gameObject;
        enemyStatus = enemy.GetComponent<EnemyDamageable>();
        hp.maxValue = enemyStatus.MaxHealth;
        hp.minValue = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        hp.value = enemyStatus.CurrentHealth;
        this.transform.LookAt(GameObject.FindGameObjectsWithTag("MainCamera")[0].transform);
        
    }
}
