using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#region
//作者:Saber
#endregion
public class SDKUtility 
{

    public const string SDKCONFIG_PATH = "Assets\\Resources";
    public const string SDKCONFIG_NAME = "SDK_CONFIG.txt";

    public static string ReadSDKPackageName()
    {
        string s = "";
#if UNITY_EDITOR
        var path = $"{SDKCONFIG_PATH}\\{SDKCONFIG_NAME}";
        //if (!Directory.Exists(SDKCONFIG_PATH)) return s;
        if (!File.Exists(path)) return s;
        using (StreamReader reader = new StreamReader(path, System.Text.Encoding.ASCII))
        {
            s = reader.ReadToEnd();
            reader.Close();
        }
#else
        var path2 = SDKCONFIG_NAME.Remove(SDKCONFIG_NAME.Length - 4);
        s= Resources.Load<TextAsset>(path2).text;
#endif

        return s;
    }
}