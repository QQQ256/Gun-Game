using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class SceneMgr : BaseManager<SceneMgr>
{
    // public SceneMgr(){
    //     SceneManager.sceneLoaded += OnSceneLoaded;
    // }
    /// <summary>
    /// 异步加载场景
    /// 给外部加载场景的接口
    /// </summary>
    /// <param name="name"></param>
    /// <param name="func"></param>
    public void LoadSceneAsync(string name, UnityAction func = null){
        // 由于BaseManager没有继承mono，使用monoController中的Coroutine
        MonoManager.GetInstance().StartCoroutine(_LoadSceneAsync(name, func));
    }

    IEnumerator _LoadSceneAsync(string name, UnityAction func){
        AsyncOperation ao = SceneManager.LoadSceneAsync(name);

        /*
            在下一次迭代时，我们重新检查 AsyncOperation 对象的 isDone 属性，
            如果该属性为 true，说明场景加载已经完成，此时我们使用 yield return 返回一个整数 0，
            表示协程已经执行完毕，并立即执行传递给函数的委托方法 func()。
        */
        while(!ao.isDone){
            EventCenter.GetInstance().EventTrigger("Loading", ao.progress);
            
            // 使用 yield return 关键字暂停协程的执行，直到下一次迭代。
            yield return ao.progress;
        }
        yield return 0;

        // 加载完场景之后再执行函数
        if(func != null)
        {
            func();
        }
    }

    // public void Dispose(){
    //     SceneManager.sceneLoaded -= OnSceneLoaded;
    // }

    // private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    // {
    //     // 处理场景加载完成事件
    // }

    
}
