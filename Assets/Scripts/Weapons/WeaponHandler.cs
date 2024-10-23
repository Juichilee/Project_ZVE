using Cinemachine;
using TMPro;
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
    private Collider currentPickupCollider;
    private PlayerControlScript playerControlScript;
    public Rig aimRig;
    public TextMeshProUGUI ammoCountText;

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

        if (GetCurrentWeapon() is RangedWeapon rangedWeapon)
        {
            string ammoString = $"{rangedWeapon.currentClip}/{rangedWeapon.maxClip}({rangedWeapon.currentAmmo})";
            ammoCountText.text = ammoString;
        }
    }

    BaseWeapon GetCurrentWeapon()
    {
        return weaponSlots[currentWeaponIndex];
    }

    private void HandlePickupTrigger()
    {
        if (currentPickupCollider != null && playerControlScript._interact)
        {
            BaseWeapon weapon = currentPickupCollider.GetComponent<BaseWeapon>();
            PickUpWeapon(weapon);
        }
    }

    private void HandleWeaponSwitching()
    {
        for (int i = 0; i < weaponSlots.Length; i++)
        {
            if (Input.GetKeyDown((KeyCode)((int)KeyCode.Alpha1 + i)))
            {
                Debug.Log("EQUIPPED WEAPON SLOT: " + i);
                EquipWeapon(i);
            }
        }
    }

    private void HandleWeaponInput()
    {
        // Reset all combat layer weights
        playerControlScript.anim.SetLayerWeight(2, 0); // Index 2 is ranged combat layer
        playerControlScript.anim.SetLayerWeight(3, 0); // Index 3 is melee combat layer

        BaseWeapon currentWeapon = GetCurrentWeapon();
        if (currentWeapon == null)
        {
            // Reset aimRig weight if no weapon equipped
            aimRig.weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 2f);
            return;
        }

        if (currentWeapon is RangedWeapon rangedWeapon){

            playerControlScript.anim.SetLayerWeight(2, 1);

            if (playerControlScript._inputAimDown)
            {
                aimRig.weight = Mathf.Lerp(aimRig.weight, 1f, Time.deltaTime * 2f);
            } else {
                aimRig.weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 2f);
            }

            if(playerControlScript._reload)
            {
                rangedWeapon.Reload();
            }
            rangedWeapon.UpdateWeaponAim(ref aimTarget);
        }

        if (currentWeapon is MeleeWeapon meleeWeapon){
            aimRig.weight = 0f; // Reset aimRig weight immediately
            playerControlScript.anim.SetLayerWeight(3, 1);
        }

        if (currentWeapon.isReady && playerControlScript._inputAttack)
        {
            currentWeapon.Attack();
        }
    }

    private void HandleWeaponDrop()
    {
        BaseWeapon currentWeapon = GetCurrentWeapon();
        if (playerControlScript._drop && currentWeapon != null)
        {
            DropWeapon(currentWeaponIndex);
        }
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponSlots.Length)
        {
            Debug.LogError("EquipWeapon() index is not within the range of weaponSlots array");
        }

        if (GetCurrentWeapon() != null)
        {
            DeEquipWeapon(currentWeaponIndex);
        }

        BaseWeapon nextWeapon = weaponSlots[index];
        // Activate the weapon at the new index
        if (nextWeapon != null)
        {
            nextWeapon.gameObject.SetActive(true);
            playerControlScript.anim.SetInteger("weaponAnimId", nextWeapon.weaponAnimId); // Update the animator's weaponAnimId to change weapon anim configs
        }
        currentWeaponIndex = index;
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
                collider.enabled = true;
            }

            weaponSlots[index] = null;
        }
    }

    private void PickUpWeapon(BaseWeapon weapon)
    {
        if (GetCurrentWeapon() != null)
        {
            DropWeapon(currentWeaponIndex);
        }

        GameObject weaponGameObject = weapon.gameObject;
        weaponSlots[currentWeaponIndex] = weapon;

        // Update weapon references to current holder
        weapon.weaponHolder = gameObject;
        weapon.weaponHolderAnim = playerControlScript.anim;
        weaponGameObject.transform.SetParent(holdWeaponParent);
        weaponGameObject.transform.localPosition = weapon.holdPosition;
        weaponGameObject.transform.localRotation = Quaternion.Euler(weapon.holdRotation);
        weaponGameObject.transform.localScale = Vector3.one;

        Rigidbody rb = weaponGameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = true;
        }

        Collider collider = weaponGameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false; // Turn off collider to prevent detection by triggers
        }
        currentPickupCollider = null;
        EquipWeapon(currentWeaponIndex); // Used to update anim parameters
    }

    private void OnTriggerEnter(Collider other)
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
