using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttackTriggerHandler : MonoBehaviour
{

    public float attackDamage = 10f;
    public LayerMask enemyLayer;
    
    void OnTriggerEnter(Collider other){
        Debug.Log("Trigger Enter");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if(other.gameObject.layer == enemyLayer)
        {
            other.gameObject.GetComponent<Status>().TakeDamage(1f);
        }
    }

    void OnTriggerStay(Collider other){
        Debug.Log("Trigger Stayed");
        int enemyLayer = LayerMask.NameToLayer("Enemy");
        if(other.gameObject.layer == enemyLayer)
        {
            other.gameObject.GetComponent<Status>().TakeDamage(1f);
        }
    }
}
