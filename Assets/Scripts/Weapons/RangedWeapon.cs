using UnityEngine;
using System.Collections;

public class RangedWeapon : Weapon
{
    [SerializeField] private string weaponName;
    [SerializeField] private DamageData damageAttributes;
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
    [SerializeField] private Vector3 holdPosition;
    [SerializeField] private Vector3 holdRotation;
    [SerializeField] private AudioSource audioSource;
    private Vector3 aimDir;
    public AudioClip gunshot;
    public AudioClip gunClick;
    private bool hasPlayedGunReady = false;
    public string SetWeaponName;

    void Start()
    {
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        WeaponName = SetWeaponName;

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
    public override Vector3 HoldPosition { get => holdPosition; }
    public override Vector3 HoldRotation { get => holdRotation; } 
    #endregion

    void OnEnable()
    {
        IsReady = true; // Reset IsReady when re-enabled
    }

    public override void Attack()
    {
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
        Debug.Log($"Reloading {WeaponName}");

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
        GameObject projInst = Instantiate(ProjectileObj, ShootPos.position, Quaternion.LookRotation(aimDir, Vector3.up));
        projInst.GetComponent<DamageObject>().SetDamageSource(this);
    }

    public void ResetGunReadySound()
    {
        hasPlayedGunReady = false;
    }
}