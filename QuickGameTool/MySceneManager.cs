using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#region
//保持UTF-8
#endregion
public class MySceneManager : Singleton<MySceneManager>
{

    private float loadingSpeed = 1;  //加载速度，这里是进度条的读取速度

    private float targetValue;  //进度条目标的值/异步加载进度的值

    private AsyncOperation asyncLoad;  //定义异步加载的引用

    // Use this for initialization  
    void Start()
    {

        //if (SceneManager.GetActiveScene().name == "B")  //如果当前活动场景是B
        //{
        //    AsyncLoad()
        //    //启动协程  
        //    //StartCoroutine(AsyncLoading());  //开启协程进行异步加载
        //}
    }
    //public void AsyncLoad(string sceneName)
    //{
    //    asyncLoad = SceneManager.LoadSceneAsync(sceneName);
    //    //阻止当加载完成自动切换
    //    asyncLoad.allowSceneActivation = false;
    //    loadingSlider.value = 0.0f;  //初始化进度条

    //}
    public void AsyncLoadScene(string sceneName,Action onChangeScene,Action<float> changingPer)
    {
        if (asyncLoad == null)
        {
            if(changingPer==null)
                StartCoroutine(AsyncLoading(sceneName, onChangeScene));
            else
                StartCoroutine(AsyncLoadingWithSlider(sceneName, onChangeScene,changingPer));

        }
    }
    IEnumerator AsyncLoading(string sceneName,Action onChangeScene)
    {
        bool control = false;
        SceneFadeInOut.Instance.sceneStarting = true;
        SceneFadeInOut.Instance.FadeScene(() => control);

        yield return new WaitUntil(() => SceneFadeInOut.Instance.sceneStarting==false);


        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        //阻止当加载完成自动切换
        asyncLoad.allowSceneActivation = false;


        asyncLoad.completed += (u) =>
        {
            asyncLoad = null;
            onChangeScene?.Invoke();
        };

        yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);
        control = true;
        //yield return new WaitForSeconds(0.5f);
        asyncLoad.allowSceneActivation = true;


    } // Update is called once per frame 
    IEnumerator AsyncLoadingWithSlider(string sceneName, Action onChangeScene, Action<float> changingPer)
    {
        float loadingValue = 0;
        SceneFadeInOut.Instance.canvas.gameObject.SetActive(true);
        asyncLoad = SceneManager.LoadSceneAsync(sceneName);
        //阻止当加载完成自动切换
        asyncLoad.allowSceneActivation = false;
        targetValue = 0f;
        changingPer(targetValue);
        while (true)
        {
            targetValue = asyncLoad.progress;
            if (asyncLoad.progress >= 0.9f)
            {
                //progress的值最大为0.9 
                targetValue = 1.0f;
                loadingSpeed = 3f;
            }
            if (targetValue != loadingValue)
            { //插值运算 
                loadingValue = Mathf.Lerp(loadingValue, targetValue, Time.deltaTime * loadingSpeed);
                changingPer(loadingValue);
                if (Mathf.Abs(loadingValue - targetValue) < 0.01f)
                //如果当前进度条value和目标值接近 设置进度条value为目标值 
                {
                    changingPer(targetValue);
                    loadingSpeed = 1;
                    break;
                }
            }
            yield return new WaitForSeconds(Time.deltaTime);
        }


        bool control = false;
        SceneFadeInOut.Instance.sceneStarting = true;
        SceneFadeInOut.Instance.FadeScene(() => control);

        yield return new WaitUntil(() => SceneFadeInOut.Instance.sceneStarting == false);


  


        asyncLoad.completed += (u) =>
        {
            asyncLoad = null;
            onChangeScene?.Invoke();
        };

        //yield return new WaitUntil(() => asyncLoad.progress >= 0.9f);
        control = true;
        //yield return new WaitForSeconds(0.5f);
        asyncLoad.allowSceneActivation = true;


    }
    //void Update()
    //{
    //    if (asyncLoad == null) return;
    //    targetValue = asyncLoad.progress;
    //    if (asyncLoad.progress >= 0.9f)
    //    {
    //        //progress的值最大为0.9 
    //        targetValue = 1.0f;
    //    }
    //    if (loadingSlider == null)
    //    {
    //        if (targetValue != loadingSlider.value)
    //        { //插值运算 
    //            loadingSlider.value = Mathf.Lerp(loadingSlider.value, targetValue, Time.deltaTime * loadingSpeed);
    //            if (Mathf.Abs(loadingSlider.value - targetValue) < 0.01f)
    //            //如果当前进度条value和目标值接近 设置进度条value为目标值 
    //            { loadingSlider.value = targetValue; }
    //        }
    //        loadingText.text = ((int)(loadingSlider.value * 100)).ToString() + "%";
    //        if ((int)(loadingSlider.value * 100) == 100)
    //        //当进度条读取到百分之百时允许场景切换 
    //        { //允许异步加载完毕后自动切换场景 
    //            asyncLoad.allowSceneActivation = true;
    //            asyncLoad = null;
    //        }
    //    }
    //    else if (targetValue == 1.0f)
    //    {
    //        asyncLoad.allowSceneActivation = true;
    //        asyncLoad = null;
    //    }

    //}


}
