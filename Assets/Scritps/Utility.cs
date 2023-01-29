using System.Collections;
using System.Collections.Generic;

public static class Utility
{
    // 视频参考：https://www.youtube.com/watch?v=q7BL-lboRXo&ab_channel=SebastianLague
    public static T[] ShuffleArray<T>(T[] array, int seed){
        System.Random prng = new System.Random (seed);

        for (int i = 0; i < array.Length - 1; i++) // 最后一个shuffle也没什么意义，自己和自己交换
        {
            int randomIndex = prng.Next(i, array.Length);
            T tempItem = array[randomIndex];
            array[randomIndex] = array[i];
            array[i] = tempItem;
        }

        return array;
    }
}
