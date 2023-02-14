using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    public LayerMask collisionMask;
    public Color trailColor;
    float speed = 10f;
    float damage = 1;
    float lifeTime = 3f;
    float closerWidthToEnemy = .1f; // 当敌人运动速度极快，update中子弹没有检测到敌人（因为已经进入低人体内），这里加入一个极小的偏差来修复这个问题

    private void Start() {
        Destroy(gameObject, lifeTime);

        // 若子弹最初就生成在一个敌人体内，则raycast无法做出判断，此时需要用这个overlap函数进行判断
        Collider[] initalCollision = Physics.OverlapSphere(transform.position, 0.1f, collisionMask);
        if (initalCollision.Length > 0)
        {
            // 默认给第一个碰撞体
            OnHitObject(initalCollision[0], transform.position);
        }

        GetComponent<TrailRenderer>().material.SetColor("_TintColor", trailColor);
    }

    public void SetSpeed(float newSpeed){
        speed = newSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        float moveDistance = Time.deltaTime * speed;
        CheckCollisions(moveDistance);
        transform.Translate(Vector3.forward * moveDistance);
    }

    void CheckCollisions(float moveDistance){
        Ray ray = new Ray(transform.position, transform.forward);
        RaycastHit hit;

        if(Physics.Raycast(ray, out hit, moveDistance + closerWidthToEnemy, collisionMask, QueryTriggerInteraction.Collide)){
            OnHitObject(hit.collider, hit.point);
        }
    }

    void OnHitObject(Collider c, Vector3 hitPoint){

        IDamageable damageableObject = c.GetComponent<IDamageable>();
        if (damageableObject != null)
        {
            damageableObject.TakeHit(damage, hitPoint, transform.forward);
        }
        print(c.transform.name);
        GameObject.Destroy(gameObject);
    }
}
