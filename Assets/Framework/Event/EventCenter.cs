using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


// 使用：1、字典 2、委托 3、观察者
public class EventCenter : BaseManager<EventCenter>
{
    // key - 事件的名称（例如：玩家复活，死亡，怪物死亡复活）
    // val - 监听该事件对应的委托们
    private Dictionary<string, UnityAction<object>> eventDic = new Dictionary<string, UnityAction<object>>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">用于处理该事件的委托函数</param>
    public void AddEventListener(string name, UnityAction<object> action){
        // 有事件
        if(eventDic.ContainsKey(name)){
            // 来了个新的时间，加入监听
            eventDic[name] += action;
        }
        else{
            eventDic.Add(name, action);
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener(string name, UnityAction<object> action){
        if(eventDic.ContainsKey(name)){
            eventDic[name] -= action;
        }
    }

    /// <summary>
    /// 事件触发器
    /// </summary>
    /// <param name="name">哪个名称的事件被触发了？</param>
    public void EventTrigger(string name, object info){
        if(eventDic.ContainsKey(name)){
            // 执行对应的委托函数
            eventDic[name].Invoke(info);
        }
    }

    public void Clear(){
        eventDic.Clear();
    }
}
