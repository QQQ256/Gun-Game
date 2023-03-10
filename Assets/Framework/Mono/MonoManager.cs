using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 作为暴露MonoController的外部接口，为外部提供无需继承monoBehavior的帧更新方法
/// </summary>
public class MonoManager : BaseManager<MonoManager>
{
    private MonoController monoController;

    public MonoManager(){
        // 由于继承单例模式，只会被初始化一次，保证唯一性
        GameObject obj = new GameObject("MonoController");
        monoController = obj.AddComponent<MonoController>();
    }    

    // 封装add和remove方法，之后就可以在其他地方调用MonoController中的方法了！
    // 使用方式：调用manager，在start中加入要在update方法中使用函数的监听
    public void AddUpdateEventListener(UnityAction func){
        monoController.AddUpdateEventListener(func);
    }

    public void RemoveUpdateEventListener(UnityAction func){
        monoController.RemoveUpdateEventListener(func);
    }

    // 由于monoController继承mono，我们要做的就是在做一次封装
    /// <summary>
    /// 携程只能开启在MonoCOntroller中的携程，若调用其他脚本中的是不行的
    /// </summary>
    /// <param name="routine"></param>
    /// <returns></returns>
    public Coroutine StartCoroutine(IEnumerator routine){
        return monoController.StartCoroutine(routine);
    }

    public Coroutine StartCoroutine(string methodName, [DefaultValue("null")] object value){
        return monoController.StartCoroutine(methodName, value);
    }
}
