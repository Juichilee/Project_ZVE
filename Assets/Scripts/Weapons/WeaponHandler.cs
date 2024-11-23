using TMPro;
using UnityEngine;
using UnityEngine.Animations.Rigging;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(PlayerControlScript))]
public class WeaponHandler : MonoBehaviour, IWeaponHolder
{
    // public Transform holdWeaponParent;
    public Transform secondHandAimTarget;
    public Transform secondHandHintTarget;
    [SerializeField] private Transform rightHandHoldParent;
    [SerializeField] private Transform leftHandHoldParent;
    [SerializeField] private Transform headHoldParent;


    [SerializeField] private Weapon[] weaponSlots = new Weapon[3];
    // Main difference between abilitySlots & weaponSlots is that the former is pre-initialized and pre-equipped
    // Abilities cannot be equipped or dequipped, picked up or dropped, only active or inactive
    [SerializeField] private Weapon[] abilitySlots = new Weapon[3];
    private int currentAbilityIndex = -1; // -1 for inactive ability 
    private int currentWeaponIndex = 0;
    private Collider currentPickupCollider;
    private PlayerControlScript playerControlScript;
    private ThirdPersonCamera thirdPersonCamera;
    public Rig aimRig;
    public TextMeshProUGUI ammoCountText;
    private GameObject pickupGuide;
    private Coroutine weaponsAtReadyCoroutine;
    private const float bodyWeightConst = 0.75f;
    private const float aimWeightConst = 1.0f;
    private const float secondHandWeightConst = 1.0f;
    private const int RANGEDLAYERINDEX = 2;
    private const int MELEELAYERINDEX = 3;
    private const int ABILITYLAYERINDEX = 4;
    private const float LAYERTRANSITIONDURATION = 0.2f;

    void Awake()
    {
        playerControlScript = GetComponent<PlayerControlScript>();
        thirdPersonCamera = GetComponent<ThirdPersonCamera>();
    }

    void Start()
    {
        // Initialize ability weapon fields
        for(int i = 0; i < abilitySlots.Length; i++)
        {
            Weapon abilityWeap = abilitySlots[i];
            if (abilityWeap)
            {
                abilityWeap.gameObject.SetActive(true);
                ParentWeapon(abilityWeap);
                abilityWeap.gameObject.SetActive(false);
            }
        }
    }

