using UnityEngine;
using System.Collections;

public class RangedWeapon : BaseWeapon
{
    // Weapon References
    public GameObject projectileObj;
    public Transform shootPos;
    public override WeaponType WeaponType => WeaponType.Ranged;
    public override int weaponAnimId => 1;

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
    public int maxClip;
    public int currentAmmo;
    public int currentClip;
    private float effectiveRange;
    private Vector3 targetPosition;
    private Vector3 aimDir;
    public LayerMask aimColliderLayerMask;

    void OnEnable()
    {
        isReady = true; // Reset isReady when re-enabled
    }
    public override void Attack()
    {
        if (currentClip > 0)
        {
            // Attack logic
            FireWeapon();
            currentClip--;
            StartCoroutine(AttackCooldown());
        }
    }

    public void Reload()
    {
        if (currentAmmo <= 0)
        {
            return;
        }
        Debug.Log($"Reloading {weaponName}");

        int spentClip = maxClip - currentClip; // How many bullets were spent in the clip
        if (spentClip <= currentAmmo) // Full reload if enough ammo
        {
            currentAmmo -= spentClip;
            currentClip += spentClip;
        } else {
            currentClip = currentAmmo;
        }
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