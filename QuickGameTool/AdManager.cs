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
    const string SDK_NAME = "com.qxqrj.ad.AdSDK";

    AndroidJavaClass jo =null;
    private Action _videoCB;

    protected override void Awake()
    {
        base.Awake();
        jo = new AndroidJavaClass(SDK_NAME);

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
    public  void ShowVideo()
    {
        try
        {
            Debug.Log("ShowVideo");

            jo.CallStatic("ShowVideo");
        }
        catch (Exception e)
        {
            Debug.LogError(e.Message + "ShowVideoFail!!!");

        }
    }


    #region 回调
    public void AfterPlayVideo(string status)
    {
        Debug.Log("AfterPlayVideo" + status);
        if (status == "success")
        {
            _videoCB?.Invoke();
        }
    }

    #endregion
}
