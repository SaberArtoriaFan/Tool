using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
#region
//作者:Saber
#endregion
public class SDKUtility 
{


    /// <summary>
    /// 请勿轻易修改SDKFunc已有枚举的值
    /// 如果需要修改请新增！！！
    /// 一定要修改原值的话请删除SDKCONFIG文件重新生成，不然会乱序
    /// </summary>
    public enum SDKFunc:byte
    {
        ShowSplash=1,
        ShowBanner=2,
        CloseBanner=3,
        ShowVideo=4,
        showGooglePlayAssess=5,
        ShowInterstitial=6
    }
    //回调参数
    public enum SDKCallbackPara:byte
    {
        Succ=1,
        fail=2,
        error=3
    }
    [Serializable]
    public class CallbackPara
    {
        [SerializeField]
        public  byte id;
        [SerializeField]
        public byte funcID;
        [SerializeField]
        public string callName;

    }
    [Serializable]
    public class AndroidCallback
    {
        public string CallbackName;
        public CallbackPara[] callbackParas;
    }
    // [DataContract]
    [Serializable]
    public class SDKConfig
    {
        // [DataMember]
        [SerializeField]
        string sDKNAME;
        [SerializeField]
        AndroidJavaCall[] sdkFuncArray;
        //[SerializeField]
        //CallbackPara[] sdkCallbackPara=null;

        public SDKConfig(string sDKNAME, AndroidJavaCall[] sdkFuncArray = null)
        {
            this.sDKNAME = sDKNAME;
            this.sdkFuncArray = sdkFuncArray;
            //sdkCallbackPara = new CallbackPara[0];
        }

        public string SDKNAME { get=>sDKNAME; set=>sDKNAME=value; }

        public AndroidJavaCall[] SdkCallArray { get => sdkFuncArray; set => sdkFuncArray = value; }
        //public CallbackPara[] SdkCallbackPara { get => sdkCallbackPara; set => sdkCallbackPara = value; }

        //public CallbackPara[] GetFuncCallBackParas(byte id)
        //{
        //    var list=new List<CallbackPara>();
        //    for(int i = 0; i < sdkCallbackPara.Length; i++)
        //    {
        //        if (sdkCallbackPara[i].funcID == id)
        //            list.Add(sdkCallbackPara[i]);
        //    }
        //    return list.ToArray();
        //}

        //public void RemoveRepeatPara(byte id)
        //{
        //    List<CallbackPara> callbackParas = new List<CallbackPara>(sdkCallbackPara);
        //    for(int i=0;i< callbackParas.Count;i++)
        //    {
        //        if (callbackParas[i].funcID == id)
        //            callbackParas.RemoveAt(i);
        //    }
        //    sdkCallbackPara = callbackParas.ToArray();
        //}
    }
    [Serializable]
    public class AndroidJavaCall
    {
        [SerializeField]
        byte id;
        [SerializeField]
        string funcName;
        [SerializeField]
        public int maxParametersNum = int.MaxValue;
        [SerializeField]
        public int minParametersNum = 0;

        public AndroidJavaCall(byte id ,string funcName="", int maxParametersNum = int.MaxValue, int minParametersNum = 0)
        {
           // if (funcName == "") funcName = string.Empty;
            this.funcName = funcName;
            this.maxParametersNum = maxParametersNum;
            this.minParametersNum = minParametersNum;
            this.id = id;
        }

        public string FuncName { get => funcName; set => funcName = value; }
        public byte Id { get => id; }

        //public byte Id { get => id; set => id = value; }

        public void CallStatic(params object[] objects)
        {
#if UNITY_EDITOR
            return;
#endif

            var paraNum = objects == null ? 0 : objects.Length;
            if (paraNum < minParametersNum || paraNum > maxParametersNum)
            {
                Debug.LogWarning($"{funcName}方法，传入的参数{objects}似乎与设置的参数数量不一致:min-{minParametersNum},curr-{paraNum},max-{maxParametersNum}");
            }
            try
            {
                AdManager.Instance.Jo.CallStatic(funcName, objects);
            }
            catch (Exception e)
            {
                Debug.LogError("CallAndroid出错:" + e);
            }
        }
    }


    public const string Default_SDKCONFIG_PATH = "Assets\\Resources";
    public const string Default_SDKCONFIG_PATH_FileName = "SDKCONFIGScriptObject.asset";

    public static string SDKCONFIG_PATH { get=> SDKEditorUtility.SafeGetSDKPathConfig().SDKCONFIG_PATH; 
        set
        {
            var v = SDKEditorUtility.SafeGetSDKPathConfig();
            v.SDKCONFIG_PATH= value;
            v.SafeSave();
        } }
    public static string SDKCONFIG_NAME => SDKEditorUtility.SafeGetSDKPathConfig().SDKCONFIG_NAME;
    public static string SDKCONFIG_FullPath 
    {
        get
        {
            var config = SDKEditorUtility.SafeGetSDKPathConfig();
           return Path.Combine(config.SDKCONFIG_PATH, config.SDKCONFIG_NAME);
        }
    }
    public const string SDKCONFIG_WINDOW_PATH = "Tools/SDK/Config";
    public static SDKConfig ReadConfig(SDKConfigEditor sDKConfig =null)
    {
        if (sDKConfig == null) sDKConfig = SDKEditorUtility.SafeGetSDKPathConfig();
        string s = "";
        bool isPlaying = false;
#if UNITY_EDITOR
        isPlaying = AdManager.Instance != null;
#else
        isPlaying=Application.isPlaying;
#endif
        var path = Path.Combine(sDKConfig.SDKCONFIG_PATH, sDKConfig.SDKCONFIG_NAME);

        if (!isPlaying)
        {
            //if (!Directory.Exists(SDKCONFIG_PATH)) return s;
            if (!File.Exists(path)) return null;
            SDKConfig v = null;
            using (var reader = new StreamReader(path))
            {
                var str = reader.ReadToEnd();
                v = JsonUtility.FromJson<SDKConfig>(str);
                reader.Close();
                reader.Dispose();
            }

            //= JsonUtil.ReadData<SDKConfig>(path);
            return v;
        }
        else
        {
            //var path2 = SDKCONFIG_NAME.Remove(SDKCONFIG_NAME.Length - 4);
            //path = Path.Combine(SDKUtility.Default_SDKCONFIG_PATH, SDKUtility.Default_SDKCONFIG_PATH_FileName);
            path = path.Remove(0, path.LastIndexOf("Resources") + ("Resources").Length + 1);
            path= path.Replace(sDKConfig.SDKCONFIG_NAME, Path.GetFileNameWithoutExtension(sDKConfig.SDKCONFIG_NAME));
            var fileName = path;
            try
            {
                s = Resources.Load<TextAsset>(fileName).text;
            }
            catch
            {
                Debug.LogError($"加载SDK配置路径->[{fileName}]出现错误!!!");
                return null;
            }
            var v = JsonUtility.FromJson<SDKConfig>(s);
            return v;
        }
    }
    public static string ReadSDKPackageName() => ReadConfig()?.SDKNAME;
}
