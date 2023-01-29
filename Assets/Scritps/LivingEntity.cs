using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LivingEntity : MonoBehaviour, IDamageable
{
    public float startHealth;

    public event System.Action OnDeath;
    protected float health;
    protected bool dead;

    protected virtual void Start() {
        health = startHealth;
    }

    public void TakeHit(float damage, RaycastHit hit)
    {
        // do sth for this function later: ex. partical effect
        TakeDamage(damage);
    }

    protected void Die(){
        dead = true;
        if(OnDeath != null){
            OnDeath();
        }
        GameObject.Destroy(gameObject);
    }

    public void TakeDamage(float damage)
    {
        health -= damage;

        if (health <= 0 && !dead)
        {
            Die();
        }
    }
}
