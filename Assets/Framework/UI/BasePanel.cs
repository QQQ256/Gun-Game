using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Unity.VisualScripting;

/// <summary>
/// 面板基类，找到一个面板下的所有控件
/// 非常省事，就不用一直Public button xxx;的去声明后拖拽了，直接通过名字就能找到那个控件
/// （这么做是为了直接在代码中对控件事件做操作，无需在Hierarchy中手动拖拽）
/// </summary>
public class BasePanel : MonoBehaviour
{
    /// <summary>
    /// 存储每个面板，以及每个面板下的所有控件
    /// </summary>
    /// <typeparam name="string">面板的名称</typeparam>
    /// <typeparam name="UIBehaviour">里是转换原则，所有控件的基类是UIBehaviour</typeparam>
    /// 由于一个object下会挂载两个组件，例如image + button，所以把value设置为list
    /// <returns></returns>
    private Dictionary<string, List<UIBehaviour>> panelDic = new Dictionary<string, List<UIBehaviour>>();

    protected virtual void Awake()
    {
        FindChildComponents<Slider>();
        FindChildComponents<Button>();
        FindChildComponents<Image>();
        FindChildComponents<Text>();
        FindChildComponents<Toggle>();      
    }

    /// <summary>
    /// 获取名字对应的控件
    /// </summary>
    /// <param name="keyName">控件名称</param>
    /// <typeparam name="T">需要找什么类型的控件</typeparam>
    /// <returns></returns>
    protected T GetBaseComponent<T>(string keyName) where T : UIBehaviour{
        if(panelDic.ContainsKey(keyName)){
            foreach (var component in panelDic[keyName])
            {
                return (component is T) ? (component as T) : null;
            }
        }
        Debug.LogError(typeof(T) + " was not found!");
        return null;
    }

    /// <summary>
    /// 查找所有组件，并加入字典，并对组件进行事件监听
    /// </summary>
    /// <typeparam name="T"></typeparam>
    private void FindChildComponents<T>() where T : UIBehaviour{
        T[] components = this.GetComponentsInChildren<T>();
        foreach (var component in components)
        {
            string keyName = component.name;
            if(panelDic.ContainsKey(keyName)){
                // Debug.Log("Add existed panelDic[keyName] to the dictionary: " + keyName);
                panelDic[keyName].Add(component);
            }
            else{
                //Debug.Log("Create dictionary" + typeof(T).Name + " with value" + component);
                panelDic.Add(keyName, new List<UIBehaviour>(){component});  
            }

            // 现在想实现一个方法，每个按钮被加载之后，顺带的去监听这个按钮上的方法
            // 若想进行监听，那么则需要传入一个无参数的函数，作为被监听函数（因为onClick.AddListener不需要参数）
            // 但又需要通过按钮的名称去判断到底按下了哪个按钮，所以这里使用lambda表达式的方法（回调函数）来将一个参数（这里就能传递参数了）传入函数中
            // 实际作用就是1、监听这个按钮，2、传递这个按钮的名称给子类（所以这里用virtual）
            if(component is Button){
                (component as Button).onClick.AddListener(() => { // 这里的onClick是unity自带的button的onClick
                    //在这里将foreach中遇到的button都进行监听，并将按钮名称传入

                    /*
                        每个按钮都被添加了一个回调函数，当按钮被单击时，回调函数被调用并将按钮的名称作为参数传递给OnClick虚方法。
                        在子类中，可以重写OnClick方法，并根据按钮名称进行操作。

                        当子类重写OnClick方法时，可以{访问}从父类继承的所有数据，包括被单击的按钮的名称。
                        子类可以使用这个名称来判断哪个按钮被单击了，并根据需要执行相应的操作。
                    */
                    OnClick(keyName);
                });
            }
            else if(component is Toggle){
                // Toggle的onValueChanged.AddListener则需要一个bool类型的值传入，所以这里lambda表达式需要一个bool类型的value
                (component as Toggle).onValueChanged.AddListener((value) => {
                    OnToggleChanged(keyName, value);
                });
            }
            else if (component is Slider)
            {
                // Slider则传入的是float值
                (component as Slider).onValueChanged.AddListener((value) => {
                    OnSliderValueChanged(keyName, value);
                    // Debug.Log(value.ToString());
                });
            }
        }
    }

    public virtual void Show(){

    }

    public virtual void Hide(){

    }

    protected virtual void OnClick(string name){

    }

    protected virtual void OnToggleChanged(string toggleName, bool value){

    }

    protected virtual void OnSliderValueChanged(string sliderName, float value)
    {

    }
}
