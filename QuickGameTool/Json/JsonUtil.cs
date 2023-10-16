using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonUtil 
{
    //读取文件
    public static string ReadData(string fileName)
    {
        //string类型的数据常量
        string readData;
        //获取到路径
        string fileUrl =  fileName + " .json";
        Debug.Log($"读取路径->{fileUrl}");
        if (File.Exists(fileUrl))
        {
            //读取文件
            using (StreamReader sr = File.OpenText(fileUrl))
            {
                //数据保存
                readData = sr.ReadToEnd();
                sr.Close();
            }
            Debug.Log($"正在从{fileUrl}读取文件,内容为{readData}");

            //返回数据
            return readData;
        }
        else
            return null;


    }
    public static T ReadData<T>(string fileName)where T:class
    {
        string s=ReadData(fileName);
        if (string.IsNullOrEmpty(s)) return null;
        return JsonConvert.DeserializeObject<T>(s);
    }

    //通过文件名称保存数据到json文件中，存储的路径为persistentDataPath
    public static void Saver(string fileName, object value)
    {

        string json = JsonConvert.SerializeObject(value);
        string filepath =  fileName + " .json";
        Debug.Log("自动保存了，内容为" + json+$"\n路径为:{filepath}");

        if (!File.Exists(filepath))
        {
            File.Create(filepath).Dispose();
            //Debug.Log(filepath);
        }

        using (StreamWriter sw = new StreamWriter(filepath))
        {
            sw.WriteLine(json);
            sw.Close();
            sw.Dispose();
        }
    }
}
