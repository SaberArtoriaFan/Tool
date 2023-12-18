using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

public class ArrayUtil
{
    //随机提取一项
    public static T Random<T>(List<T> arr)
    {
        return arr[UnityEngine.Random.Range(0, arr.Count)];
    }
    //随机提取N个项
    public static List<T> RandomList<T>(List<T> arr, int count=1)
    {
        List<T> arr2 = Clone(arr);
        SortRandom(arr2);
        List<T> arr3 = new List<T>();
        for (int i = 0; i < count; i++) arr3.Add(arr2[i]);
        arr2.Clear();
        return arr3;
    }

    //随机排序
    public static List<T> SortRandom<T>(List<T> arr)
    {
        arr.Sort(delegate (T a, T b) { return UnityEngine.Random.Range(0f, 1f) > 0.5f ? 1 : -1; });
        return arr;
    }
    //浅克隆
    public static List<T> Clone<T>(List<T> arr)
    {
        List<T> arr2 = new List<T>();
        foreach(T item in arr) arr2.Add(item);
        return arr2;
    }
    //从数组1中排除数组2的元素
    public static List<T> Diff<T>(List<T> arr1, List<T> arr2) {
        List<T> arr = new List<T>();
        foreach (T item in arr1)
            if (arr2.IndexOf(item) == -1) 
                arr.Add(item);
        return arr;
    }
}
