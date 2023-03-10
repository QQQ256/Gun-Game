using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 统计输入管理器
/// 使用到：1、Input类 2、事件中心 3、公共mono（因为要监测update，继承baseMgr是没有mono可使用的）
/// </summary>
public class InputManager : BaseManager<InputManager>
{

    private bool isOpen; // 用于控制是否对按键进行检测

    /// <summary>
    /// 构造函数，监听Update
    /// 由于继承BaseManager，该构造函数会也只会被调用一次
    /// </summary>
    public InputManager(){
        MonoManager.GetInstance().AddUpdateEventListener(InputUpdate);
    }

    /// <summary>
    /// 按键匹配监测，并在按下后触发对应事件
    /// </summary>
    /// <param name="k"></param>
    private void KeyCodeCheck(KeyCode k){
        if(Input.GetKeyDown(k)){
            EventCenter.GetInstance().EventTrigger("Key_Down", k);
        }
        if(Input.GetKeyUp(k)){
            EventCenter.GetInstance().EventTrigger("Key_Up", k);
        }
    }

    private void InputUpdate(){
        if(!isOpen) return;

        KeyCodeCheck(KeyCode.W);
        KeyCodeCheck(KeyCode.A);
        KeyCodeCheck(KeyCode.S);
        KeyCodeCheck(KeyCode.D);
    }

    public void DetectInput(bool status){
        isOpen = status;
    }
}
