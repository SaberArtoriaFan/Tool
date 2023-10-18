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

    public const string SDKCONFIG_WINDOW_PATH = "Tool/SDKConfig";
    public static string ReadSDKPackageName(bool isPlaying = false)
    {
        string s = "";

#if !UNITY_EDITOR
            var path2 = SDKCONFIG_NAME.Remove(SDKCONFIG_NAME.Length - 4);
            s = Resources.Load<TextAsset>(path2).text;
            s = s.Replace("\r\n", "");
            return s;
#endif

        if (!isPlaying)
        {
            var path = $"{SDKCONFIG_PATH}\\{SDKCONFIG_NAME}";
            //if (!Directory.Exists(SDKCONFIG_PATH)) return s;
            if (!File.Exists(path)) return s;
            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.ASCII))
            {
                s = reader.ReadToEnd();
                reader.Close();
            }
        }
        else
        {

            var path3 = SDKCONFIG_NAME.Remove(SDKCONFIG_NAME.Length - 4);
            s = Resources.Load<TextAsset>(path3).text;
            s = s.Replace("\r\n", "");
        }




        return s;
    }
}
