using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoulderFistWeapon : MeleeWeapon, Ability
{
    [SerializeField] private string boulderFistName = "boulderFist";
    [SerializeField] private float boulderFistCoolDownTime = 1f;
    [SerializeField] private int boulderFistWeaponAnimId = 5;
    [SerializeField] private string boulderFistHoldConfigName = "HoldConfig_boulderFist";
    // abilityReady tracks when the player can use this ability again regardless of animation state
    // Not to be confused with IsReady, which tracks when the player can use a weapon after its animation is done playing
    [SerializeField] 
    private bool abilityReady = true;
    [SerializeField] 
    private WeaponType boulderFistweaponType = WeaponType.Ability;

    [SerializeField]
    private GameObject secondHandWeapon;
    [SerializeField]
    private DamageObject secondHandHitBoxInstance;

    public bool IsAbilityReady { get { return abilityReady; } set { abilityReady = value; } }

    public void Awake()
    {
        // Setup attributes for this weapon
        weaponName = boulderFistName;
        coolDownTime = boulderFistCoolDownTime;
        weaponAnimId = boulderFistWeaponAnimId;
        holdConfigName = boulderFistHoldConfigName;
        weaponType = boulderFistweaponType;
    }

    public override void OnEnable()
    {
        secondHandWeapon.SetActive(true);
    }

    public override void OnDisable()
    {
        base.OnDisable();
        secondHandWeapon.SetActive(false);
    }

    public override void Attack()
    {
        if(IsReady)
        {
            // Read in combo input if the player is already attacking
            WeaponHolderAnim.SetTrigger("attack");
        }
    }

    public override void PlayMeleeSound()
    {
        if (meleeSoundClip != null)
        {
            audioSource.PlayOneShot(meleeSoundClip);
        }
    }

    private Coroutine activateHitBox;
    private Coroutine secondActivateHitBox;
    public override void SpawnDamageObject()
    {
        hitBoxInstance.SetDamageSource(this);
        activateHitBox = StartCoroutine(ActivateHitbox(hitBoxInstance));
        secondHandHitBoxInstance.SetDamageSource(this);
        secondActivateHitBox = StartCoroutine(ActivateHitbox(secondHandHitBoxInstance));
    }
}
