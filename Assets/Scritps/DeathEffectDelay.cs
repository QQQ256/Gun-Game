using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeathEffectDelay : MonoBehaviour
{
    protected void OnEnable() {
        Invoke("Push", 1);
    }

    protected void Push(){
        PoolManager.GetInstance().PushObjectToPool(this.gameObject.name, this.gameObject);
    }
}
