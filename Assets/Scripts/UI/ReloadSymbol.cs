using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ReloadSymbol : MonoBehaviour
{
    WeaponHandler weaponHandler;
    Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        weaponHandler = GameObject.FindGameObjectsWithTag("Player")[0].GetComponent<WeaponHandler>();
        animator = this.gameObject.GetComponent<Animator>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Weapon weapon = weaponHandler.GetWeapon(weaponHandler.GetCurrentWeaponIndex());
        animator.SetBool("Flash", weapon != null && weapon is RangedWeapon && ((RangedWeapon)weapon).getCurrentClip() == 0);
    }
}
