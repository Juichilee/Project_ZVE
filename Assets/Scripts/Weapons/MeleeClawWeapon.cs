using UnityEngine;
using System.Collections;

public class MeleeClawWeapon : Weapon
{
    [SerializeField] private string weaponName;
    [SerializeField] private DamageData damageAttributes;
    [SerializeField] private float coolDownTime = 1f;
    [SerializeField] private int weaponAnimId = 0;
    [SerializeField] private WeaponType weaponType = WeaponType.Melee;
    [SerializeField] private Vector3 holdPosition;
    [SerializeField] private Vector3 holdRotation;
    [SerializeField] private DamageObject hitBoxInstance; // Should have DamageObject component
    [SerializeField] private AudioSource audioSource;
    
    public string SetWeaponName;

    public AudioClip meleeSoundClip;

    #region Accessors
    public override string WeaponName { get => weaponName; set => weaponName = value; }
    public override DamageData DamageAttributes { get => damageAttributes; protected set => damageAttributes = value; }
    public override float CoolDownTime { get => coolDownTime; protected set => coolDownTime = value; }
    public override int WeaponAnimId { get => weaponAnimId; protected set => weaponAnimId = value; }
    public override WeaponType WeaponType { get => weaponType; protected set => weaponType = value; }
    public override Vector3 HoldPosition { get => holdPosition; }
    public override Vector3 HoldRotation { get => holdRotation; } 
    #endregion
    
    void OnEnable()
    {
        IsReady = true; // Reset IsReady when re-enabled
    }

    void OnDisable()
    {
        hitBoxInstance.gameObject.SetActive(false); // Reset hitbox after unequipped
    }

    void Start(){
        hitBoxInstance.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        WeaponName = "Claw";
    }

    public override void Attack()
    {
        if (IsReady) 
        {
            Debug.Log($"Melee attack with {WeaponName}");
            SpawnDamageObject();
            StartCoroutine(AttackCooldown());
        }
        
        if (meleeSoundClip != null)
        {
            audioSource.PlayOneShot(meleeSoundClip);
        }
    }

    public override void SpawnDamageObject()
    {
        hitBoxInstance.SetDamageSource(this);
    }

    public void EnableHitbox()
    {
        hitBoxInstance.gameObject.SetActive(true);
    }

    public void DisableHitbox()
    {
        hitBoxInstance.gameObject.SetActive(false);
    }

    private IEnumerator AttackCooldown()
    {
        IsReady = false;
        yield return new WaitForSeconds(CoolDownTime);
        IsReady = true;
    }
}