    public void DropAllWeapons()
    {
        DropWeapon(0);
        DropWeapon(1);
        DropWeapon(2);           
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
        // Debug.Log("Current Ability: " + GetActiveAbility()?.WeaponName);

        HandleAbilityInput();

        // No abilities can be active for regular weapon handling to occur
        if (GetActiveAbility() == null) 
        {
            HandlePickupTrigger();
            HandleWeaponSwitching();
            HandleWeaponInput();
            HandleWeaponDrop();
            pickupGuide.SetActive(currentPickupCollider != null);
        }

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

    Weapon GetActiveAbility()
    {
        if (currentAbilityIndex == -1)
        {
            return null;
        }
        return abilitySlots[currentAbilityIndex];
    }

    public Weapon GetWeapon(int index)
    {
        return weaponSlots[index];
    }

    public int GetCurrentWeaponIndex()
    {
        return currentWeaponIndex;
    }

    public int GetCurrentAbilityIndex()
    {
        return currentAbilityIndex;
    }

    private void HandlePickupTrigger()
    {
        if (currentPickupCollider != null && playerControlScript.Interact)
        {
            Weapon weapon = currentPickupCollider.GetComponent<Weapon>();
            PickUpWeapon(weapon);
        }
    }

    private void HandleAbilityInput()
    {
        if (elapsedTime != 0) return; // Wait until ability layer weight is completely reset to 0

        if (GetActiveAbility() == null) // If current ability is inactive, start input check
        {
            if (playerControlScript.InputAbility1)
            {
                ActivateAbility(0);
            } else if(playerControlScript.InputAbility2) {
                ActivateAbility(1);
            } else if(playerControlScript.InputAbility3) {
                ActivateAbility(2);
            }
            return;
        } else {
            // Only allow another ability to activate when current ability animation is done

            if (!GetActiveAbility().IsReady) return;

            // Disable previous active ability
            if (currentAbilityIndex != -1)
            {
                DeactivateAbility(currentAbilityIndex);
                currentAbilityIndex = -1; // Reset current ability to inactive state
            }
        }
    }

    private void ActivateAbility(int index)
    {
        // Force disable input during ability activation
        playerControlScript.ForceDisableInput = true;
        currentAbilityIndex = index;

        // Deactivate current weapons
        Weapon currWeapon = GetCurrentWeapon();
        if (currWeapon != null)
        {
            DeEquipWeapon(currentWeaponIndex);
        }

        // Enable ability layer while disabling others
        playerControlScript.Anim.SetLayerWeight(RANGEDLAYERINDEX, 0);
        playerControlScript.Anim.SetLayerWeight(MELEELAYERINDEX, 0);
        playerControlScript.Anim.SetLayerWeight(ABILITYLAYERINDEX, 1);
        Weapon abilityWeap = abilitySlots[index];

        // Enable ability model
        abilityWeap.gameObject.SetActive(true);
        playerControlScript.Anim.SetInteger("weaponAnimId", abilityWeap.WeaponAnimId);

        // Attack with ability
        abilityWeap.Attack();
        abilityWeap.IsReady = false;
        Ability ability = (Ability)abilityWeap; // Convert to Ability type
        StartCoroutine(StartAbilityCooldown(ability, abilityWeap.CoolDownTime));
    }

    IEnumerator StartAbilityCooldown(Ability ability, float coolDownTime)
    {
        ability.IsAbilityReady = false;
        yield return new WaitForSeconds(coolDownTime);
        ability.IsAbilityReady = true;
    }

    private void DeactivateAbility(int index)
    {
        // Turn off force disable after ability ends
        playerControlScript.ForceDisableInput = false;
        
        // Disable ability model
        Weapon ability = abilitySlots[index];
        ability.gameObject.SetActive(false);

        // Restore equipped weapon state
        Weapon currWeapon = GetCurrentWeapon();
        if (currWeapon == null)
        {
            playerControlScript.Anim.SetLayerWeight(RANGEDLAYERINDEX, 0);
            playerControlScript.Anim.SetLayerWeight(MELEELAYERINDEX, 0);
            
        } else if (currWeapon is MeleeWeapon) {
            playerControlScript.Anim.SetLayerWeight(MELEELAYERINDEX, 1);

        } else if (currWeapon is RangedWeapon) {
            playerControlScript.Anim.SetLayerWeight(RANGEDLAYERINDEX, 1);
        }
        EquipWeapon(currentWeaponIndex);

        // Resetting ability layer by lerping it to smooth the animation
        StartCoroutine(LerpLayerWeight(ABILITYLAYERINDEX, 0, LAYERTRANSITIONDURATION));
    }

    private float elapsedTime = 0f;
    IEnumerator LerpLayerWeight(int layerIndex, int targetWeight, float duration)
    {
        float initialWeight = playerControlScript.Anim.GetLayerWeight(layerIndex); // Current layer weight
        
        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float newWeight = Mathf.Lerp(initialWeight, targetWeight, elapsedTime / duration);
            playerControlScript.Anim.SetLayerWeight(layerIndex, newWeight);
            yield return null; // Wait for the next frame
        }

        // Ensure the weight is set to the exact target at the end
        playerControlScript.Anim.SetLayerWeight(layerIndex, targetWeight);

        elapsedTime = 0f; // Reset elapsedTime
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
            playerControlScript.Anim.SetLayerWeight(MELEELAYERINDEX, 0);
            playerControlScript.Anim.SetLayerWeight(RANGEDLAYERINDEX, 0);
            playerControlScript.Anim.SetInteger("weaponAnimId", -1); // id for unequipped is -1
            playerControlScript.ForceStrafe = playerControlScript.InputAimDown; // allow player to strafe while unarmed
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
    // Updates current weapon rig multi aim constraints to new weapon by name (rig setup name needs to match weapon name)
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
        // Weapons At Ready state keeps player ranged weapon at the ready after aiming or firing
        if (playerControlScript.InputAimDown || playerControlScript.InputHoldAttack)
        {
            if (weaponsAtReadyCoroutine != null)
            {
                StopCoroutine(weaponsAtReadyCoroutine);
            }
            weaponsAtReadyCoroutine = StartCoroutine(WeaponsAtReady(0.5f));
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
            rangedWeapon.Attack();
        }

        rangedWeapon.UpdateWeaponAim(playerControlScript.aimTarget);
    }

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
        if (playerControlScript.InputAttack)
        {
            if (weaponsAtReadyCoroutine != null)
            {
                StopCoroutine(weaponsAtReadyCoroutine);
            }
            meleeWeapon.Attack();
        }

