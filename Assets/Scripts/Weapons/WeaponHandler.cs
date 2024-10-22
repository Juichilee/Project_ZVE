using UnityEngine;
using UnityEngine.Animations.Rigging;

[RequireComponent(typeof(PlayerControlScript))]
public class WeaponHandler : MonoBehaviour
{
    public Transform holdWeaponParent;
    public Transform aimTarget;
    [SerializeField]
    private BaseWeapon[] weaponSlots = new BaseWeapon[3]; // Fixed array to hold 3 weapons
    private int currentWeaponIndex = 0;
    private BaseWeapon currentWeapon;
    private Collider currentPickupCollider;
    private PlayerControlScript playerControlScript;
    public Rig aimRig;

    void Awake()
    {
        playerControlScript = GetComponent<PlayerControlScript>();
    }

    void Update()
    {
        HandlePickupTrigger();
        HandleWeaponSwitching();
        HandleWeaponInput();
        HandleWeaponDrop();
    }

    private void HandlePickupTrigger()
    {
        if (currentPickupCollider != null && Input.GetKeyDown(KeyCode.E))
        {
            BaseWeapon weapon = currentPickupCollider.GetComponent<BaseWeapon>();
            if (weapon != null)
            {
                // Add weapon to the first available slot
                for (int i = 0; i < weaponSlots.Length; i++)
                {
                    if (weaponSlots[i] == null)
                    {
                        GameObject weaponGameObject = weapon.gameObject;
                        weaponSlots[i] = weapon;
                        weapon.weaponHolder = gameObject;
                        weaponGameObject.transform.SetParent(holdWeaponParent);
                        weaponGameObject.transform.localPosition = weapon.holdPosition;
                        weaponGameObject.transform.localRotation = Quaternion.Euler(weapon.holdRotation);

                        Rigidbody rb = weaponGameObject.GetComponent<Rigidbody>();
                        if (rb != null)
                        {
                            rb.isKinematic = true;
                            rb.useGravity = true;
                        }

                        Collider collider = weaponGameObject.GetComponent<Collider>();
                        if (collider != null)
                        {
                            collider.isTrigger = true;
                        }

                        EquipWeapon(i);
                        currentPickupCollider = null;
                        break;
                    }
                }
            }
        }
    }

    private void HandleWeaponSwitching()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                EquipWeapon(i);
            }
        }
    }

    private void HandleWeaponInput()
    {
        if (currentWeapon != null)
        {
            playerControlScript.anim.SetLayerWeight(2, 1); // Index 2 is combat layer
            if (playerControlScript._inputAimDown && currentWeapon is RangedWeapon rangedWeapon)
            {
                rangedWeapon.UpdateWeaponAim(ref aimTarget);
                aimRig.weight = Mathf.Lerp(aimRig.weight, 1f, Time.deltaTime * 2f);
            } else {
                aimRig.weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 2f);
            }

            if (playerControlScript._inputAttack)
            {
                currentWeapon.Attack();
            }
        } else {
            playerControlScript.anim.SetLayerWeight(2, 0); // Index 2 is combat layer
        }
    }

    private void HandleWeaponDrop()
    {
        if (Input.GetKeyDown(KeyCode.X) && currentWeapon != null)
        {
            DropWeapon(currentWeaponIndex);
        }
    }

    private void EquipWeapon(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index] != null)
        {
            if (currentWeapon != null)
            {
                DeEquipWeapon(currentWeaponIndex);
            }

            currentWeaponIndex = index;
            currentWeapon = weaponSlots[index];
            currentWeapon.gameObject.SetActive(true);
        }
    }

    private void DeEquipWeapon(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index] != null)
        {
            weaponSlots[index].gameObject.SetActive(false);
        }
    }

    private void DropWeapon(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index] != null)
        {
            BaseWeapon weapon = weaponSlots[index];
            weapon.weaponHolder = null;
            weapon.gameObject.transform.SetParent(null);

            Rigidbody rb = weapon.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = true;
            }

            Collider collider = weapon.gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.isTrigger = false;
            }

            weaponSlots[index] = null;
            currentWeapon = null;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<BaseWeapon>() != null)
        {
            currentPickupCollider = other;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (currentPickupCollider == other)
        {
            currentPickupCollider = null;
        }
    }
}
