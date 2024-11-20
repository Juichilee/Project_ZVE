using System.Collections;
using UnityEngine;

public class KnifeWeapon : MeleeWeapon
{
    [SerializeField] private string knifeName = "Knife";
    [SerializeField] private float knifeCoolDownTime = 1f;
    [SerializeField] private int knifeWeaponAnimId = 0;
    [SerializeField] private string knifeHoldConfigName = "HoldConfig_Knife";
    [SerializeField] private float comboDelay = 0.2f; // Time allowed between combo inputs
    private bool canCombo = true;
    private bool comboRead = false;
    private int comboStep = 0;
    private int maxComboStep = 3; // Change to match max number of combos for this weapon
 
    public override void Start()
    {
        base.Start();
        // Setup attributes for this weapon
        weaponName = knifeName;
        coolDownTime = knifeCoolDownTime;
        weaponAnimId = knifeWeaponAnimId;
        holdConfigName = knifeHoldConfigName;
    }

    public override void OnDisable()
    {
        base.OnDisable();
        comboStep = 0;
        canCombo = true;
        comboRead = false;
    }

    public override void Attack()
    {
        if(IsReady)
        {
            // Read in combo input if the player is already attacking
            WeaponHolderAnim.SetTrigger("attack");
            if (comboResetCoroutine != null)
            {
                StopCoroutine(comboResetCoroutine);
            }
            comboResetCoroutine = StartCoroutine(StartComboResetTimer());
        } else {
            if (canCombo)
            {
                comboRead = true;
                WeaponHolderAnim.SetTrigger("attack"); // Set attack trigger to be consumed
                comboStep += 1;
            }
        }
    }
    private Coroutine comboResetCoroutine;

    // Coroutine for enforcing combo delay on melee combos. Resets to 0 after completion
    IEnumerator StartComboResetTimer()
    {
        canCombo = true;
        yield return new WaitForSeconds(comboDelay); // Duration of the active hitbox
        // If combo hasn't been read once the combo delay is up or on final step of combo
        if(!comboRead || comboStep == maxComboStep)
        {     
            canCombo = false;
            comboStep = 0;
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
