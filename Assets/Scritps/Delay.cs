using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Delay : MonoBehaviour
{
   
    protected void OnEnable() {
        Invoke("Push", 1);
    }

    protected void Push(){
        Debug.Log("Pushed + "  + this.gameObject.name);
        PoolManager.GetInstance().PushObjectToPool(this.gameObject.name, this.gameObject);
    }
}
