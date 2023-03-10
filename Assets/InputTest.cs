using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputTest : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        InputManager.GetInstance().DetectInput(true);

        EventCenter.GetInstance().AddEventListener<KeyCode>("Key_Down", CheckInputDown);
        EventCenter.GetInstance().AddEventListener<KeyCode>("Key_Up", CheckInputUp);
    }

    private void CheckInputDown(KeyCode key){
        // KeyCode key = (KeyCode)k; 避免了装箱拆箱咯
        switch(key){
            case KeyCode.W:
                Debug.Log("W down");
                break;
            case KeyCode.A:
                Debug.Log("A down");
                break;
            case KeyCode.S:
                Debug.Log("S down");
                break;
            case KeyCode.D:
                Debug.Log("D down");
                break;
        }
    }

    private void CheckInputUp(KeyCode key){
        // KeyCode key = (KeyCode)k;
        switch(key){
            case KeyCode.W:
                Debug.Log("W up");
                break;
            case KeyCode.A:
                Debug.Log("A up");
                break;
            case KeyCode.S:
                Debug.Log("S up");
                break;
            case KeyCode.D:
                Debug.Log("D up");
                break;
        }
    }
}
