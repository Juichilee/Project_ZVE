using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PiercingYellWeapon : MeleeWeapon, Ability
{
    [SerializeField] private string piercingYellName = "piercingYell";
    [SerializeField] private float piercingYellCoolDownTime = 1f;
    [SerializeField] private int piercingYellWeaponAnimId = 6;
    [SerializeField] private string piercingYellHoldConfigName = "HoldConfig_piercingYell";
    // abilityReady tracks when the player can use this ability again regardless of animation state
    // Not to be confused with IsReady, which tracks when the player can use a weapon after its animation is done playing
    [SerializeField] private bool abilityReady = true; 
    private HoldParentType piercingYellHoldParentType = HoldParentType.Head;
    private WeaponType piercingYellweaponType = WeaponType.Ability;
    public bool IsAbilityReady { get { return abilityReady; } set { abilityReady = value; } }

    public void Awake()
    {
        // Setup attributes for this weapon
        weaponName = piercingYellName;
        coolDownTime = piercingYellCoolDownTime;
        weaponAnimId = piercingYellWeaponAnimId;
        holdConfigName = piercingYellHoldConfigName;
        weaponType = piercingYellweaponType;
        holdParentType = piercingYellHoldParentType;
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
}