        // Force player to strafe (attack in camera direction) while in attack animation
        AnimatorStateInfo currentStateInfo = playerControlScript.Anim.GetCurrentAnimatorStateInfo(MELEELAYERINDEX);
        int statehash = Animator.StringToHash("Empty");
        if (currentStateInfo.shortNameHash == statehash)
        {
            playerControlScript.ForceStrafe = false;
        } else {
            playerControlScript.ForceStrafe = true;
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
                thirdPersonCamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Ranged);
                playerControlScript.Anim.SetLayerWeight(MELEELAYERINDEX, 0); // Reset melee layer
                playerControlScript.Anim.SetLayerWeight(RANGEDLAYERINDEX, 1);
                aimRig.weight = 1f;
            } else {
                thirdPersonCamera.SwitchCameraStyle(ThirdPersonCamera.CameraStyle.Basic);
                playerControlScript.Anim.SetLayerWeight(MELEELAYERINDEX, 1);
                playerControlScript.Anim.SetLayerWeight(RANGEDLAYERINDEX, 0); // Reset ranged layer
                aimRig.weight = 0f;
            }
        } else {
            playerControlScript.Anim.SetInteger("weaponAnimId", -1); // id for unequipped is -1
        }
        playerControlScript.Anim.SetTrigger("changeWeapon");
        playerControlScript.ForceStrafe = false; // Reset force strafe after weapon switches

        currentWeaponIndex = index;
    }

    private void ActivateWeaponAimWeights(Weapon weapon)
    {
        if (weapon is RangedWeapon)
        {
            aim.weight = aimWeightConst;
            bodyAim.weight = bodyWeightConst;
            secondHandAim.weight = secondHandWeightConst;
        }
    }

    private void DeactivateWeaponAimWeights(Weapon weapon)
    {
        if (weapon is RangedWeapon)
        {
            // Reset prev ranged weapon aim weights so that curr equipped weapon aim is not affected
            aim.weight = 0f;
            bodyAim.weight = 0f;
            secondHandAim.weight = 0f;
        }
    }

    private void DeEquipWeapon(int index)
    {
        if (index >= 0 && index < weaponSlots.Length && weaponSlots[index] != null)
        {
            Weapon prevWeapon = weaponSlots[index];
            DeactivateWeaponAimWeights(prevWeapon);
            prevWeapon.gameObject.SetActive(false);
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

    private Transform GetHoldParent(HoldParentType holdParentType)
    {
        if (holdParentType == HoldParentType.RightHand) return rightHandHoldParent;
        if (holdParentType == HoldParentType.LeftHand) return leftHandHoldParent;
        if (holdParentType == HoldParentType.Head) return headHoldParent;
        Debug.LogError("No hold parent transform for the given hold parent type!");
        return null;
    }

    private void ParentWeapon(Weapon weapon)
    {
        GameObject weaponGameObject = weapon.gameObject;
        weapon.WeaponHolder = this;
        weapon.WeaponHolderAnim = playerControlScript.Anim;
        Transform holdParent = GetHoldParent(weapon.HoldParentType);
        weaponGameObject.transform.SetParent(holdParent);
        weapon.SetHoldConfigs(holdParent); // hold configs should be directly under the hold weapon parent
        weaponGameObject.transform.localScale = Vector3.one;
        
        // Disable kinematic and gravity for parented weapons
        Rigidbody rb = weaponGameObject.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.useGravity = true;
        }

        // Disable weapon collider
        Collider collider = weaponGameObject.GetComponent<Collider>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        currentPickupCollider = null;

        // Disable spinner script
        Spinner spinner = weaponGameObject.GetComponent<Spinner>();
        if (spinner != null)
        {
            spinner.enabled = false;
        }

        // Disable weapon pickup particle effects
        Transform transform = weaponGameObject.transform;
        for (int i = 0; i < transform.childCount; i++)
        {
            if (transform.GetChild(i).gameObject.tag == "Particle")
            {
                transform.GetChild(i).gameObject.SetActive(false);
            }
        }
    }

    private void PickUpWeapon(Weapon weapon)
    {
        if (GetCurrentWeapon() != null)
        {
            DropWeapon(currentWeaponIndex);
        }

        weaponSlots[currentWeaponIndex] = weapon;
        ParentWeapon(weapon);
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

    #region Event and Animation Event Callback Handling
    public void SpawnDamageObject()
    {
        Weapon currWeapon;
        if (GetActiveAbility())
        {
            currWeapon = GetActiveAbility();
        } else {
            currWeapon = GetCurrentWeapon();
        }

        if (currWeapon && currWeapon is MeleeWeapon meleeWeapon)
        {
            meleeWeapon.SpawnDamageObject();
        }
    }

    // Mainly for melee weapon combos
    public void SetIsReady(int weaponReady) // 0 for ready, 1 for not ready
    {
        Weapon currWeapon = GetCurrentWeapon();
        if (currWeapon == null)
        {
            return;
        }
        if(weaponReady == 0)
        {
            currWeapon.IsReady = true;
        } else {
            currWeapon.IsReady = false;
        }
    }

    public void SetIsAbilityReady(int abilityActive) // 0 for ready, 1 for not ready
    {
        Weapon currAbilty = GetActiveAbility();
        if (currAbilty == null)
        {
            return;
        }

        if(abilityActive == 0)
        {
            currAbilty.IsReady = true;
        } else {
            currAbilty.IsReady = false;
        }
    }

    public void PlayMeleeSound()
    {
        Weapon currWeapon = GetCurrentWeapon();
        if (!currWeapon)
        {
            return;
        }
        
        if (currWeapon is MeleeWeapon meleeWeapon)
        {
            meleeWeapon.PlayMeleeSound();
        }
    }

    #endregion
}
