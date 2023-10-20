using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
#region
//保持UTF-8
#endregion
public class AdManager : Singleton<AdManager>
{
    [Space(10)]
    [SerializeField]
    [Disable]
    string SDK_NAME = "";

    AndroidJavaClass jo =null;
    private Action succ_VideoAction;
    private Action fail_VideoAction;
    private Action error_VideoAction;

    protected override void Awake()
    {
        base.Awake();
        SDK_NAME = SDKUtility.ReadSDKPackageName(true);
        jo = new AndroidJavaClass(SDK_NAME);
        Debug.Log($"---------SDK:[{SDK_NAME}]------------");

    }
    protected void Start()
    {

        gameObject.name = "AdManager";
        SceneManager.activeSceneChanged += SceneChangeBanner;
    }
    private void OnDestroy()
    {
        SceneManager.activeSceneChanged -= SceneChangeBanner;

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
        try
        {
            Debug.Log("拉起谷歌商店");
            jo.CallStatic("showGooglePlayAssess");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "showGooglePlayAssessFail!!!");
        }

    }

    public  void ShowBanner(string position)
    {
        try
        {
            Debug.Log("showBanner");

            jo.CallStatic("ShowBanner", new object[] { position });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "ShowBanner!!!");

        }

    }
    public  void CloseBanner()
    {
        try
        {
            Debug.Log("CloseBanner");

            jo.CallStatic("CloseBanner");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "CloseBannerFail!!!");

        }
    }
    public  void ShowInterstitial(string type)
    {
        try
        {
            Debug.Log("ShowInterstitial");
            jo.CallStatic("ShowInterstitial", new object[] { type });
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "ShowInterstitialFail!!!");
        }
    }
    public  void ShowVideo(Action succ=null,Action fail=null,Action error = null)
    {
#if UNITY_EDITOR
        succ?.Invoke();
        return;

#endif

        try
        {
            Debug.Log("ShowVideo");

            jo.CallStatic("ShowVideo");
            succ_VideoAction = succ;
            fail_VideoAction = fail;
            error_VideoAction = error;
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "ShowVideoFail!!!");

        }
    }


    #region 回调
    public void AfterPlayVideo(string status)
    {
        Debug.Log("接受回调AfterPlayVideo" + status);
        if (status == "success")
        {
            succ_VideoAction?.Invoke();
        }
        else if (status == "fail")
            fail_VideoAction?.Invoke();
        else
            error_VideoAction?.Invoke();

        succ_VideoAction = null;
        fail_VideoAction = null;
        error_VideoAction = null;
    }

    #endregion
}
