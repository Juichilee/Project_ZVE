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
 
    public override void Start()
    {
        base.Start();
        // Setup attributes for this weapon
        weaponName = knifeName;
        coolDownTime = knifeCoolDownTime;
        weaponAnimId = knifeWeaponAnimId;
        holdConfigName = knifeHoldConfigName;
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
                WeaponHolderAnim.SetTrigger("attack");
            }
        }
    }
    private Coroutine comboResetCoroutine;

    // Coroutine for enforcing combo delay on melee combos. Resets to 0 after completion
    IEnumerator StartComboResetTimer()
    {
        canCombo = true;
        yield return new WaitForSeconds(comboDelay); // Duration of the active hitbox
        // If combo hasn't been read once delay is up
        if(!comboRead)
        {     
            Debug.Log("Combo Time's Up, reseting combo");
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
