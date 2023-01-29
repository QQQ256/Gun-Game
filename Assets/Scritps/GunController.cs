using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;

    public Gun originGun;

    Gun equipGun;

    private void Start() {
        if(originGun != null){
            EuqibGun(originGun);
        }
    }

    // instantiate a new gun when the player is equipping the gun.
    public void EuqibGun(Gun gunToEquip){
        if(equipGun != null){
            Destroy(equipGun.gameObject);
        }
        equipGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equipGun.transform.parent = weaponHold;
    }

    public void Shoot(){
        if(equipGun != null){
            equipGun.Shoot();
        }
    }
}
