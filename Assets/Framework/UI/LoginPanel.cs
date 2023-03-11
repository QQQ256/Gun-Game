using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : BasePanel
{
    protected override void Awake()
    {
        base.Awake();
    }
    // Start is called before the first frame update
    void Start()
    {
        // GetBaseComponent<Button>("1").onClick.AddListener(OnClickStart);
        UIManager.AddCustomEventListener(GetBaseComponent<Button>("1"), UnityEngine.EventSystems.EventTriggerType.PointerEnter, (data)=>{
            Debug.Log("进入"); 
        });
        UIManager.AddCustomEventListener(GetBaseComponent<Button>("1"), UnityEngine.EventSystems.EventTriggerType.PointerExit, (data)=>{
            Debug.Log("离开"); 
        });
    }

    protected override void OnClick(string name)
    {
        switch(name){
            case "1" :
                Debug.Log("111 was clicked");
            break;
            case "2" :
                Debug.Log("222 was clicked");
            break;
        }
    }

    protected override void OnTiggleChanged(string toggleName, bool value)
    {
        // switch(toggleName){

        // }
    }

    private void OnClickStart(){
        Debug.Log("1111");
    }

    public void Foo(){
        Debug.Log("LoginPanel after callback@!!");
    }

    public override void Show()
    {
        base.Show();
        // 由于showPanel每次都会调用show，可在这里编写show之后的逻辑
    }
}
