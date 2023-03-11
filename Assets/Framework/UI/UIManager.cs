using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public enum UI_Layer{
    buttom,
    middle,
    top,
    system
}

public class UIManager : BaseManager<UIManager>
{
    public RectTransform canvas;
    // 异步加载面板，并存入字典中
    public Dictionary<string, BasePanel> panelDic = new Dictionary<string, BasePanel>();

    // 四个UI层级
    private Transform buttom;
    private Transform middle;
    private Transform top;
    private Transform system;

    public UIManager(){
        // 同步加载canvas
        GameObject obj          = ResourceManager.GetInstance().LoadResourceSync<GameObject>("UI/Canvas");
        GameObject eventSystem  =  ResourceManager.GetInstance().LoadResourceSync<GameObject>("UI/EventSystem");
        GameObject.DontDestroyOnLoad(obj);
        GameObject.DontDestroyOnLoad(eventSystem);

        canvas  = obj.transform as RectTransform;
        buttom  = canvas.Find("Buttom");
        middle  = canvas.Find("Middle");
        top     = canvas.Find("Top");
        system  = canvas.Find("System");
    }


    /// <summary>
    /// 加载一个面板
    /// 由于是异步加载面板，并且有个回调函数，只有在面板加载后，回调函数才会去执行（这里用到了泛型去获取BasePanel子类中所挂载的脚本）
    /// </summary>
    /// <param name="panelName">要加载的预制体面板名称</param>
    /// <param name="layer">想要创在哪各层级下 || 存在默认层级</param>
    /// <param name="callBack">创建完后，想要对面板做的事情 || 设置为null说明不是所有的面板都需要回调函数</param>
    /// <typeparam name="T">面板的脚本类型</typeparam>
    public void ShowPanel<T>(string panelName, UI_Layer layer = UI_Layer.middle, UnityAction<T> callBack = null) where T: BasePanel{
        if(panelDic.ContainsKey(panelName)){
            if(callBack != null){
                // 里是转换
                callBack(panelDic[panelName] as T);
            }

            // 若已经存在，则不应该再加载，直接返回
            return;
        }

        ResourceManager.GetInstance().LoadResourceASync<GameObject>("UI/" + panelName, (obj) => {
            // 加载的面板变成canvas子类，并设置位置
            Transform root = buttom;
            switch(layer){
                case UI_Layer.middle:
                    root = middle;
                break;
                case UI_Layer.top:
                    root = top;
                break;
                case UI_Layer.system:
                    root = system;
                break;
            }

            // 设置父对象
            obj.transform.SetParent(root);

            // 归零初始化位置
            obj.transform.localPosition                = Vector3.zero;
            obj.transform.localScale                   = Vector3.one;
            (obj.transform as RectTransform).offsetMax = Vector2.zero;
            (obj.transform as RectTransform).offsetMin = Vector2.zero;

            // 获取面板上所挂载的脚本
            T panel = obj.GetComponent<T>();
            if (panel == null)
            {
                Debug.LogError("Failed to get component: " + typeof(T));
                return;
            }

            if(callBack != null){
                callBack(panel);
            }

            panel.Show();

            // 存入字典
            panelDic.Add(panelName, panel);
        });
    }

    public void HidePanel(string panelName){
        if(panelDic.ContainsKey(panelName)){
            // 先删除，后从字典中移除
            GameObject.Destroy(panelDic[panelName].gameObject);
            panelDic.Remove(panelName);
        }
    }

    /// <summary>
    /// 返回一个面板
    /// </summary>
    /// <param name="panelName">面板名称</param>
    /// <typeparam name="T">面板上挂载脚本名称</typeparam>
    /// <returns></returns>
    public T GetPanel<T>(string panelName) where T: BasePanel{
        if(panelDic.ContainsKey(panelName)){
            return panelDic[panelName] as T;
        }
        return null;
    }

    /// <summary>
    /// 获取层级
    /// </summary>
    /// <param name="layer">UI_Layer.你需要层级</param>
    /// <returns></returns>
    public Transform GetLayer(UI_Layer layer){
        switch(layer){
            case UI_Layer.buttom:
                return this.buttom;
            case UI_Layer.middle:
                return this.middle;
            case UI_Layer.top:
                return this.top;
            case UI_Layer.system:
                return this.system;
        }

        return null;
    }

    /// <summary>
    /// 给控件添加自定义事件监听
    /// 这么做相当于封装了一个方法，供UI模块使用
    /// </summary>
    /// <param name="component">控件对象</param>
    /// <param name="type">事件类型</param>
    /// <param name="callBack">事件的响应函数</param>
    public static void AddCustomEventListener(UIBehaviour component, EventTriggerType type, UnityAction<BaseEventData> funcCallBack){
        EventTrigger trigger = component.GetComponent<EventTrigger>();
        if(trigger == null){
            trigger = component.gameObject.AddComponent<EventTrigger>();
        }

        EventTrigger.Entry entry = new EventTrigger.Entry();
        entry.eventID = type;  // 要设置哪种事件
        entry.callback.AddListener(funcCallBack);

        trigger.triggers.Add(entry);
    }
}
