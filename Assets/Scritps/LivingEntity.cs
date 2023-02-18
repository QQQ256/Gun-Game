using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startHealth;

    public event System.Action OnDeath;
    public float health {get; protected set;}
    protected bool dead;

    protected virtual void Start() {
        health = startHealth;
    }

    public virtual void TakeHit(float damage, Vector3 hitPoint, Vector3 hitDirection)
    {
        // do sth for this function later: ex. partical effect
        TakeDamage(damage);
    }

    [ContextMenu("Self Destruct")] // 写了这个，可以右键脚本，选中这个就能直接调用这个函数，非常便于测试!!!
    public virtual void Die(){
        dead = true;
        if(OnDeath != null){
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public virtual void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }
}
