using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// 资源加载模块
/// 使用了：异步加载、委托和泛型、lambda表达式、携程
/// </summary>
public class ResourceManager : BaseManager<ResourceManager>
{
    /// <summary>
    /// 同步加载资源
    /// 举个例子：GameObject prefab = LoadResourceSync<GameObject>("MyPrefab");
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    /// <returns></returns>
    public T LoadResourceSync<T>(string name) where T: Object{
        T t = Resources.Load<T>(name);
        
        // 若是要加载一个GameObject，则直接在这里实例化，返回GameObject
        if(t is GameObject){
            return GameObject.Instantiate(t);
        }
        // 其他文件，例如不需要实例化的：音效，文本，直接返回出去
        else{
            return t;
        }
    }

    /// <summary>
    /// 异步加载资源，使用到异步则必使用携程
    /// </summary>
    /// <param name="name"></param>
    /// <typeparam name="T"></typeparam>
    public void LoadResourceASync<T>(string name, UnityAction<T> callback) where T: Object{
        MonoManager.GetInstance().StartCoroutine(_LoadResourceASync(name, callback));
    }

    IEnumerator _LoadResourceASync<T>(string name, UnityAction<T> callback) where T: Object{
        ResourceRequest t = Resources.LoadAsync<T>(name);
        yield return t;

        // 由于携程无法返回被加载的资源，因此使用委托来返回，所以函数第二个形参是一个Event
        if(t.asset is GameObject){
            // 这里要做隐式转换
            callback(GameObject.Instantiate(t.asset) as T);
        }
        else{
            callback(t.asset as T);
        }
    }

}
