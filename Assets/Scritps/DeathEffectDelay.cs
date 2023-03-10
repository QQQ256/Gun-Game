using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectDelay : MonoBehaviour
{
    // private ParticleSystem ps = GetComponent<ParticleSystem>();

    private void Start() {
        // particleSystem = GetComponent<ParticleSystem>();
    }

    protected void OnEnable() {
        Invoke("Push", 1);
    }

    protected void Push(){
        PoolManager.GetInstance().PushObjectToPool(this.gameObject.name, this.gameObject);
    }
}
