using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OptionPanel : BasePanel
{

    List<Toggle> resolutionToggles = new List<Toggle>();
    Toggle fullScreenToggle;
    Slider masterVolumeSlider;
    Slider musicVolumeSlider;
    Slider sfxVolumeSlider;

    int[] screenWidths = {960, 1280, 1920};
    int screenResolutionIndex;
    bool isFullScreen;
    protected override void Awake()
    {
        base.Awake();
        
        screenResolutionIndex = PlayerPrefs.GetInt("screen resolution index");
        print(screenResolutionIndex);
        isFullScreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

        // toggle init
        fullScreenToggle = GetBaseComponent<Toggle>("Full Screen");
        resolutionToggles.Add(GetBaseComponent<Toggle>("960x640"));
        resolutionToggles.Add(GetBaseComponent<Toggle>("1280x720"));
        resolutionToggles.Add(GetBaseComponent<Toggle>("1920x1080"));

        for (int i = 0; i < resolutionToggles.Count; i++)
        {
            resolutionToggles[i].isOn = i == screenResolutionIndex;
        }
        fullScreenToggle.isOn = isFullScreen;
    }

    private void Start()
    {
        // slider init
        masterVolumeSlider = GetBaseComponent<Slider>("Master Volume");
        masterVolumeSlider.value = AudioManager.instance.masterVolumePercent;

        musicVolumeSlider = GetBaseComponent<Slider>("Music Volume");
        musicVolumeSlider.value = AudioManager.instance.musicVolumePercent;

        sfxVolumeSlider = GetBaseComponent<Slider>("SFX Volume");
        sfxVolumeSlider.value = AudioManager.instance.sfxVolumePercent;
    }

    protected override void OnClick(string name)
    {
        switch (name)
        {
            case "Quit":
                UIManager.GetInstance().HidePanel("OptionPanel");
                UIManager.GetInstance().ShowPanel<StartPanel>("StartPanel", UI_Layer.middle);
                break;
            default:
                Debug.LogError("No such name: " + name);
                break;
        }
    }

    protected override void OnSliderValueChanged(string sliderName, float value)
    {
        switch (sliderName)
        {
            case "Master Volume":
                SetMasterVolume(value);
                break;
            case "Music Volume":
                SetMusicVolume(value);
                break;
            case "SFX Volume":
                SetSfxVolume(value);    
                break;
            default:
                Debug.LogError("No such name: " + sliderName);
                break;
        }
    }

    protected override void OnToggleChanged(string toggleName, bool value)
    {
        switch (toggleName)
        {
            case "960x640":
                SetScreenResolution(0);
                break;
            case "1280x720":
                SetScreenResolution(1);
                break;
            case "1920x1080":  
                SetScreenResolution(2);
                break;
            case "Full Screen":  
                SetFullScreen();
                break;
            default:
                Debug.LogError("No such name: " + toggleName);
                break;
        }
    }

    private void SetMasterVolume(float value)
    {
        masterVolumeSlider.value = value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    private void SetMusicVolume(float value)
    {
        musicVolumeSlider.value = value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    private void SetSfxVolume(float value)
    {
        sfxVolumeSlider.value = value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }

    private void SetFullScreen()
    {
    //    isFullScreen = fullScreenToggle.GetComponent<Toggle>().isOn;
    //    print(isFullScreen);
       for (int i = 0; i < resolutionToggles.Count; i++)
       {
           resolutionToggles[i].interactable = !isFullScreen;
       }

       if (isFullScreen)
       {
           Resolution[] allResolutions = Screen.resolutions; // 由游戏引擎枚举的分辨率，支持当前显示器的分辨率
           Resolution maxResolution = allResolutions[allResolutions.Length - 1];
           Screen.SetResolution(maxResolution.width, maxResolution.height, true);
       }
       else
       {
           SetScreenResolution(screenResolutionIndex);
       }

       PlayerPrefs.SetInt("fullscreen", ((isFullScreen) ? 1 : 0));
       PlayerPrefs.Save();
    }

    private void SetScreenResolution(int i){
        Debug.Log("SetScreenResolution" + i);
        Debug.Log(resolutionToggles.Count);
        if(resolutionToggles[i].isOn){
            screenResolutionIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);

            // Remember Resolution
            PlayerPrefs.SetInt("screen resolution index", screenResolutionIndex);
        }
    }
}
