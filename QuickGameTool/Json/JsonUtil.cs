using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public static class JsonUtil 
{
    //��ȡ�ļ�
    public static string ReadData(string fileName)
    {
        //string���͵����ݳ���
        string readData;
        //��ȡ��·��
        string fileUrl =  fileName + " .json";
        Debug.Log($"��ȡ·��->{fileUrl}");
        if (File.Exists(fileUrl))
        {
            //��ȡ�ļ�
            using (StreamReader sr = File.OpenText(fileUrl))
            {
                //���ݱ���
                readData = sr.ReadToEnd();
                sr.Close();
            }
            Debug.Log($"���ڴ�{fileUrl}��ȡ�ļ�,����Ϊ{readData}");

            //��������
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

    //ͨ���ļ����Ʊ������ݵ�json�ļ��У��洢��·��ΪpersistentDataPath
    public static void Saver(string fileName, object value)
    {

        string json = JsonConvert.SerializeObject(value);
        string filepath =  fileName + " .json";
        Debug.Log("�Զ������ˣ�����Ϊ" + json+$"\n·��Ϊ:{filepath}");

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
