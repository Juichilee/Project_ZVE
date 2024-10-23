using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public string weaponName;
    public int damage;
    public abstract float coolDownTime { get; }
    public bool isReady = true;
    public GameObject weaponHolder; 
    public Animator weaponHolderAnim; // Used to update weapon holder's animation for the weapon
    public abstract int weaponAnimId { get; } // Used to determine the specific animation type appropriate for the weapon
    public abstract WeaponType WeaponType { get; }
    // holdPosition and holdRotation are used to tweak the weapon's pos/rot when held
    public Vector3 holdPosition;
    public Vector3 holdRotation;
    public abstract void Attack();
}

public enum WeaponType
{
    Melee,
    Ranged,
}