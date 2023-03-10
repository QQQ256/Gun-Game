using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GunController : MonoBehaviour
{
    public Transform weaponHold;

    public Gun[] allGuns;

    Gun equipGun;

    // instantiate a new gun when the player is equipping the gun.
    public void EuqibGun(Gun gunToEquip){
        if(equipGun != null){
            Destroy(equipGun.gameObject);
        }
        equipGun = Instantiate(gunToEquip, weaponHold.position, weaponHold.rotation);
        equipGun.transform.parent = weaponHold;
    }

    public void EuqibGun(int weaponIndex){
        EuqibGun(allGuns[weaponIndex]);
    }

    public void AimAt(Vector3 transform){
        if(equipGun != null){
            equipGun.AimAt(transform);
        }
    }

    public void OnTriggerHold(){
        if(equipGun != null){
            equipGun.OnTriggerHold();
        }
    }

    public void OnTriggerRelease(){
        if(equipGun != null){
            equipGun.OnTiggerRelease();
        }
    }

    // 获取枪的高度
    public float GetWeaponHeight{
        get {
            return weaponHold.position.y;
        }
    }

    public void Reload(){
        if(equipGun != null){
            equipGun.Reload();
        }
    }
}
