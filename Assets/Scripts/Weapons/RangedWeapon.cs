using UnityEngine;
using System.Collections;

public class RangedWeapon : BaseWeapon
{
    // Weapon References
    public GameObject projectileObj;
    public Transform shootPos;
    public override WeaponType WeaponType => WeaponType.Ranged;

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
    public int maxAmmo;
    public int currentAmmo;
    private float effectiveRange;
    private Vector3 targetPosition;
    private Vector3 aimDir;
    public LayerMask aimColliderLayerMask;

    public override void Attack()
    {
        if (isReady && currentAmmo > 0)
        {
            // Attack logic
            Debug.Log($"Ranged attack with {weaponName}");
            FireWeapon();
            currentAmmo--;
            StartCoroutine(AttackCooldown());
        }
    }

    public void Reload()
    {
        Debug.Log($"Reloading {weaponName}");
        currentAmmo = maxAmmo;
    }

    private IEnumerator AttackCooldown()
    {
        isReady = false;
        yield return new WaitForSeconds(coolDownTime);
        isReady = true;
    }

    // Calculates where the ranged weapon is aiming using screen raycast
    // aimTarget is a reference to AimTarget GameObject transform that the character rig uses to aim using procedural animation
    public void UpdateWeaponAim(ref Transform aimTarget)
    {
        if (weaponHolder == null) return;

        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        int fixedDistance = 99;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, fixedDistance, aimColliderLayerMask))
        {
            targetPosition = raycastHit.point;
        }
        else
        {
            targetPosition = ray.origin + ray.direction * fixedDistance;
        }
        aimDir = (targetPosition - shootPos.position).normalized;
        aimTarget.position = targetPosition; // update aimTarget position
    }

    private void FireWeapon()
    {
        // playerSounds.Shots();
        GameObject projInst = Instantiate(projectileObj, shootPos.position, Quaternion.LookRotation(aimDir, Vector3.up));
        projInst.GetComponent<Projectile>().SetShooter(weaponHolder);
    }
}