using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public Weapon weaponSource; // Weapon ref used by Damageable objects to calculate OnDamage
    public Transform attacker;

    public void SetDamageSource(Weapon weapon)
    {
        weaponSource = weapon;
        attacker = weaponSource.WeaponHolder?.GetWeaponHolderRootTransform();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore the weapon holder and the weapon itself
        if (weaponSource == null || other.gameObject.transform.root == attacker)
        {
            return;
        }

        Damageable damageable = other.GetComponent<Damageable>();
        if (damageable != null && weaponSource != null)
        {
            damageable.OnDamage(weaponSource.DamageAttributes);
        }

        if (weaponSource is RangedWeapon)
        {
            // Destroy the damage object after collision if needed
            Destroy(gameObject);
        }
    }
}