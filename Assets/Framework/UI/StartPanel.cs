using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class StartPanel : BasePanel
{
    protected override void Awake()
    {
        base.Awake();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    protected override void OnClick(string name)
    {
        switch (name)
        {
            case "Play":
                Debug.Log("Play was clicked");
                UIManager.GetInstance().HidePanel("StartPanel");

                EventCenter.GetInstance().Clear();
                PoolManager.GetInstance().Clear();

                SceneMgr.GetInstance().LoadSceneAsync("Game");
                break;
            case "Options":
                Debug.Log("Option was clicked");
                UIManager.GetInstance().HidePanel("StartPanel");
                UIManager.GetInstance().ShowPanel<OptionPanel>("OptionPanel", UI_Layer.middle);
                break;
            case "Quit":
                Application.Quit();
                Debug.Log("Quit was clicked");
                break;
            default:
                Debug.LogError("No such name: " + name);
                break;
        }
    }


    public override void Show()
    {
        base.Show();
        // 由于showPanel每次都会调用show，可在这里编写show之后的逻辑
        //masterVolumeSlider = GetBaseComponent<Slider>("Master Volume");
        //if (masterVolumeSlider != null)
        //{
        //    Debug.Log("masterVolumeSlider");
        //    masterVolumeSlider.value = AudioManager.instance.masterVolumePercent;
        //}
        
    }
}
