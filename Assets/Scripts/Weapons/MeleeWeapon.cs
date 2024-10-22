using UnityEngine;
using System.Collections;

public class MeleeWeapon : BaseWeapon
{
    // Weapon References
    public GameObject projectileObj;
    public Transform shootPos;
    public override WeaponType WeaponType => WeaponType.Melee;

    // References that need to be populated / updated on pickup
    // public GameObject weaponHolder;
    // public Vector3 holdPosition;
    // public Vector3 holdRotation;
    // private PlayerSounds playerSounds;

    // Weapon Attributes
    // public string weaponName;
    // public int damage;
    public override float coolDownTime => 1f;
    // public bool isReady;
    public GameObject attackTriggerPrefab; // Trigger Prefab 
    public Transform meleePos;


    public override void Attack()
    {
        if (isReady)
        {
            // Attack logic
            Debug.Log($"Melee attack with {weaponName}");

            SpawnAttackTrigger();
        }
    }

    // Coroutine to spawn the attack trigger for a short duration
    void SpawnAttackTrigger()
    {
        // playerSounds.PlayMeleeSound(); // Play melee sound when attacking

        // Create a trigger collider temporarily
        GameObject attackTrigger = Instantiate(attackTriggerPrefab, meleePos);
        attackTrigger.transform.position = meleePos.position + transform.forward * 2; // Slightly in front of the player
        attackTrigger.transform.rotation = meleePos.rotation;

        StartCoroutine(AttackCooldown(attackTrigger));
    }

    private IEnumerator AttackCooldown(GameObject attackTrigger)
    {
        isReady = false;
        yield return new WaitForSeconds(coolDownTime);
        Destroy(attackTrigger);
        isReady = true;
    }
}