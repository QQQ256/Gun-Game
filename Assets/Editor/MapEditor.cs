using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MapGenerator))]
public class MapEditor : Editor
{
    public override void OnInspectorGUI(){
    
        MapGenerator map = target as MapGenerator;

        // DrawDefaultInspector返回一个bool值，地图值的变化才会触发地图的生成
        if(DrawDefaultInspector()){
            map.GenerateMap();
        }

        // 或是点击按钮
        if(GUILayout.Button("Generate Map")){
            map.GenerateMap();
        }
    }
}
