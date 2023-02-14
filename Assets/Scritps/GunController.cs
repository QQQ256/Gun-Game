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
}
