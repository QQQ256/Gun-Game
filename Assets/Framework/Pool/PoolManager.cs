using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// 将每个衣柜以对象化的形式展现就是这样，将list下所有的物体都存于root之下
public class PoolObject{
    public GameObject root;
    public List<GameObject> poolList;

    public PoolObject(GameObject obj, GameObject poolObj){
        // 为抽屉创建父对象，并把父对象设置为pool的子对象
        root = new GameObject(obj.name);
        root.transform.parent = poolObj.transform;

        poolList = new List<GameObject>() {obj};
    }

    // 往抽屉里塞东西，并设置父对象
    public void PushObj(GameObject obj){
        poolList.Add(obj);
        obj.transform.parent = root.transform;
        obj.SetActive(false);
    }

    // 从抽屉里取东西
    public GameObject GetObj(){
        GameObject obj = null;

        obj = poolList[0];
        obj.SetActive(true);
        obj.transform.parent = null;

        poolList.RemoveAt(0);
        return obj;
    }
}

public class PoolManager : BaseManager<PoolManager>
{
    public Dictionary<string, PoolObject> poolDic = new Dictionary<string, PoolObject>();

    private GameObject poolObject;

    // 获取对应衣柜中的gameobject，需要知道从哪个衣柜里拿
    public GameObject GetObjectFromPool(string name){
        GameObject obj = null;

        if(poolDic.ContainsKey(name) && poolDic[name].poolList.Count > 0){
            // 将list头部的object拿出来
            obj = poolDic[name].GetObj();
        }
        else{
            obj = GameObject.Instantiate(Resources.Load<GameObject>(name));
            obj.name = name;
        }
        return obj;
    }

    // 如果存在这个list，就加进去，否则就创建一个list加进去
    public void PushObjectToPool(string name, GameObject obj){
        if(poolObject == null)
            poolObject = new GameObject("Pool");
            // GameObject.DontDestroyOnLoad(poolObject);

        // 将物体层级置于poolObject下面
        obj.transform.parent = poolObject.transform;
        obj.SetActive(false);

        if(poolDic.ContainsKey(name)){
            poolDic[name].PushObj(obj);
        }
        else{
            poolDic.Add(name, new PoolObject(obj, poolObject));
        }
    }

    // 手动清除所有生成的物体，相当于解所有的ref
    // 一般用于场景切换之时
    public void Clear(){
        poolDic.Clear();
        poolObject = null;
    }
}
