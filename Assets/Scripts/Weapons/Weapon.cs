using UnityEngine;

public abstract class Weapon : MonoBehaviour
{
    public string WeaponName { get; set; }
    public abstract DamageData DamageAttributes { get; protected set; }
    public abstract float CoolDownTime { get; protected set; }
    public bool IsReady { get; set; } = true;
    public WeaponHandler WeaponHolder { get; set; } 
    public Animator WeaponHolderAnim { get; set; } // Used to update weapon holder's animation for the weapon
    public abstract int WeaponAnimId { get; protected set; } // Used to determine the specific animation type appropriate for the weapon
    public abstract WeaponType WeaponType { get; protected set; }
    // holdPosition and holdRotation are used to tweak the weapon's pos/rot when held
    public abstract Vector3 HoldPosition { get; }
    public abstract Vector3 HoldRotation { get; }
    public abstract void Attack();
    public abstract void SpawnDamageObject();
}

public enum WeaponType
{
    Melee,
    Ranged,
}