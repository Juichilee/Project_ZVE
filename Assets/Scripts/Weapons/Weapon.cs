using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public abstract string WeaponName { get; set; }
    public abstract DamageData DamageAttributes { get; protected set; }
    public abstract float CoolDownTime { get; protected set; }
    public bool IsReady { get; set; } = true;
    public IWeaponHolder WeaponHolder { get; set; } 
    public Animator WeaponHolderAnim { get; set; } // Used to update weapon holder's animation for the weapon
    public abstract int WeaponAnimId { get; protected set; } // Used to determine the specific animation type appropriate for the weapon
    public abstract WeaponType WeaponType { get; protected set; }
    public abstract HoldParentType HoldParentType { get; protected set; }
    public abstract Transform Hold { get; } // Hold is used to tweak weapon position in weapon holder's hand
    public abstract void Attack();
    public abstract void SpawnDamageObject();
    public abstract void SetHoldConfigs(Transform holdParent);
}

public enum WeaponType
{
    Melee,
    Ranged,
    Ability
}

public enum HoldParentType
{
    RightHand,
    LeftHand,
    Head
}