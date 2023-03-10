using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public enum AudioChannel {Master, Sfx, Music};
    public static AudioManager instance;

    public float masterVolumePercent {get; private set;} // 意思是：只能get，不能在外部设置，也就是set
    public float sfxVolumePercent {get; private set;}
    public float musicVolumePercent {get; private set;}

    AudioSource sfx2DSource; // 由于我们使用了AudioSource.PlayClipAtPoint，在3D世界中声音也就变成3D的了，我们要的是均衡感，所以要转成2D
    AudioSource[] musicSources;
    int activeMusicSourceIndex; // 追踪哪个music在运行

    Transform audioListener;
    Transform playerTransform;

    SoundLibrary soundLibrary;

    private void Awake() {

        if(instance != null){
            // 防止一个单例出现多个复制
            Destroy(gameObject);
        }
        else{
            instance = this;
            DontDestroyOnLoad(gameObject);

            musicSources = new AudioSource[2];
            for (int i = 0; i < 2; i++)
            {
                GameObject newMusicSource = new GameObject("Music Source " + (i + 1));
                musicSources[i] = newMusicSource.AddComponent<AudioSource>();
                newMusicSource.transform.parent = transform;
            }

            GameObject newSfx2DSource = new GameObject("newSfx2DSource");
            sfx2DSource = newSfx2DSource.AddComponent<AudioSource>();
            newSfx2DSource.transform.parent = transform;

            audioListener = FindObjectOfType<AudioListener>().transform;
            if(FindObjectOfType<Player>() != null){
                playerTransform = FindObjectOfType<Player>().transform;
            }
            soundLibrary = FindObjectOfType<SoundLibrary>();

            // load PlayerPrefs;
            
            masterVolumePercent = PlayerPrefs.GetFloat("master vol", 1);
            sfxVolumePercent = PlayerPrefs.GetFloat("sfx vol", 1);
            musicVolumePercent = PlayerPrefs.GetFloat("music vol", 1);
        }
    }
    
    private void Start() {
        MonoManager.GetInstance().AddUpdateEventListener(AudioUpdate);
    }

    private void OnDisable() {
        MonoManager.GetInstance().RemoveUpdateEventListener(AudioUpdate);
    }

    private void AudioUpdate() {
        if(playerTransform != null){
            audioListener.position = playerTransform.position;
        }
    }

    public void SetVolume(float volumePercent, AudioChannel channel){
        switch(channel){
            case AudioChannel.Master:
                masterVolumePercent = volumePercent;
                break;
            case AudioChannel.Music:
                musicVolumePercent = volumePercent;
                break;
            case AudioChannel.Sfx:
                sfxVolumePercent = volumePercent;
                break;
        }

        musicSources[0].volume = musicVolumePercent * masterVolumePercent;
        musicSources[1].volume = sfxVolumePercent * masterVolumePercent;

        // 持久记录音量数据
        PlayerPrefs.SetFloat("master vol", masterVolumePercent);
        PlayerPrefs.SetFloat("sfx vol", sfxVolumePercent);
        PlayerPrefs.SetFloat("music vol", musicVolumePercent);
        PlayerPrefs.Save();
    }

    // 跟随某个物体去playSound
    public void PlaySound(AudioClip clip, Vector3 pos){
        if(clip != null){
            AudioSource.PlayClipAtPoint(clip, pos, sfxVolumePercent * masterVolumePercent);
        }
    }

    public void PlaySound(string soundName, Vector3 pos){
        PlaySound(soundLibrary.GetClipFromName(soundName), pos);
    }

    public void PlaySound2D(string soundName){
        sfx2DSource.PlayOneShot(soundLibrary.GetClipFromName(soundName), sfxVolumePercent * masterVolumePercent);
    }

    // 用于 main theme 和 menu theme的切换，切换时会有一个fade in的效果
    public void PlayMusic(AudioClip clip, float fadeDuration = 1){
        activeMusicSourceIndex = 1 - activeMusicSourceIndex; // 0 - 1 - 0 - 1 - ...
        musicSources[activeMusicSourceIndex].clip = clip;
        musicSources[activeMusicSourceIndex].Play();

        StartCoroutine(AnimateMusicCorssFade(fadeDuration));
    }

    IEnumerator AnimateMusicCorssFade(float duration){
        float percent = 0;

        while(percent < 1){
            percent += Time.deltaTime * 1 / duration; // * speed
            // 打开一个音频，关闭另外一个
            musicSources[activeMusicSourceIndex].volume = Mathf.Lerp(0, musicVolumePercent * masterVolumePercent, percent);
            musicSources[1 - activeMusicSourceIndex].volume = Mathf.Lerp(musicVolumePercent * masterVolumePercent, 0, percent);
            yield return null;
        }
    }
}
