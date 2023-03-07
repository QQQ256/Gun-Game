using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestForMonoInheritance{
    public void foo(){
        Debug.Log("TestForMonoInheritance");
    }
}

public class Test : MonoBehaviour
{
    private void Start() {
        TestForMonoInheritance t = new TestForMonoInheritance();
        MonoManager.GetInstance().AddUpdateEventListener(t.foo);
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetMouseButtonDown(0)){
            PoolManager.GetInstance().GetObjectFromPool("Prefabs/Cube");
        }
    }
}
