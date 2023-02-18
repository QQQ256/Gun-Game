using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 存储所有音频
public class SoundLibrary : MonoBehaviour
{
    public SoundGroup[] soundGroups;

    Dictionary<string, AudioClip[]> groupDictionary = new Dictionary<string, AudioClip[]>();

    private void Awake() {
        foreach(SoundGroup group in soundGroups){
            // 将ID与group进行绑定
            groupDictionary.Add(group.groupID, group.group);
        }
    }

    public AudioClip GetClipFromName(string name){
        // 随机返回一个group中的音效
        if(groupDictionary.ContainsKey(name)){
            AudioClip[] sounds = groupDictionary[name];
            return sounds[Random.Range(0, sounds.Length)];
        }
        return null;
    }

    [System.Serializable]
    public class SoundGroup{
        public string groupID;
        public AudioClip[] group;
    }
}
