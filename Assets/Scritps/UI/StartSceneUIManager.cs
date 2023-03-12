using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneUIManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        UIManager.GetInstance().ShowPanel<StartPanel>("StartPanel", UI_Layer.middle);
    }
}
