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
    public int burstCount; // 爆发模式下子弹能打多少发子弹\
    [Header("Reload Attributes")]
    public int projectilesPerMag;
    public float reloadTime;

    [Header("Recoil Force Attributes")]
    public Vector2 recoilMinMaxForce = new Vector2(.05f, .2f);
    [Header("Recoil Angle Attributes")]
    public Vector2 recoilMinMaxAngle = new Vector2(3, 5);
    public float recoilMoveSpeed = .1f;
    public float recoilRotationSpeed = .1f;

    [Header("Aduios")]
    public AudioClip shootAudio;
    public AudioClip reloadAudio;

    MuzzleFlash muzzleFlash;
    float nextShotTime;
    bool triggerReleasedSinceLastShot; // 一直按着键，对于爆发模式，单射击模式来说不能一直触发
    int shotsRemainCountInBurst; // 还能打几发子弹
    int projectilesRemainingInMag; // 弹夹
    bool isReloading; // 子弹装填状态

    Vector3 recoilSmoothDampVelocity;
    float recoilRotationSmoothDampVelocity;
    float recoilAngle;

    private void Start() {
        projectilesRemainingInMag = projectilesPerMag;
        muzzleFlash = GameObject.FindObjectOfType<MuzzleFlash>();
        shotsRemainCountInBurst = burstCount;
        for(int i = 0; i < projectileSpawn.Length; i++){
            projectileSpawn[i].gameObject.SetActive(false);
        }
    }

    // transform.localEulerAngles写在update()中修复了换子弹朝天问题
    private void Update() {
        transform.localEulerAngles = Vector3.left * recoilAngle;
    }

    // 使用LateUpdate的原因是this.transform.LookAt会overwrite 修改的rotation
    // 所以这里写在LateUpdate里面
    void LateUpdate(){
        // ref是对变量的引用，方法中对参数的影响会改变参数本身的值
        // localPosition：父坐标的移动不会导致localPosition的变化
        transform.localPosition = Vector3.SmoothDamp(transform.localPosition, Vector3.zero, ref recoilSmoothDampVelocity, .1f);
        
        // transform.localEulerAngles 表示该物体的本地旋转角度
        // transform.localRotation 表示该物体的本地旋转四元数。
        recoilAngle = Mathf.SmoothDamp(recoilAngle, 0, ref recoilRotationSmoothDampVelocity, recoilRotationSpeed);
        // transform.localEulerAngles = transform.localEulerAngles + Vector3.left * recoilAngle;

        if(!isReloading && projectilesRemainingInMag == 0){
            Reload();
        }
    }

    void Shoot(){
        if(!isReloading && Time.time > nextShotTime && projectilesRemainingInMag > 0){
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
                if(projectilesRemainingInMag == 0){
                    break;
                }

                projectilesRemainingInMag--;
                nextShotTime              = Time.time + msBetweenShots / 1000;
                GameObject bullet         = PoolManager.GetInstance().GetObjectFromPool("Prefabs/Bullet");
                bullet.transform.position = projectileSpawn[i].position;
                bullet.transform.rotation = projectileSpawn[i].rotation;
                bullet.GetComponent<Projectile>().SetSpeed(muzzleVelocity);
                // Projectile newProjectile = Instantiate(projectile, projectileSpawn[i].position, projectileSpawn[i].rotation) as Projectile;
                // newProjectile.SetSpeed(muzzleVelocity);
            }

            // 弹壳
            GameObject shell = PoolManager.GetInstance().GetObjectFromPool("Prefabs/Shell");
            shell.transform.position = shellEjection.position;
            shell.transform.rotation = shellEjection.rotation;
            // Instantiate(shell, shellEjection.position, shellEjection.rotation);

            // 火光
            muzzleFlash.Activate();

            // 后坐力模拟，按下往后移动，之后update()把它再回归起始位置
            transform.localPosition -= Vector3.forward * Random.Range(recoilMinMaxForce.x, recoilMinMaxForce.y); 
            // 后坐力模拟，角度转动
            recoilAngle             += Random.Range(recoilMinMaxAngle.x, recoilMinMaxAngle.y);
            recoilAngle              = Mathf.Clamp(recoilAngle, 0, 30);

            // 播放声音
            AudioManager.instance.PlaySound(shootAudio, transform.position);
        }
    }

    public void Reload(){
        if(!isReloading && projectilesRemainingInMag != projectilesPerMag){
            StartCoroutine(AnimateReload());
            AudioManager.instance.PlaySound(reloadAudio, transform.position);
        }
    }

    IEnumerator AnimateReload(){
        isReloading = true;

        // pause here
        yield return new WaitForSeconds(.2f);
        
        float reloadSpeed               = 1f / reloadTime;
        float percent                   = 0;
        float maxReloadAngle            = 30;
        Vector3 initalRot               = transform.localEulerAngles;

        while(percent < 1){
            percent                     += Time.deltaTime * reloadSpeed;
            // 这里用一个函数来控制interpolation，目的是值从0-1-0
            // 目的是做一个换弹夹的角度旋转动画
            float interpolation         = (-Mathf.Pow(percent, 2) + percent) * 4;
            float reloadAngle           = Mathf.Lerp(0, maxReloadAngle, interpolation);
            transform.localEulerAngles  = initalRot + Vector3.left * reloadAngle;
            Debug.Log(transform.localEulerAngles);
            yield return null;
        }

        isReloading = false;
        projectilesRemainingInMag = projectilesPerMag;
    }

    public void AimAt(Vector3 transform){
        if(!isReloading){
            this.transform.LookAt(transform);  
        }  
    }

    public void OnTriggerHold(){
        Shoot();
        triggerReleasedSinceLastShot        = false;
    }

    public void OnTiggerRelease(){
        triggerReleasedSinceLastShot        = true;
        shotsRemainCountInBurst             = burstCount;
    }
}
