using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairs : MonoBehaviour
{
    public LayerMask targetMask;
    public SpriteRenderer dot;
    public Color dotHighlightColor;

    Color originalColor;

    private void Start() {
        Cursor.visible = false;
        originalColor = dot.color;
    }

    void Update()
    {
        // 做一个自动旋转的效果
        transform.Rotate(Vector3.forward * 40 * Time.deltaTime);
    }

    public void DetectEnemy(Ray ray){
       if(Physics.Raycast(ray, 100, targetMask)){
        dot.color = dotHighlightColor;
       }
       else{
        dot.color = originalColor;
       }
    }
}
