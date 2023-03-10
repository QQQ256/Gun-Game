using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MusicManager : MonoBehaviour
{
   public AudioClip mainTheme;
   public AudioClip menuTheme;

   private void Start() {
      AudioManager.instance.PlayMusic(menuTheme, 2);
      MonoManager.GetInstance().AddUpdateEventListener(MusicUpdate);
   }

   private void OnDisable() {
      MonoManager.GetInstance().RemoveUpdateEventListener(MusicUpdate);
   }

   private void MusicUpdate() {
      if(Input.GetKeyDown(KeyCode.Space)){
         AudioManager.instance.PlayMusic(mainTheme, 3);
      }
   }
}
