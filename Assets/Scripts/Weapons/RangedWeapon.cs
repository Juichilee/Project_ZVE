using UnityEngine;
using System;
using System.Collections;

public class RangedWeapon : Weapon
{
    [SerializeField] private bool hitScan;
    [SerializeField] private string weaponName;
    [SerializeField] private DamageData damageAttributes;
    public LayerMask hitLayer;
    [SerializeField] private float coolDownTime = 1f;
    [SerializeField] private int weaponAnimId = 1;
    [SerializeField] private WeaponType weaponType = WeaponType.Ranged;
    [SerializeField] private GameObject projectileObj;
    [SerializeField] private Transform shootPos;
    [SerializeField] private int maxAmmo;
    [SerializeField] private int maxClip;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int currentClip;
    [SerializeField] private float effectiveRange;
    [SerializeField] private string holdConfigName;
    [SerializeField] private Transform hold;
    [SerializeField] private Transform secondHandTarget;
    [SerializeField] private Transform secondHandHint;
    [SerializeField] private AudioSource audioSource;


    private Vector3 aimDir;
    public AudioClip gunshot;
    public AudioClip gunClick;
    private bool hasPlayedGunReady = false;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    #region Accessors
    public override string WeaponName { get => weaponName; set => weaponName = value; }
    public override DamageData DamageAttributes { get => damageAttributes; protected set => damageAttributes = value; }
    public override float CoolDownTime { get => coolDownTime; protected set => coolDownTime = value; }
    public override int WeaponAnimId { get => weaponAnimId; protected set => weaponAnimId = value; }
    public override WeaponType WeaponType { get => weaponType; protected set => weaponType = value; }
    public GameObject ProjectileObj { get => projectileObj; set => projectileObj = value; }
    public Transform ShootPos { get => shootPos; set => shootPos = value; }
    public int MaxAmmo { get => maxAmmo; set => maxAmmo = value; }
    public int MaxClip { get => maxClip; set => maxClip = value; }
    public int CurrentAmmo { get => currentAmmo; set => currentAmmo = value; }
    public int CurrentClip { get => currentClip; set => currentClip = value; }
    public override Transform Hold { get => hold; }
    public Transform SecondHandTarget { get => secondHandTarget; }
    public Transform SecondHandHint { get => secondHandHint; }
    #endregion

    void OnDisable()
    {
        IsReady = true;
        if (WeaponHolderAnim)
        {
            WeaponHolderAnim.ResetTrigger("attack");
        }
    }

    // Automatically sets the hold configurations
    public override void SetHoldConfigs(Transform holdParent)
    {
        IsReady = true; // Reset IsReady when re-enabled
        hold = holdParent.Find(holdConfigName).Find("Hold");
        secondHandTarget = holdParent.Find(holdConfigName).Find("SecondHandTarget");
        secondHandHint = holdParent.Find(holdConfigName).Find("SecondHandHint");
    }

    public override void Attack()
    {
        WeaponHolderAnim.SetTrigger("attack");
        if (IsReady && CurrentClip > 0)
        {
            FireWeapon();
            CurrentClip--;
            StartCoroutine(AttackCooldown());
        }
    }

    public void Reload()
    {
        if (CurrentAmmo <= 0)
        {
            return;
        }

        int spentClip = MaxClip - CurrentClip; // How many bullets were spent in the clip
        if (spentClip <= CurrentAmmo) // Full reload if enough ammo
        {
            CurrentAmmo -= spentClip;
            CurrentClip += spentClip;
        }
        else
        {
            CurrentClip = CurrentAmmo;
            CurrentAmmo = 0;
        }

        if (!hasPlayedGunReady && gunClick != null)
        {
            audioSource.PlayOneShot(gunClick);
            hasPlayedGunReady = true;
        }
        audioSource.PlayOneShot(gunClick);
    }

    private IEnumerator AttackCooldown()
    {
        IsReady = false;
        yield return new WaitForSeconds(CoolDownTime);
        IsReady = true;
    }

    public void UpdateWeaponAim(Transform aimTarget)
    {
        if (WeaponHolder == null) return;

        aimDir = (aimTarget.position - ShootPos.position).normalized;
    }

    private void FireWeapon()
    {
        SpawnDamageObject();
        
        if (gunshot != null)
        {
            audioSource.PlayOneShot(gunshot);
        }
    }

    public override void SpawnDamageObject()
    {
        // Spawn projectiles
        Instantiate(ProjectileObj, ShootPos.position, Quaternion.LookRotation(aimDir, Vector3.up));

        // Hitscan based damage
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        int fixedDistance = 99;
        Collider damageableCollider = null;
        if (Physics.Raycast(ray, out RaycastHit raycastHit, fixedDistance, hitLayer))
        {
            damageableCollider = raycastHit.collider;
        }

        if (damageableCollider != null)
        {
            Damageable damageable = damageableCollider.GetComponent<Damageable>();
            damageable?.OnDamage(DamageAttributes);
        }
    }

    public void ResetGunReadySound()
    {
        hasPlayedGunReady = false;
    }

    public Boolean gainAmmo(int ammoAmt)
    {
        if (currentAmmo == maxAmmo)
            return false;
        else if (currentAmmo + ammoAmt >= maxAmmo)
            currentAmmo = maxAmmo;
        else
            currentAmmo += ammoAmt;
        return true;
    }

    public int getCurrentClip()
    {
        return currentClip;
    }

    public int getCurrentAmmo()
    {
        return currentAmmo;
    }
}