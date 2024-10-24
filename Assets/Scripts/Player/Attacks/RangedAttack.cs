// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;
// using UnityEngine.Animations.Rigging;

// public class RangedAttack : MonoBehaviour
// {

//     public CharacterInputController cinput;
//     // public Transform shootPos;
//     public GameObject projectileObj;
//     public Rig aimRig;
//     public GameObject crossHair;
//     public Transform targetPos;
//     bool _inputAimDown = false;
//     bool _inputShoot = false;
//     public LayerMask aimColliderLayerMask  = new LayerMask();
//     public ProjectileWeapon currProjectileWeapon;

//     void Awake()
//     {
//         cinput = GetComponent<CharacterInputController>();
//         if (cinput == null)
//             Debug.Log("CharacterInput could not be found");
//         crossHair.SetActive(true);
//     }

//     private PlayerSounds playerSounds;
//     // Start is called before the first frame update
//     void Start()
//     {
//         playerSounds = GetComponent<PlayerSounds>();
//     }

//     // Update is called once per frame
//     void Update()
//     {
//         if (cinput.enabled)
//         {
//             _inputAimDown = cinput.AimDown;
//             _inputShoot = cinput.Shoot;
//         }

//         // Shooting logic
//         if (_inputAimDown)
//         {
//             playerSounds.GunReady();
//             aimRig.weight = Mathf.Lerp(aimRig.weight, 1f, Time.deltaTime * 2f);

//             // crossHair.SetActive(true);

//             currProjectileWeapon.UpdateWeaponAim(gameObject);

//             targetPos.position = currProjectileWeapon.GetTargetPos();

//             if (_inputShoot){
//                 currProjectileWeapon.FireWeapon();
//                 playerSounds.Shots();
//             }
            
//         } else {
//             aimRig.weight = Mathf.Lerp(aimRig.weight, 0f, Time.deltaTime * 2f);
//             // crossHair.SetActive(false);
//         }
//     }
// }
