using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
#region
//保持UTF-8
#endregion

public class SceneFadeInOut : Singleton<SceneFadeInOut>
{
    public float fadeSpeed = 1.5f;
    public bool sceneStarting = true;
    [HideInInspector]
    public RawImage rawImage;
    [Header("如果没拖动序列化Canvas那么将根据这个名字使用AssestLoad.Load方法加载")]
    [SerializeField]
    string canvansName = "SceneFadeCanvas";
    Coroutine coroutine;
    
    public Canvas canvas;

    void Start()
    {
        if (canvas == null)
            canvas = GameObject.Instantiate(AssestLoad.Load<Canvas>(canvansName), this.transform.parent);
        else
            canvas = GameObject.Instantiate(canvas,transform.parent);
       rawImage =canvas.GetComponentInChildren<RawImage>();
        canvas.gameObject.SetActive(false);

    }

    public void FadeScene(Func<bool> endFunc)
    {
 
            canvas.gameObject.SetActive(true);
             StartCoroutine(IEStartSceneFade(endFunc));
    }

    private void FadeToClear()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.clear, fadeSpeed * Time.deltaTime);
    }

    private void FadeToBlack()
    {
        rawImage.color = Color.Lerp(rawImage.color, Color.black, fadeSpeed * Time.deltaTime);
    }
    IEnumerator IEStartSceneFade(Func<bool> endFunc)
    {
        sceneStarting = true;
        rawImage.enabled = true;
        rawImage.color = Color.clear;
        yield return new WaitUntil(() =>
        {
            EndScene(endFunc());
            return sceneStarting == false;
        });
        yield return new WaitUntil(endFunc);
        sceneStarting = true;
        yield return new WaitForSeconds(0.4f);

        yield return new WaitUntil(() =>
        {
            StartScene();
            return sceneStarting == false;
        });

        coroutine = null;
        fadeSpeed = 1.5f;

        canvas.gameObject.SetActive(false);
    }
    void StartScene()
    {
        FadeToClear();
        if (rawImage.color.a < 0.05f)
        {
            rawImage.color = Color.clear;
            rawImage.enabled = false;
            sceneStarting = false;
        }
    }

    void EndScene(bool isFinish)
    {
        rawImage.enabled = true;
        FadeToBlack();
        if (isFinish||rawImage.color.a > 0.95f)
        {
            rawImage.color = Color.black;
            sceneStarting = false;

        }
    }

    void OnDestroy()
    {

    }
}
