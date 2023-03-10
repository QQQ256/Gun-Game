using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class MusicMgr : BaseManager<MusicMgr>
{
    private AudioSource BGM;
    private float BGM_MaxVolume = 1;

    /// <summary>
    /// 一个GameObject上挂载非常多个Sound Clip，但BGM只有一个，这里用list来承载
    /// </summary>
    private GameObject soundObject = null;
    private List<AudioSource> soundList = new List<AudioSource>();
    private float soundVolume;
    public void PlayBGM(string name){
        if(BGM == null){
            GameObject obj = new GameObject();
            obj.name = "BGM";
            obj.AddComponent<AudioSource>();
        }

        // 异步加载audio
        ResourceManager.GetInstance().LoadResourceASync<AudioClip>("Music/BGM/" + name, (clip) => {
            BGM.clip = clip;
            BGM.volume = BGM_MaxVolume;
            BGM.Play();
        });
    }

    public void ChangeBGMVolume(float value){
        if(BGM == null)
            return;

        BGM_MaxVolume = value;
        BGM.volume = BGM_MaxVolume;
    }   

    public void PauseBGM(){
        if(BGM == null)
            return;
        BGM.Pause();
    }

    /// <summary>
    /// Stop方法会停止正在播放的音频，将播放进度归零，并且可以立即开始播放新的音频
    /// </summary>
    public void StopBGM(){
        if(BGM == null)
            return;
        BGM.Stop();
    }

    /// <summary>
    /// 播放音效
    /// </summary>
    /// <param name="name">音效名称</param>
    /// <param name="isLoop">是否循环播放该sound</param>
    /// <param name="callBack">如有需要则可以获取加载的clip，否则默认回调函数为null</param>
    public void PlaySound(string name, bool isLoop, UnityAction<AudioSource> callBack = null){
        if(soundObject == null){
            soundObject = new GameObject();
            soundObject.name = "BGM";
            soundObject.AddComponent<AudioSource>();
        }

        // 异步加载audio
        ResourceManager.GetInstance().LoadResourceASync<AudioClip>("Music/Sound/" + name, (clip) => {
            AudioSource source = soundObject.AddComponent<AudioSource>();
            source.clip = clip;
            source.loop = isLoop;
            source.volume = soundVolume;
            source.Play();

            soundList.Add(source);

            // 如有需要，委托返回出去
            if(callBack != null)
                callBack(source);
        });
    }

    public void StopSound(AudioSource source){
        if(soundList.Contains(source)){
            soundList.Remove(source);
            source.Stop();
            GameObject.Destroy(source);
        }
    }

    /// <summary>
    /// 修改所有Sound的音效
    /// </summary>
    /// <param name="value"></param>
    public void ChangeSoundVolume(float value){
        soundVolume = value;
        foreach (var sound in soundList)
        {
            sound.volume = soundVolume;
        }
    }
}
