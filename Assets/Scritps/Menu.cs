using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{
    public GameObject mainMenuHolder;
    public GameObject optionsMenuHolder;
    public Slider[] volumeSliders;
    public Toggle[] resolutionToggles;
    public Toggle fullScreenToggle;
    public int[] screenWidths;

    int screenResolutionIndex;
    bool isFullScreen;

    private void Start() {
        screenResolutionIndex = PlayerPrefs.GetInt("screen resolution index");
        print(screenResolutionIndex);
        isFullScreen = (PlayerPrefs.GetInt("fullscreen") == 1) ? true : false;

        // Set Volume Values
        volumeSliders[0].value = AudioManager.instance.masterVolumePercent;
        volumeSliders[1].value = AudioManager.instance.musicVolumePercent;
        volumeSliders[2].value = AudioManager.instance.sfxVolumePercent;

        for(int i = 0; i < resolutionToggles.Length; i++){
            resolutionToggles[i].isOn = i == screenResolutionIndex;
        }

        fullScreenToggle.isOn = isFullScreen;
        // SetFullScreen();
    }

    public void Play(){
        SceneManager.LoadScene("Game");
    }

    public void Quit(){
        Application.Quit();
    }

    public void OptionsMenu(){
        mainMenuHolder.SetActive(false);
        optionsMenuHolder.SetActive(true);
    }
    public void MainMenu(){
        mainMenuHolder.SetActive(true);
        optionsMenuHolder.SetActive(false);
    }

    public void SetScreenResolution(int i){
        if(resolutionToggles[i].isOn){
            screenResolutionIndex = i;
            float aspectRatio = 16 / 9f;
            Screen.SetResolution(screenWidths[i], (int)(screenWidths[i] / aspectRatio), false);

            // Remember Resolution
            PlayerPrefs.SetInt("screen resolution index", screenResolutionIndex);
        }
    }

    public void SetFullScreen(){
        isFullScreen = fullScreenToggle.GetComponent<Toggle>().isOn; 
        print(isFullScreen);
        for(int i = 0; i < resolutionToggles.Length; i++){
            resolutionToggles[i].interactable = !isFullScreen;
        }

        if(isFullScreen){
            Resolution[] allResolutions = Screen.resolutions; // 由游戏引擎枚举的分辨率，支持当前显示器的分辨率
            Resolution maxResolution = allResolutions[allResolutions.Length - 1];
            Screen.SetResolution(maxResolution.width, maxResolution.height, true);
        }
        else{
            SetScreenResolution(screenResolutionIndex);
        }

        PlayerPrefs.SetInt("fullscreen", ((isFullScreen) ? 1 : 0));
        PlayerPrefs.Save();
    }

    public void SetMasterVolume(){
        float value = volumeSliders[0].value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Master);
    }

    public void SetMusicVolume(){
        float value = volumeSliders[1].value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Music);
    }

    public void SetSfxVolume(){
        float value = volumeSliders[2].value;
        AudioManager.instance.SetVolume(value, AudioManager.AudioChannel.Sfx);
    }
}
