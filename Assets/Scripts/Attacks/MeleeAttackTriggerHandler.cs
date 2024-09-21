using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class MeleeAttackTriggerHandler : MonoBehaviour
{

    public float attackDamage = 10f;
    public LayerMask enemyLayer;
    
    void OnTriggerEnter(Collider collider){
        Debug.Log("Trigger Entered");
    }

    void OnTriggerStay(Collider collider){
        Debug.Log("Trigger Stayed");
    }
}
