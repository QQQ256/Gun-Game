using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 这个脚本的意义：unity中update多了有性能开销，把所有的update都集中在一个update更新是比较好的。
/// </summary>
public class MonoController : MonoBehaviour
{
    private event UnityAction upadteEvent;
    
    void Start()
    {
        DontDestroyOnLoad(this.gameObject);
    }

    void Update()
    {
        if(upadteEvent != null){
            upadteEvent();
        }
    }

    /// <summary>
    /// 为外部提供的添加帧更新的方法
    /// </summary>
    /// <param name="func"></param>
    public void AddUpdateEventListener(UnityAction func){
        upadteEvent += func;
    }

    /// <summary>
    /// 为外部提供的移除帧更新的方法
    /// </summary>
    /// <param name="func"></param>
    public void RemoveUpdateEventListener(UnityAction func){
        upadteEvent -= func;
    }
}
