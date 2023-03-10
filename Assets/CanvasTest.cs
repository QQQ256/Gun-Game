using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CanvasTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.GetInstance().ShowPanel<LoginPanel>("LoginPanel", UI_Layer.middle, Foo);
    }

    void Foo(LoginPanel loginPanel){
        // 高级的回调，不需要像之前一样先获取，比如xx.getComponent<>()，再去怎么样，这样就能直接访问面板所挂载的脚本中的函数了！牛批
        loginPanel.Foo();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
