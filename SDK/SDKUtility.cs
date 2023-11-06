using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using UnityEngine;
#region
//作者:Saber
#endregion
public class SDKUtility 
{
    [DataContract]
    public class SDKConfig
    {
        [DataMember]
        string sDKNAME;

        public SDKConfig(string sDKNAME)
        {
            this.sDKNAME = sDKNAME;
        }

       public string SDKNAME => sDKNAME;
    }

    public const string SDKCONFIG_PATH = "Assets\\Resources";
    public const string SDKCONFIG_NAME = "SDK_CONFIG.json";

    public const string SDKCONFIG_WINDOW_PATH = "Tool/SDKConfig";
    public static string ReadSDKPackageName()
    {
        string s = "";
        bool isPlaying = false;
#if UNITY_EDITOR
        isPlaying = AdManager.Instance!=null;
#else
        isPlaying=Application.isPlaying;
#endif
        var path = Path.Combine(SDKCONFIG_PATH, SDKCONFIG_NAME);

        if (!isPlaying)
        {
            //if (!Directory.Exists(SDKCONFIG_PATH)) return s;
            if (!File.Exists(path)) return s;
            var v= JsonUtil.ReadData<SDKConfig>(path);
            return v==null? string.Empty : v.SDKNAME;
            using (StreamReader reader = new StreamReader(path, System.Text.Encoding.ASCII))
            {
                s = reader.ReadLine();
                reader.Close();
            }
            return s;
        }
        else
        {
            //var path2 = SDKCONFIG_NAME.Remove(SDKCONFIG_NAME.Length - 4);
            s = AssestLoad.Load<TextAsset>(path).text;
            var v = JsonConvert.DeserializeObject<SDKConfig>(s);
            return v == null ? string.Empty : v.SDKNAME;
        }

    }
}
