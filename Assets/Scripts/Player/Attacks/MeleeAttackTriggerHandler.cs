using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttackTriggerHandler : MonoBehaviour
{

    public float baseAttackDamage = 1f;
    public float attackDamage = 1f;
    public LayerMask enemyLayer;
    Status playerStatus;

    // Start is called before the first frame update
    void Start()
    {
        playerStatus = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<Status>();
    }

    void FixedUpdate()
    {
        attackDamage = baseAttackDamage + playerStatus.strengthUpgrade;
    }
    
    void OnTriggerEnter(Collider other){
        // Debug.Log("Trigger Enter");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if(other.gameObject.layer == enemyLayer)
        {
            other.gameObject.GetComponent<Status>().TakeDamage(attackDamage);
        }
    }

    void OnTriggerStay(Collider other){
        // Debug.Log("Trigger Stayed");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if(other.gameObject.layer == enemyLayer)
        {
            other.gameObject.GetComponent<Status>().TakeDamage(attackDamage);
        }
    }
}
