using UnityEngine;
using System.Collections;

public class MeleeWeapon : Weapon
{
    [SerializeField] private DamageData damageAttributes;
    [SerializeField] private float coolDownTime = 1f;
    [SerializeField] private int weaponAnimId = 0;
    [SerializeField] private WeaponType weaponType = WeaponType.Melee;
    [SerializeField] private Vector3 holdPosition;
    [SerializeField] private Vector3 holdRotation;
    [SerializeField] private DamageObject hitBoxInstance; // Should have DamageObject component
    [SerializeField] private AudioSource audioSource;

    public AudioClip meleeSoundClip;

    #region Accessors
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

    void Start(){
        hitBoxInstance.SetDamageSource(this);
        hitBoxInstance.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void Attack()
    {
        Debug.Log($"Melee attack with {WeaponName}");
        WeaponHolderAnim.SetTrigger("attack1");
        SpawnDamageObject();
        StartCoroutine(AttackCooldown());
        
        if (meleeSoundClip != null)
        {
            audioSource.PlayOneShot(meleeSoundClip);
        }
    }

    private IEnumerator ActivateHitbox(DamageObject hitBoxInstance)
    {
        // Activate the hitbox for a short duration to simulate an attack
        // hitBoxInstance.transform.position = this.transform.position + transform.forward * 1.5f; // Adjust as needed
        hitBoxInstance.gameObject.SetActive(true);

        yield return new WaitForSeconds(1.0f); // Duration of the active hitbox

        hitBoxInstance.gameObject.SetActive(false);
    }

    private IEnumerator AttackCooldown()
    {
        IsReady = false;
        yield return new WaitForSeconds(CoolDownTime);
        IsReady = true;
    }

    public override void SpawnDamageObject()
    {
        StartCoroutine(ActivateHitbox(hitBoxInstance));
    }
}