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
    private RawImage rawImage;
    string canvansName = "SceneFadeCanvas";
    Coroutine coroutine;
    Canvas canvas;

    void Start()
    {
         canvas =GameObject.Instantiate(AssestLoad.Load<Canvas>(canvansName),this.transform.parent);
        rawImage =canvas.GetComponentInChildren<RawImage>();
        canvas.gameObject.SetActive(false);

    }

    public void FadeScene(Func<bool> endFunc)
    {
        if (coroutine==null)
            coroutine = StartCoroutine(IEStartSceneFade(endFunc)) ;
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
        canvas.gameObject.SetActive(true);

        yield return new WaitUntil(() =>
        {
            EndScene(endFunc());
            return sceneStarting == false;
        });
        yield return new WaitUntil(endFunc);
        sceneStarting = true;
        yield return new WaitForSeconds(0.7f);

        yield return new WaitUntil(() =>
        {
            StartScene();
            return sceneStarting == false;
        });
        coroutine = null;
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
