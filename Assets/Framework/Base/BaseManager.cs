using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
    因为这个约束，
    我们可以在 GetInstance() 方法中使用 new T() 创建一个 T 类型的实例。
    如果 T 类型不满足 new() 约束，
    则不能使用无参构造函数创建实例，
    代码会在编译时报错。
*/
public class BaseManager<T> where T : new()
{
   private static T instance;

   public static T GetInstance(){
    if(instance == null){
        instance = new T();
    }
    return instance;
   }
}
