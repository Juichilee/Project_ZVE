using UnityEngine;
using System.Collections;

public abstract class MeleeWeapon : Weapon
{
    protected string weaponName;
    protected float coolDownTime = 1f;
    protected int weaponAnimId = 0;
    protected string holdConfigName;
    protected Transform hold;
    [SerializeField] protected float hitBoxDuration = 0.1f;
    [SerializeField] protected WeaponType weaponType = WeaponType.Melee;
    [SerializeField] protected DamageData damageAttributes;
    [SerializeField] protected DamageObject hitBoxInstance; // Should have DamageObject component
    [SerializeField] protected AudioSource audioSource;
    public AudioClip meleeSoundClip;

    #region Accessors
    public override string WeaponName { get => weaponName; set => weaponName = value; }
    public override DamageData DamageAttributes { get => damageAttributes; protected set => damageAttributes = value; }
    public override float CoolDownTime { get => coolDownTime; protected set => coolDownTime = value; }
    public override int WeaponAnimId { get => weaponAnimId; protected set => weaponAnimId = value; }
    public override WeaponType WeaponType { get => weaponType; protected set => weaponType = value; }
    public override Transform Hold { get => hold; }
    #endregion

    public virtual void OnDisable()
    {
        IsReady = true; // Reset IsReady when re-enabled
        hitBoxInstance.gameObject.SetActive(false); // Reset hitbox after unequipped
        // To disable or reset the attack trigger
        if (WeaponHolderAnim)
        {
            WeaponHolderAnim.ResetTrigger("attack");
        }
    }

    public virtual void Start(){
        hitBoxInstance.gameObject.SetActive(false);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
    }

    public override void SetHoldConfigs(Transform holdParent)
    {
        hold = holdParent.Find(holdConfigName).Find("Hold");
    }

    #region Event and Animation Event Callback Handling
    public override void SpawnDamageObject()
    {
        hitBoxInstance.SetDamageSource(this);
        StartCoroutine(ActivateHitbox(hitBoxInstance));
    }

    private IEnumerator ActivateHitbox(DamageObject hitBoxInstance)
    {
        // Activate the hitbox for a short duration to simulate an attack
        hitBoxInstance.gameObject.SetActive(true);

        yield return new WaitForSeconds(hitBoxDuration); // Duration of the active hitbox

        hitBoxInstance.gameObject.SetActive(false);
    }

    public abstract void PlayMeleeSound();
    #endregion
}