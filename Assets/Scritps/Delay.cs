using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delay : MonoBehaviour
{
    private void OnEnable() {
        Invoke("Push", 1);
    }

    private void Push(){
        Debug.Log("Pushed + "  + this.gameObject.name);
        PoolManager.GetInstance().PushObjectToPool(this.gameObject.name, this.gameObject);
    }
}
