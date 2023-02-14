    using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public enum FileMode{Auto, Burst, Single}; // 设置三种射击模式：全自动，爆发，单点
    public FileMode fileMode;
    public Transform[] projectileSpawn; // 枪口，有的枪会有多个枪口，例如霰弹枪
    public Transform shell;
    public Transform shellEjection;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;
    public int burstCount; // 爆发模式下子弹能打多少发子弹

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    bool triggerReleasedSinceLastShot; // 一直按着键，对于爆发模式，单射击模式来说不能一直触发
    int shotsRemainCountInBurst; // 还能打几发子弹

    private void Start() {
        muzzleFlash = GameObject.FindObjectOfType<MuzzleFlash>();
        shotsRemainCountInBurst = burstCount;
        for(int i = 0; i < projectileSpawn.Length; i++){
            projectileSpawn[i].gameObject.SetActive(false);
        }
    }

    void Shoot(){
        if(Time.time > nextShotTime){
            
            // 爆发模式
            if(fileMode == FileMode.Burst){
                if(shotsRemainCountInBurst == 0){
                    return;
                }
                shotsRemainCountInBurst--;
            }
            // 单点模式
            else if(fileMode == FileMode.Single){
                if(!triggerReleasedSinceLastShot){
                    return;
                }
            }

            // 全自动模式
            for(int i = 0; i < projectileSpawn.Length; i++){
                nextShotTime             = Time.time + msBetweenShots / 1000;
                Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                newProjectile.SetSpeed(muzzleVelocity);
            }

            // 弹壳
            Instantiate(shell, shellEjection.position, shellEjection.rotation);

            // 火光
            muzzleFlash.Activate();
        }
    }

    public void OnTriggerHold(){
        Shoot();
        triggerReleasedSinceLastShot = false;
    }

    public void OnTiggerRelease(){
        triggerReleasedSinceLastShot        = true;
        shotsRemainCountInBurst             = burstCount;
    }
}
