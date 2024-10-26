using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public Weapon weaponSource; // Weapon ref used by Damageable objects to calculate OnDamage

    public void SetDamageSource(Weapon weapon)
    {
        weaponSource = weapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        // Ignore the weapon holder and the weapon itself
        if (weaponSource == null || other.gameObject.transform.root == weaponSource.WeaponHolder.gameObject.transform.root)
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