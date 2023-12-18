using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UIElements;
#region
//保持UTF-8
#endregion

public class AdManager : Singleton<AdManager>
{



    [Space(10)]
    [SerializeField]
    [Disable]
    string SDK_NAME = "";
    [Space(10)]
    public bool OpenAd = true;

    SDKUtility.SDKConfig SDKConfig;
    AndroidJavaClass jo =null;
    private Action succ_VideoAction;
    private Action fail_VideoAction;
    private Action error_VideoAction;

    Dictionary<SDKUtility.SDKFunc,SDKUtility.AndroidJavaCall> androidCallDict=new Dictionary<SDKUtility.SDKFunc, SDKUtility.AndroidJavaCall> ();

    Dictionary<SDKUtility.SDKFunc,Dictionary<SDKUtility.SDKCallbackPara,SDKUtility.CallbackPara>> androidCallBackParaDict=new Dictionary<SDKUtility.SDKFunc, Dictionary<SDKUtility.SDKCallbackPara, SDKUtility.CallbackPara>> ();

    public AndroidJavaClass Jo { get => jo; }

    protected override void Awake()
    {
        base.Awake();
        SDKConfig=SDKUtility.ReadConfig();
        SDK_NAME = SDKConfig?.SDKNAME;
        foreach(var v in SDKConfig.SdkCallArray) 
            androidCallDict.Add((SDKUtility.SDKFunc)v.Id, v);
        //foreach (var v in SDKConfig.SdkCallbackPara)
        //{
        //    if (androidCallBackParaDict.ContainsKey((SDKUtility.SDKFunc)v.funcID) == false)
        //        androidCallBackParaDict.Add((SDKUtility.SDKFunc)v.funcID, new Dictionary<SDKUtility.SDKCallbackPara, SDKUtility.CallbackPara>());
        //    androidCallBackParaDict[(SDKUtility.SDKFunc)v.funcID].Add((SDKUtility.SDKCallbackPara)v.id, v);
        //}

        Debug.Log($"----SDK_Path:[{SDKUtility.SDKCONFIG_FullPath}]------------");
        Debug.Log($"---------SDK:[{SDK_NAME}]------------");
        jo = new AndroidJavaClass(SDK_NAME);
        
        //非编辑器下默认开启
#if !UNITY_EDITOR
        OpenAd = true;
#endif

    }
    protected void Start()
    {

        gameObject.name = "AdManager";
        //SceneManager.activeSceneChanged += SceneChangeBanner;
    }
    private void OnDestroy()
    {
        //SceneManager.activeSceneChanged -= SceneChangeBanner;

    }
    private void SceneChangeBanner(Scene arg0, Scene arg1)
    {
        if (arg1.name == "Main")
            ShowBanner("down");
        else
            CloseBanner();
    }

    public  void showGooglePlayAssess()
    {
        if(androidCallDict.TryGetValue(SDKUtility.SDKFunc.showGooglePlayAssess,out var call))
        {
            Debug.Log("拉起谷歌商店");
            call.CallStatic();
        }           
    }

    public  void ShowBanner(string position)
    {
        if (androidCallDict.TryGetValue(SDKUtility.SDKFunc.ShowBanner, out var call))
        {
            Debug.Log("展示横幅");
            call.CallStatic(position);
        }


    }
    public  void CloseBanner()
    {
        if (androidCallDict.TryGetValue(SDKUtility.SDKFunc.CloseBanner, out var call))
        {
            Debug.Log("关闭横幅");
            call.CallStatic();
        }
    }
    public  void ShowInterstitial(string type)
    {
        if (androidCallDict.TryGetValue(SDKUtility.SDKFunc.ShowInterstitial, out var call))
        {
            Debug.Log("展示插屏");
            call.CallStatic(type);
        }
    }
    public  void ShowVideo(Action succ=null,Action fail=null,Action error = null)
    {
#if UNITY_EDITOR
        succ?.Invoke();
        return;

#endif
        succ_VideoAction = succ;
        fail_VideoAction = fail;
        error_VideoAction = error;
        if (androidCallDict.TryGetValue(SDKUtility.SDKFunc.ShowVideo, out var call))
        {
            Debug.Log("展示视频");
            call.CallStatic();
        }
    }


    #region 回调
    public void AfterPlayVideo(string status)
    {
        Debug.Log("接受回调AfterPlayVideo" + status);
        if (status == androidCallBackParaDict[SDKUtility.SDKFunc.ShowVideo][SDKUtility.SDKCallbackPara.Succ].callName)
        {
            succ_VideoAction?.Invoke();
        }
        else if (status == androidCallBackParaDict[SDKUtility.SDKFunc.ShowVideo][SDKUtility.SDKCallbackPara.fail].callName)
            fail_VideoAction?.Invoke();
        else
            error_VideoAction?.Invoke();

        succ_VideoAction = null;
        fail_VideoAction = null;
        error_VideoAction = null;
    }

    #endregion
}
