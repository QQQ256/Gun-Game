using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public interface IEventInfo{

}

public class EventInfo<T> : IEventInfo{
    public UnityAction<T> actions;

    public EventInfo(UnityAction<T> action){
        actions += action;
    }
}

public class EventInfo : IEventInfo{
    public UnityAction actions;

    public EventInfo(UnityAction action){
        actions += action;
    }
}


// 使用：1、字典 2、委托 3、观察者 4、避免装箱和拆箱
// 之前的字典：Dictionary<string, UnityAction<object>>，由于使用到object，需要做装箱和拆箱
public class EventCenter : BaseManager<EventCenter>
{
    // key - 事件的名称（例如：玩家复活，死亡，怪物死亡复活）
    // val - 监听该事件对应的委托们（为了避免装箱拆箱，这里做了修改）
    // 由于EventInfo<T>类实现了IEventInfo接口(接口是引用类型)，所以可以将EventInfo<T>对象直接存储在eventDic字典中。
    private Dictionary<string, IEventInfo> eventDic = new Dictionary<string, IEventInfo>();

    /// <summary>
    /// 添加事件监听
    /// </summary>
    /// <param name="name">事件名称</param>
    /// <param name="action">用于处理该事件的委托函数</param>
    public void AddEventListener<T>(string name, UnityAction<T> action){
        // 有事件
        if(eventDic.ContainsKey(name)){
            // 来了个新的时间，加入监听

            // 类型转换的语法(eventDic[name] as EventInfo<T>)，将事件字典中的值对象强制转换为EventInfo<T>类型。
            // EventInfo<T>类是实现了IEventInfo接口的一个具体类，因此可以将EventInfo<T>对象转换为IEventInfo类型的值对象
            (eventDic[name] as EventInfo<T>).actions += action;
        }
        else{
            // eventDic.Add(name, action);
            // 首先创建了一个新的EventInfo<T>对象，该对象的构造函数会将传入的委托函数action添加到actions委托变量中，在内部实现事件的订阅
            eventDic.Add(name, new EventInfo<T>(action));
        }
    }

    /// <summary>
    /// 监听不需要参数传递的事件
    /// 并且同时写一个无参数类型的EventInfo
    /// </summary>
    /// <param name="name"></param>
    /// <param name="action"></param>
    public void AddEventListener(string name, UnityAction action){
        // 有事件
        if(eventDic.ContainsKey(name)){
            // 来了个新的时间，加入监听

            // 类型转换的语法(eventDic[name] as EventInfo<T>)，将事件字典中的值对象强制转换为EventInfo<T>类型。
            // EventInfo<T>类是实现了IEventInfo接口的一个具体类，因此可以将EventInfo<T>对象转换为IEventInfo类型的值对象
            (eventDic[name] as EventInfo).actions += action;
        }
        else{
            // eventDic.Add(name, action);
            // 首先创建了一个新的EventInfo<T>对象，该对象的构造函数会将传入的委托函数action添加到actions委托变量中，在内部实现事件的订阅
            eventDic.Add(name, new EventInfo(action));
        }
    }

    /// <summary>
    /// 移除事件监听
    /// </summary>
    public void RemoveEventListener<T>(string name, UnityAction<T> action){
        if(eventDic.ContainsKey(name)){
           (eventDic[name] as EventInfo<T>).actions -= action;
        }
    }

    // 重载版
    public void RemoveEventListener(string name, UnityAction action){
        if(eventDic.ContainsKey(name)){
           (eventDic[name] as EventInfo).actions -= action;
        }
    }

    /// <summary>
    /// 事件触发器
    /// </summary>
    /// <param name="name">哪个名称的事件被触发了？</param>
    public void EventTrigger<T>(string name, T info){
        if(eventDic.ContainsKey(name)){
            // 执行对应的委托函数
            if((eventDic[name] as EventInfo<T>).actions != null)
                (eventDic[name] as EventInfo<T>).actions.Invoke(info);
            // eventDic[name].Invoke(info);
        }
    }

    // 触发器重载版
    public void EventTrigger(string name){
        if(eventDic.ContainsKey(name)){
            // 执行对应的委托函数
            if((eventDic[name] as EventInfo).actions != null)
                (eventDic[name] as EventInfo).actions.Invoke();
            // eventDic[name].Invoke(info);
        }
    }

    public void Clear(){
        eventDic.Clear();
    }
}
