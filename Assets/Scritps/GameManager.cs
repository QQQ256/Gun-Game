using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : SingletonAutoMono<GameManager>
{

    void Start()
    {
        UIManager.GetInstance().ShowPanel<GamePanel>("GamePanel", UI_Layer.middle);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
