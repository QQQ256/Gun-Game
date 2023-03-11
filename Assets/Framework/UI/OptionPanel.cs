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

    int screenResolutionIndex;
    bool isFullScreen;
    protected override void Awake()
    {
        base.Awake();
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

    protected override void OnTiggleChanged(string toggleName, bool value)
    {
        
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

    //private void SetFullScreen()
    //{
    //    isFullScreen = fullScreenToggle.GetComponent<Toggle>().isOn;
    //    print(isFullScreen);
    //    for (int i = 0; i < resolutionToggles.Length; i++)
    //    {
    //        resolutionToggles[i].interactable = !isFullScreen;
    //    }

    //    if (isFullScreen)
    //    {
    //        Resolution[] allResolutions = Screen.resolutions; // 由游戏引擎枚举的分辨率，支持当前显示器的分辨率
    //        Resolution maxResolution = allResolutions[allResolutions.Length - 1];
    //        Screen.SetResolution(maxResolution.width, maxResolution.height, true);
    //    }
    //    else
    //    {
    //        SetScreenResolution(screenResolutionIndex);
    //    }

    //    PlayerPrefs.SetInt("fullscreen", ((isFullScreen) ? 1 : 0));
    //    PlayerPrefs.Save();
    //}
}
