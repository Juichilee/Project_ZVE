using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class MeleeAttack : MonoBehaviour
{
    public float attackDuration = 2f;  // Duration the trigger stays active
    public float attackRange = 1f;       // Radius of the attack trigger
    public LayerMask enemyLayer;         // Layer of enemies to check against
    private bool isAttacking = false;    // To check if an attack is in progress
    public GameObject attackTriggerPrefab; // Trigger Prefab 
    public void startAttack(){
        if(!isAttacking){
            StartCoroutine(SpawnAttackTrigger());
        }
    }
    // Coroutine to spawn the attack trigger for a short duration
    IEnumerator SpawnAttackTrigger()
    {
        isAttacking = true;

        // Create a trigger collider temporarily
        GameObject attackTrigger = Instantiate(attackTriggerPrefab);
        attackTrigger.transform.position = transform.position + transform.forward * attackRange / 2; // Slightly in front of the player
        attackTrigger.transform.rotation = transform.rotation;

        // Add the attack trigger component to handle enemy detection
        // attackHandler.attackDamage = attackDamage;
        // attackHandler.enemyLayer = enemyLayer;

        // Destroy the attack trigger after the attack duration
        yield return new WaitForSeconds(attackDuration);
        Destroy(attackTrigger);

        isAttacking = false;
    }
}
