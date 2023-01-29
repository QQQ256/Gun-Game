using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Enemy : LivingEntity
{
    public enum State {Idle, Chasing, Attacking};

    State currentState;
    NavMeshAgent pathFinder;
    Transform target;
    Material skinMaterial;
    LivingEntity targetEntity;
    // Vector3 targetPosition;
    float attackDistanceThreshold = .5f;
    float sqrtDstToTarget; // the sqrt between two points
    float timeBetweenAttack = 1f;
    float nextAttackTime;
    float myCollisionRadius; // we don't want to enemy's destionation coincident with the player, so we calculate their radius.
    float targetCollisionRadius;
    float damage = 1;
    Color originalColor;
    bool hasTarget; // check if player is dead

    protected override void Start()
    {
        base.Start();        
        pathFinder = GetComponent<NavMeshAgent>();
        skinMaterial = GetComponent<Renderer>().material;
        originalColor = skinMaterial.color;

        if(GameObject.FindGameObjectWithTag("Player") != null){
            target = GameObject.FindGameObjectWithTag("Player").transform;
            targetEntity = target.GetComponent<LivingEntity>();
            targetEntity.OnDeath += OnTargetDeath;

            myCollisionRadius = GetComponent<CapsuleCollider>().radius;
            targetCollisionRadius = target.GetComponent<CapsuleCollider>().radius;

            currentState = State.Chasing;
            hasTarget = true;
            StartCoroutine(UpdatePath());
        }
    }

    void OnTargetDeath(){
        hasTarget = false;
        currentState = State.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        if(hasTarget){
            if(Time.time > nextAttackTime){
                sqrtDstToTarget = (target.position - transform.position).sqrMagnitude; // good performace without using Vector3.Distance
                // 计算边缘到另一个边缘的距离，所以加上半径
                if(sqrtDstToTarget < Mathf.Pow(attackDistanceThreshold + myCollisionRadius + targetCollisionRadius, 2)){
                    nextAttackTime = Time.time + timeBetweenAttack;
                    StartCoroutine(Attack());
                }
            }   
        }
    }

    // 模拟往前攻击的效果，核心是使用到的方程。
    // 这里不希望将攻击点设置为player的正中心，所以减去两个胶囊体的半径，使得两个物体正好做一个碰撞
    IEnumerator Attack(){
        currentState = State.Attacking;
        pathFinder.enabled = false;

        Vector3 originPosition = transform.position;

        Vector3 directionToTarget = (target.position - transform.position).normalized;
        Vector3 attackPosition = target.position - directionToTarget * (myCollisionRadius);

        float percent = 0;
        float attackSpeed = 3;

        skinMaterial.color = Color.red;

        bool doDamage = false;

        while(percent <= 1){

            if(percent >= 0.5f && !doDamage){
                doDamage = true;
                targetEntity.TakeDamage(damage);
            }

            percent += Time.deltaTime * attackSpeed;
            // 这里用一个函数来控制interpolation，目的是值从0-1-0
            // 用到一个方程 y = 4 * (-p^2 + p)
            float interpolation = (-Mathf.Pow(percent, 2) + percent) * 4;
            transform.position = Vector3.Lerp(originPosition, attackPosition, interpolation);
            yield return null;
        }

        skinMaterial.color = originalColor;
        pathFinder.enabled = true;
        currentState = State.Chasing;
    }

    // set destination in each frame is too expensive, so we use IEnumerator
    IEnumerator UpdatePath(){
        float refreshRate = 0.5f;

        while(hasTarget){
            if(currentState == State.Chasing){
                // 获取player与enemy之间两个坐标的方向，
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                Vector3 targetPosition = target.position - directionToTarget * (targetCollisionRadius + myCollisionRadius + attackDistanceThreshold / 2);
                if (!dead)
                {
                    pathFinder.SetDestination(targetPosition);
                }
            }
            
            yield return new WaitForSeconds(refreshRate);
        }
    }
}
