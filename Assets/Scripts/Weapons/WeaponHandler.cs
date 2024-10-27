using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
[RequireComponent(typeof(PlayerControlScript))]
public class WeaponHandler : MonoBehaviour, IWeaponHolder
{
    public Transform holdWeaponParent;
    public Transform aimTarget;
    [SerializeField]
    private Weapon[] weaponSlots = new Weapon[3]; // Fixed array to hold 3 weapons
    private int currentWeaponIndex = 0;
    private Collider currentPickupCollider;
    private PlayerControlScript playerControlScript;
    public Rig aimRig;
    public TextMeshProUGUI ammoCountText;
    private GameObject pickupGuide;
    void Awake()
    {
        playerControlScript = GetComponent<PlayerControlScript>();
    }
    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ammoCountText = GameObject.Find("AmmoCount").GetComponent<TextMeshProUGUI>();
        pickupGuide = GameObject.FindGameObjectWithTag("PickupPanel");
    }

    void Update()
    {
        HandlePickupTrigger();
        HandleWeaponSwitching();
        HandleWeaponInput();
        HandleWeaponDrop();
        pickupGuide.SetActive(currentPickupCollider != null);

        if (GetCurrentWeapon() is RangedWeapon rangedWeapon)
        {
            string ammoString = $"{rangedWeapon.CurrentClip}/{rangedWeapon.MaxClip}({rangedWeapon.CurrentAmmo})";
            ammoCountText.text = ammoString;
        }
        if (GetCurrentWeapon() is MeleeWeapon meleeWeapon)
        {
            string ammoString = $"1/1(1)";
            ammoCountText.text = ammoString;
        }

        if (GetCurrentWeapon() is null)
        {
            string ammoString = $"0/0(0)";
            ammoCountText.text = ammoString;
        }
    }

    public Transform GetWeaponHolderRootTransform()
    {
        return this.transform.root;
    }

    Weapon GetCurrentWeapon()
    {
        return weaponSlots[currentWeaponIndex];
    }

    private void HandlePickupTrigger()
    {
        if (currentPickupCollider != null && playerControlScript._interact)
        {
            Weapon weapon = currentPickupCollider.GetComponent<Weapon>();
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

        Weapon currentWeapon = GetCurrentWeapon();
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

        if (currentWeapon.IsReady && playerControlScript._inputAttack)
        {
            currentWeapon.Attack();
        }
    }

    private void HandleWeaponDrop()
    {
        Weapon currentWeapon = GetCurrentWeapon();
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

        Weapon nextWeapon = weaponSlots[index];
        // Activate the weapon at the new index
        if (nextWeapon != null)
        {
            nextWeapon.gameObject.SetActive(true);
            playerControlScript.anim.SetInteger("weaponAnimId", nextWeapon.WeaponAnimId); // Update the animator's weaponAnimId to change weapon anim configs
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
            Weapon weapon = weaponSlots[index];
            weapon.WeaponHolder = null;
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

    private void PickUpWeapon(Weapon weapon)
    {
        if (GetCurrentWeapon() != null)
        {
            DropWeapon(currentWeaponIndex);
        }

        GameObject weaponGameObject = weapon.gameObject;
        weaponSlots[currentWeaponIndex] = weapon;

        // Update weapon references to current holder
        weapon.WeaponHolder = this;
        weapon.WeaponHolderAnim = playerControlScript.anim;
        weaponGameObject.transform.SetParent(holdWeaponParent);
        weaponGameObject.transform.localPosition = weapon.HoldPosition;
        weaponGameObject.transform.localRotation = Quaternion.Euler(weapon.HoldRotation);
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
        if (other.GetComponent<Weapon>() != null)
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
