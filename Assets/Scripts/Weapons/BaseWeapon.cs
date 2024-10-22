using UnityEngine;

public abstract class BaseWeapon : MonoBehaviour
{
    public string weaponName;
    public int damage;
    public abstract float coolDownTime { get; }
    protected bool isReady = true;
    public GameObject weaponHolder;
    public abstract WeaponType WeaponType { get; }
    // Used to tweak the weapon's position when held
    public Vector3 holdPosition;
    public Vector3 holdRotation;
    public abstract void Attack();
}

public enum WeaponType
{
    Melee,
    Ranged,
}