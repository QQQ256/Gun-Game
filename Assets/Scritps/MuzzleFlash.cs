using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MuzzleFlash : MonoBehaviour
{
    public GameObject flashHolder;
    public Sprite[] flashSprites; // 获取flash的四帧动画
    public SpriteRenderer[] spriteRenderers; // 获取0度和90度两个muzzle的sprite
    public float flashTime;
    void Start()
    {
        Deactivate();
    }

    public void Activate(){
        flashHolder.gameObject.SetActive(true);

        int flashSpriteIndex = Random.Range(0, flashSprites.Length);
        for(int i = 0; i < spriteRenderers.Length; i++){
            spriteRenderers[i].gameObject.SetActive(true);
            spriteRenderers[i].sprite = flashSprites[flashSpriteIndex];
        }

        Invoke("Deactivate", flashTime);
    }

    void Deactivate(){
        for(int i = 0; i < spriteRenderers.Length; i++){
            spriteRenderers[i].gameObject.SetActive(false);
        }
        flashHolder.gameObject.SetActive(false);
    }
}
