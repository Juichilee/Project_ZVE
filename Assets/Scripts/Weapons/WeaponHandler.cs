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
    public Transform secondHandAimTarget;
    public Transform secondHandHintTarget;
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

    public Weapon GetWeapon(int index)
    {
        return weaponSlots[index];
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
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
            playerControlScript.Anim.SetLayerWeight(3, 0);
            playerControlScript.Anim.SetLayerWeight(2, 0);
            playerControlScript.Anim.SetInteger("weaponAnimId", -1); // id for unequipped is -1
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

    // Stores references to the current weapon rig contraints
    MultiAimConstraint aim;
    MultiAimConstraint bodyAim;
    TwoBoneIKConstraint secondHandAim;
    private void UpdateWeaponRigConByName(string weaponName){
        Transform weaponRig = aimRig.transform.Find(weaponName); // weapon rig name must match weapon name
        aim = weaponRig.Find("Aim").gameObject.GetComponent<MultiAimConstraint>();
        bodyAim = weaponRig.Find("BodyAim").gameObject.GetComponent<MultiAimConstraint>();
        secondHandAim = weaponRig.Find("SecondHandAim").gameObject.GetComponent<TwoBoneIKConstraint>();
        secondHandAim.data.target = secondHandAimTarget;
        secondHandAim.data.hint = secondHandHintTarget;
    }

    private void HandleRangedWeaponInput(RangedWeapon rangedWeapon)
    {
        thirdPersonCamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Ranged);
        playerControlScript.Anim.SetLayerWeight(3, 0); // Reset melee layer
        playerControlScript.Anim.SetLayerWeight(2, 1);

        aimRig.weight = 1f;

        // Weapons At Ready state keeps player ranged weapon at the ready after aiming or firing
        if (playerControlScript.InputAimDown || playerControlScript.InputHoldAttack)
        {
            if (weaponsAtReadyCoroutine != null)
            {
                StopCoroutine(weaponsAtReadyCoroutine);
            }
            weaponsAtReadyCoroutine = StartCoroutine(WeaponsAtReady(1f));
        }

        // Switch between weapon ready and weapon idle states by switching Aim and BodyAim source blend weights
        if (playerControlScript.Anim.GetBool("weaponsAtReady"))
        {
            PlayerControlScript.SetMultiAimSourceWeight(aim, 0, 0f); // idle aim target weight off
            PlayerControlScript.SetMultiAimSourceWeight(aim, 1, 1f); // aim target weight on
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

        if (rangedWeapon.IsReady && playerControlScript.InputHoldAttack)
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
        playerControlScript.Anim.SetLayerWeight(3, 1);
        playerControlScript.Anim.SetLayerWeight(2, 0); // Reset ranged layer
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

        // Prevents deequipping a weapon just after picking it up
        if (currentWeaponIndex != index && GetCurrentWeapon() != null)
        {
            DeEquipWeapon(currentWeaponIndex);
        }

        Weapon nextWeapon = weaponSlots[index];
        if (nextWeapon != null)
        {
            nextWeapon.gameObject.SetActive(true);
            playerControlScript.Anim.SetInteger("weaponAnimId", nextWeapon.WeaponAnimId);

            nextWeapon.transform.SetLocalPositionAndRotation(nextWeapon.Hold.localPosition, nextWeapon.Hold.localRotation);
            if (nextWeapon is RangedWeapon rangedWeapon)
            {
                // Reposition second hand targets and activate aim weights
                secondHandAimTarget.SetLocalPositionAndRotation(rangedWeapon.SecondHandTarget.localPosition, rangedWeapon.SecondHandTarget.localRotation);
                secondHandHintTarget.localPosition = rangedWeapon.SecondHandHint.localPosition;
                // Updates all weapon rig constraint references to the current weapon
                UpdateWeaponRigConByName(rangedWeapon.WeaponName);
                ActivateWeaponAimWeights(rangedWeapon);
            }
        } else {
            playerControlScript.Anim.SetInteger("weaponAnimId", -1); // id for unequipped is -1
        }
        playerControlScript.Anim.SetTrigger("changeWeapon");

        currentWeaponIndex = index;
    }

    private void ActivateWeaponAimWeights(Weapon weapon)
    {
        if (weapon is RangedWeapon)
        {
            aim.weight = 1f;
            bodyAim.weight = 1f;
        }
    }

    private void DeactivateWeaponAimWeights(Weapon weapon)
    {
        if (weapon is RangedWeapon)
        {
            // Reset prev ranged weapon aim weights so that curr equipped weapon aim is not affected
            aim.weight = 0f;
            bodyAim.weight = 0f;
        }
    }

    private void DeEquipWeapon(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index] != null)
        {
            Weapon prevWeapon = weaponSlots[index];
            DeactivateWeaponAimWeights(prevWeapon);
            weaponSlots[index].gameObject.SetActive(false);
        }
    }

    private void DropWeapon(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index] != null)
        {
            Weapon weapon = weaponSlots[index];
            DeactivateWeaponAimWeights(weapon);
            weapon.WeaponHolder = null;
            weapon.gameObject.transform.SetParent(null);
            SceneManager.MoveGameObjectToScene(weapon.gameObject, SceneManager.GetActiveScene()); // Move outside DoNotDestroyOnLoad scene

            Rigidbody rb = weapon.gameObject.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = false;
                rb.useGravity = false;
            }

            Collider collider = weapon.gameObject.GetComponent<Collider>();
            if (collider != null)
            {
                collider.enabled = true;
            }

            int rotationSetting = 0;

            Spinner spinner = weapon.gameObject.GetComponent<Spinner>();
            if (spinner != null)
            {
                spinner.enabled = true;
                rotationSetting = spinner.type;
            }

            Transform transform = weapon.gameObject.transform;
            for (int i = 0; i < transform.childCount; i++)
            {
                if (transform.GetChild(i).gameObject.tag == "Particle")
                {
                    transform.GetChild(i).gameObject.SetActive(true);
                }
            }

            if (rotationSetting == 0)
                transform.rotation = Quaternion.Euler(new Vector3(0f, 0f, 90f));
            else
                transform.rotation = Quaternion.Euler(new Vector3(0f, 90f, 0f));
            weaponSlots[index] = null;
            playerControlScript.Anim.SetTrigger("changeWeapon"); // trigger change weapon in animation
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
        weapon.SetHoldConfigs(holdWeaponParent); // hold configs should be directly under the hold weapon parent
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

        Spinner spinner = weaponGameObject.GetComponent<Spinner>();
        if (spinner != null)
        {
            spinner.enabled = false;
        }

        Transform transform = weaponGameObject.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == "Particle")
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }

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
