using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerControlScript))]
public class WeaponHandler : MonoBehaviour, IWeaponHolder
{
    public Transform holdWeaponParent;
    [SerializeField]
    private Weapon[] weaponSlots = new Weapon[3];
    private int currentWeaponIndex = 0;
    private Collider currentPickupCollider;
    private PlayerControlScript playerControlScript;
    private ThirdPersonCamera thirdPersonCamera;
    public Rig aimRig;
    public TextMeshProUGUI ammoCountText;
    private GameObject pickupGuide;

    private Coroutine weaponsAtReadyCoroutine;

    void Awake()
    {
        playerControlScript = GetComponent<PlayerControlScript>();
        thirdPersonCamera = GetComponent<ThirdPersonCamera>();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        GameObject ammoCountGameObject = GameObject.Find("AmmoCount");
        if (ammoCountGameObject)
        {
            ammoCountText = ammoCountGameObject.GetComponent<TextMeshProUGUI>();
        }
        pickupGuide = GameObject.FindGameObjectWithTag("PickupPanel");
    }

    void Update()
    {
        HandlePickupTrigger();
        HandleWeaponSwitching();
        HandleWeaponInput();
        HandleWeaponDrop();
        pickupGuide.SetActive(currentPickupCollider != null);
        UpdateAmmoCountDisplay();
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
        if (currentPickupCollider != null && playerControlScript.Interact)
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
                EquipWeapon(i);
            }
        }
    }

    private void HandleWeaponInput()
    {
        Weapon currentWeapon = GetCurrentWeapon();
        if (currentWeapon == null)
        {
            thirdPersonCamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Basic);
            aimRig.weight = 0f;
            // Reset all combat layers
            playerControlScript.Anim.SetLayerWeight(2, 0);
            playerControlScript.Anim.SetLayerWeight(1, 0);
            // UpdateAimRigWeight(0f); // Reset aim rig when no weapon equipped
            return;
        }

        if (currentWeapon is RangedWeapon)
        {
            HandleRangedWeaponInput((RangedWeapon)currentWeapon);
        }
        else if (currentWeapon is MeleeWeapon)
        {
            HandleMeleeWeaponInput((MeleeWeapon)currentWeapon);
        }
    }

    private void HandleRangedWeaponInput(RangedWeapon rangedWeapon)
    {
        thirdPersonCamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Ranged);
        playerControlScript.Anim.SetLayerWeight(2, 0); // Reset melee layer
        playerControlScript.Anim.SetLayerWeight(1, 1);

        aimRig.weight = 1f;

        // Weapons At Ready state keeps player ranged weapon at the ready after aiming or firing
        if (playerControlScript.InputAimDown || playerControlScript.InputAttack)
        {
            if (weaponsAtReadyCoroutine != null)
            {
                StopCoroutine(weaponsAtReadyCoroutine);
            }
            weaponsAtReadyCoroutine = StartCoroutine(WeaponsAtReady(1f));
        }

        MultiAimConstraint aim = null;
        MultiAimConstraint bodyAim = null;

        // Retrieve the list of multi-aimconstraint source objects for current type of ranged weapon
        if (rangedWeapon.WeaponName == "Pistol")
        {
            Transform pistolRig = aimRig.transform.Find("PistolRig");
            aim = pistolRig.Find("Aim").gameObject.GetComponent<MultiAimConstraint>();
            bodyAim = pistolRig.Find("BodyAim").gameObject.GetComponent<MultiAimConstraint>();
        }

        // Switch between weapon ready and weapon idle states by switching Aim and BodyAim source blend weights
        if (playerControlScript.Anim.GetBool("weaponsAtReady"))
        {
            PlayerControlScript.SetMultiAimSourceWeight(aim, 0, 0f);
            PlayerControlScript.SetMultiAimSourceWeight(aim, 1, 1f);
            PlayerControlScript.SetMultiAimSourceWeight(bodyAim, 0, 0f);
            PlayerControlScript.SetMultiAimSourceWeight(bodyAim, 1, 1f);
        } else {
            PlayerControlScript.SetMultiAimSourceWeight(aim, 0, 1f);
            PlayerControlScript.SetMultiAimSourceWeight(aim, 1, 0f);
            PlayerControlScript.SetMultiAimSourceWeight(bodyAim, 0, 1f);
            PlayerControlScript.SetMultiAimSourceWeight(bodyAim, 1, 0f);
        }

        // Updated ranged fov when aiming down
        thirdPersonCamera.UpdateRangedCameraFOV(playerControlScript.InputAimDown);

        // Player reload input to ranged weapon
        if (playerControlScript.Reload)
        {
            rangedWeapon.Reload();
        }

        if (rangedWeapon.IsReady && playerControlScript.InputAttack)
        {
            playerControlScript.Anim.SetTrigger("attack");
            rangedWeapon.Attack();
        }

        rangedWeapon.UpdateWeaponAim(playerControlScript.aimTarget);
    }

    // private void UpdateAimRigWeight(float targetWeight)
    // {
    //     aimRig.weight = Mathf.Lerp(aimRig.weight, targetWeight, Time.deltaTime * 25f);
    // }

    private IEnumerator WeaponsAtReady(float seconds)
    {
        // aimRig.weight = 1f;
        playerControlScript.ForceStrafe = true;
        playerControlScript.Anim.SetBool("weaponsAtReady", true);
        yield return new WaitForSeconds(seconds);
        playerControlScript.Anim.SetBool("weaponsAtReady", false);
        playerControlScript.ForceStrafe = false;
        // UpdateAimRigWeight(0f);
    }

    private void HandleMeleeWeaponInput(MeleeWeapon meleeWeapon)
    {
        thirdPersonCamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Basic);
        playerControlScript.Anim.SetLayerWeight(2, 1);
        playerControlScript.Anim.SetLayerWeight(1, 0); // Reset ranged layer
        aimRig.weight = 0f;

        if (meleeWeapon.IsReady && playerControlScript.InputAttack)
        {
            playerControlScript.Anim.SetTrigger("attack");
            meleeWeapon.Attack();
        }
    }

    private void HandleWeaponDrop()
    {
        Weapon currentWeapon = GetCurrentWeapon();
        if (playerControlScript.Drop && currentWeapon != null)
        {
            DropWeapon(currentWeaponIndex);
        }
    }

    private void UpdateAmmoCountDisplay()
    {
        Weapon currentWeapon = GetCurrentWeapon();
        if (currentWeapon is RangedWeapon rangedWeapon)
        {
            ammoCountText.text = $"{rangedWeapon.CurrentClip}/{rangedWeapon.MaxClip}({rangedWeapon.CurrentAmmo})";
        }
        else if (currentWeapon is MeleeWeapon)
        {
            ammoCountText.text = "1/1(1)";
        }
        else
        {
            ammoCountText.text = "0/0(0)";
        }
    }

    private void EquipWeapon(int index)
    {
        if (index < 0 || index >= weaponSlots.Length)
        {
            Debug.LogError("EquipWeapon() index is not within the range of weaponSlots array");
            return;
        }

        if (GetCurrentWeapon() != null)
        {
            DeEquipWeapon(currentWeaponIndex);
        }

        Weapon nextWeapon = weaponSlots[index];
        if (nextWeapon != null)
        {
            nextWeapon.gameObject.SetActive(true);
            playerControlScript.Anim.SetInteger("weaponAnimId", nextWeapon.WeaponAnimId);
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
            SceneManager.MoveGameObjectToScene(weapon.gameObject, SceneManager.GetActiveScene());

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

        weapon.WeaponHolder = this;
        weapon.WeaponHolderAnim = playerControlScript.Anim;
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
            collider.enabled = false;
        }
        currentPickupCollider = null;
        EquipWeapon(currentWeaponIndex);
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
