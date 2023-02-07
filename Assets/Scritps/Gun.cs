using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public Transform muzzle; // 枪口
    public Transform shell;
    public Transform shellEjection;
    public Projectile projectile;
    public float msBetweenShots = 100;
    public float muzzleVelocity = 35;

    MuzzleFlash muzzleFlash;
    float nextShotTime;

    private void Start() {
        muzzleFlash = GameObject.FindObjectOfType<MuzzleFlash>();
    }

    public void Shoot(){
        if(Time.time > nextShotTime){
            nextShotTime = Time.time + msBetweenShots / 1000;
            Projectile newProjectile = Instantiate(projectile, muzzle.position, muzzle.rotation) as Projectile;
            newProjectile.SetSpeed(muzzleVelocity);

            // 弹壳
            Instantiate(shell, shellEjection.position, shellEjection.rotation);

            // 火光
            muzzleFlash.Activate();
        }
    }
}
