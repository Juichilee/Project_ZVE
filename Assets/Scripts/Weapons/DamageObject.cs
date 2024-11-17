using UnityEngine;

public class DamageObject : MonoBehaviour
{
    public Weapon weaponSource; // Weapon ref used by Damageable objects to calculate OnDamage
    public Transform attacker;
    public LayerMask damageLayer;

    public void SetDamageSource(Weapon weapon)
    {
        weaponSource = weapon;
        attacker = weaponSource.WeaponHolder?.GetWeaponHolderRootTransform();
    }

    private void OnTriggerEnter(Collider other)
    {
        // Convert the collided layer to a bitmask
        int objLayerMask = 1 << other.gameObject.layer;
    
        // Perform bitwise AND between the LayerMask and the object's layer mask
        bool isDamageable = (damageLayer.value & objLayerMask) != 0;

        // Ignore the weapon holder and the weapon itself
        if (weaponSource == null || other.gameObject.transform.root == attacker || !isDamageable)
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