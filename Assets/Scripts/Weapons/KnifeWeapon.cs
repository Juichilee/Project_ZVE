using System.Collections;
using UnityEngine;

public class KnifeWeapon : MeleeWeapon
{
    [SerializeField] private string knifeName = "Knife";
    [SerializeField] private float knifeCoolDownTime = 1f;
    [SerializeField] private int knifeWeaponAnimId = 0;
    [SerializeField] private string knifeHoldConfigName = "HoldConfig_Knife";
    [SerializeField] private float comboDelay = 0.4f; // Time allowed between combo inputs
    private bool canCombo = false;
    private bool comboRead = false;
    private int maxComboStep = 3; // Change to match max number of combos for this weapon
    private Coroutine comboResetCoroutine;

    void Awake()
    {
        // Setup attributes for this weapon
        weaponName = knifeName;
        coolDownTime = knifeCoolDownTime;
        weaponAnimId = knifeWeaponAnimId;
        holdConfigName = knifeHoldConfigName;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        canCombo = false;
    }

    public override void Attack()
    {
        if (comboResetCoroutine != null)
        {
            StopCoroutine(comboResetCoroutine);
        }
        comboResetCoroutine = StartCoroutine(StartComboResetTimer());

        if (IsReady)
        {
            // Start the attack
            WeaponHolderAnim.SetTrigger("attack");
        }
        else if (!IsReady && canCombo)
        {
            comboRead = true;
            WeaponHolderAnim.SetTrigger("attack");
        }
        else
        {
            comboRead = false;
        }
    }

    IEnumerator StartComboResetTimer()
    {
        canCombo = true;
        comboRead = false; // Reset comboRead at the start
        yield return new WaitForSeconds(comboDelay);
        if (!comboRead)
        {
            canCombo = false;
        }
    }

    public override void PlayMeleeSound()
    {
        if (meleeSoundClip != null)
        {
            audioSource.PlayOneShot(meleeSoundClip);
        }
    }
}
