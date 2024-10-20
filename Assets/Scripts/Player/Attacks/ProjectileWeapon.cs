using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileWeapon : MonoBehaviour
{
    public GameObject projectileObj;
    public Transform shootPos;
    public GameObject shooter;

    public Vector3 mouseWorldPosition;
    public Vector3 aimDir;
    public LayerMask aimColliderLayerMask  = new LayerMask();
    public int ammoCount;
    public int fireRate;
    public int reloadTime;
    public int damage;
    public int stagger;
    
    public void UpdateWeaponAim(GameObject shooter)
    {
        this.shooter = shooter;

        Vector2 screenCenterPoint = new Vector2(Screen.width/2f, Screen.height/2f);
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);
        int fixedDistance = 99;
        if(Physics.Raycast(ray, out RaycastHit raycastHit, fixedDistance, aimColliderLayerMask))
        {
            mouseWorldPosition = raycastHit.point;
        } else {
            mouseWorldPosition = ray.origin + ray.direction * fixedDistance;
        }
        aimDir = (mouseWorldPosition - shootPos.position).normalized;
    }

    public Vector3 GetTargetPos()
    {
        return mouseWorldPosition;
    }
    public void FireWeapon()
    {
        GameObject projInst = Instantiate(projectileObj, shootPos.position, Quaternion.LookRotation(aimDir, Vector3.up));
        projInst.GetComponent<Projectile>().SetShooter(shooter);
    }

    public void Reload()
    {

    }
}
