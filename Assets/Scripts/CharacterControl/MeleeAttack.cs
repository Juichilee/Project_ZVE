using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeleeAttack : MonoBehaviour
{
    public float attackDuration = 2f;  // Duration the trigger stays active
    public float attackRange = 1f;       // Radius of the attack trigger
    public LayerMask enemyLayer;         // Layer of enemies to check against
    private bool isAttacking = false;    // To check if an attack is in progress
    public GameObject attackTriggerPrefab; // Trigger Prefab 
    public Transform MeleePos;
    public CharacterInputController cinput;
    bool _inputMelee = false;

    void Awake()
    {
        cinput = GetComponent<CharacterInputController>();
        if (cinput == null)
            Debug.Log("CharacterInput could not be found");
    }

    private PlayerSounds playerSounds; // Reference to PlayerSounds

    void Start()
    {
        // Get the PlayerSounds component from the same GameObject
        playerSounds = GetComponent<PlayerSounds>();
    }

    public void startAttack()
    {
        if (!isAttacking)
        {
            StartCoroutine(SpawnAttackTrigger());
            playerSounds.PlayMeleeSound(); // Play melee sound when attacking
        }
    }

    // Coroutine to spawn the attack trigger for a short duration
    IEnumerator SpawnAttackTrigger()
    {
        isAttacking = true;

        // Create a trigger collider temporarily
        GameObject attackTrigger = Instantiate(attackTriggerPrefab, MeleePos);
        attackTrigger.transform.position = MeleePos.position + transform.forward * attackRange / 2; // Slightly in front of the player
        attackTrigger.transform.rotation = MeleePos.rotation;

        // Destroy the attack trigger after the attack duration
        yield return new WaitForSeconds(attackDuration);
        Destroy(attackTrigger);

        isAttacking = false;
    }
    void Update()
    {
        if (cinput.enabled)
        {
            _inputMelee = _inputMelee || cinput.Melee;
        }
    }

    void FixedUpdate()
    {
        if (_inputMelee)
        {
            _inputMelee = false;
            startAttack();
        }
    }
}
