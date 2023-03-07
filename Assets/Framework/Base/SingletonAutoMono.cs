using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingletonAutoMono<T> : MonoBehaviour where T : MonoBehaviour
{
    private static T instance;

    // 自动产生一个GameObject，将单例脚本挂载，好处是不会因为手动挂载多个单例而出错
    public static T GetInstance(){
        if(instance == null){
            GameObject singletonObject = new GameObject();
            singletonObject.name = typeof(T).ToString();
            singletonObject.AddComponent<T>();

            DontDestroyOnLoad(singletonObject);
        }

        return instance;
    }
}
