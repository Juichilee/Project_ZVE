using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordArmWeapon : MeleeWeapon, Ability
{
    [SerializeField] private string swordArmName = "SwordArm";
    [SerializeField] private float swordArmCoolDownTime = 1f;
    [SerializeField] private int swordArmWeaponAnimId = 4;
    [SerializeField] private string swordArmHoldConfigName = "HoldConfig_swordArm";
    // abilityReady tracks when the player can use this ability again regardless of animation state
    // Not to be confused with IsReady, which tracks when the player can use a weapon after its animation is done playing
    [SerializeField] private bool abilityReady = true; 
    private WeaponType swordArmweaponType = WeaponType.Ability;
    public bool IsAbilityReady { get { return abilityReady; } set { abilityReady = value; } }

    public void Awake()
    {
        // Setup attributes for this weapon
        weaponName = swordArmName;
        coolDownTime = swordArmCoolDownTime;
        weaponAnimId = swordArmWeaponAnimId;
        holdConfigName = swordArmHoldConfigName;
        weaponType = swordArmweaponType;
    }

    public override void Attack()
    {
        if(IsReady)
        {
            // Read in combo input if the player is already attacking
            PlayMeleeSound(); // sound won't attach itself to animation, temp fix
            WeaponHolderAnim.SetTrigger("attack");
        }
    }
    #region sound
    public override void PlayMeleeSound()
    {
        if (meleeSoundClip != null)
        {
            audioSource.PlayOneShot(meleeSoundClip);
        }
    }

    #endregion
}
