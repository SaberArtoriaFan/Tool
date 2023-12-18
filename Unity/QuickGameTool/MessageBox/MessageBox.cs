using Saber.Base;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
#region
//保持UTF-8
#endregion
public class MessageBox : AutoSingleton<MessageBox>
{
    public class BannerHelper
    {
        string text;
        ShowPosition showPosition;
        float continueTime = 1.5f;
        bool isFade = true;
        float finishTime;
        bool isStarted = false;
        int fontSize = 24;
        Color fontColor=Color.white;
       TMPro.FontStyles fontStyle = FontStyles.Normal;
        Timer timer = null;
        private BannerHelper() { }
        public static BannerHelper Build(string text, ShowPosition showPosition, float continueTime, bool isFade,int fontSize=24,Color fontColor=default,FontStyles fontStyle=FontStyles.Normal)
        {
            if (!PoolManager.Instance.IsPoolAlive("BannerHelper"))
                PoolManager.Instance.AddPool(() => new BannerHelper(), (b) => b.m_Recycle(), null, "BannerHelper");
            var b=PoolManager.Instance.GetObjectInPool<BannerHelper>("BannerHelper");
            b.text = text;
            b.showPosition = showPosition;
            b.continueTime = continueTime;
            b.isFade = isFade;
            b.finishTime = 0;
            b.isStarted = false;
            b.fontSize = fontSize;
            b.fontStyle = fontStyle;
            if (fontColor != default)
                b.fontColor = fontColor;
            b.timer = null;
            //this.text = text;
            //this.showPosition = showPosition;
            //this.continueTime = continueTime;
            //this.isFade = isFade;
            return b;
        }
        public void Start()
        {
            finishTime = Time.time + continueTime;
            isStarted = true;

        }
        public void Stop()
        {
            finishTime=0;
            isStarted = true;
            if(timer!= null&&timer.IsFinish==false)
            {
                timer.Stop(true);
                timer = null;
            }
        }
        public void Recycle()
        {
            PoolManager.Instance.RecycleToPool(this, "BannerHelper");
        }
       void m_Recycle()
        {
            this.continueTime = 0;
            text = string.Empty;
            isStarted= false;
            fontSize = 24;
            fontColor = Color.white;
            fontStyle = FontStyles.Normal;

        }
        public string Text { get => text;  }
        public ShowPosition ShowPosition { get => showPosition;  }
        public float ContinueTime { get => continueTime; }
        public bool IsFade { get => isFade; }
        public float FinishTime { get => finishTime;  }
        public bool IsStarted { get => isStarted;  }
        public int FontSize { get => fontSize;  }
        public Color FontColor { get => fontColor;  }
        public FontStyles FontStyle { get => fontStyle;  }
        public Timer Timer { get => timer; set => timer = value; }
    }
    public enum ShowPosition
    {
        top,
        mid,
        down
    }

    public enum BoxType
    {
        banner,
        dialog
    }
    public bool isOpen = true;
    public GameObject bannerModel;
    public GameObject dialogModel;

    public float topInval = 50;
    public float botInval = 50;
    public float midScale = 0.9f;

    float screenWidth = 0;
    float edgeInval = 0;

    RectTransform bannerPanel=null;

    DialogBoxUI dialogBox = null;

    Dictionary<ShowPosition, List<BannerHelper>> bannerDict = new Dictionary<ShowPosition, List<BannerHelper>>();

    public RectTransform BannerPanel { get 
        {
            if (bannerPanel == null)
            {
                var go = new GameObject("BannerPanel");
                go.transform.SetParent(UIManager.Instance.CurrSceneMainCanvas.transform);
                bannerPanel = go.AddComponent<RectTransform>();
                bannerPanel.localScale = Vector3.one;
                bannerPanel.anchorMin = new Vector2(0, 0);
                bannerPanel.anchorMax = new Vector2(1, 1);
                bannerPanel.offsetMin = new Vector2(0, 0);
                bannerPanel.offsetMax = new Vector2(0, 0);
                bannerPanel.SetAsLastSibling();
            }
            return bannerPanel;
        } }
    private void Start()
    {
        screenWidth = Screen.width;
        edgeInval = screenWidth * (1-midScale) / 2;
        bannerDict.Add(ShowPosition.top, new List<BannerHelper>());
        bannerDict.Add(ShowPosition.mid, new List<BannerHelper>());
        bannerDict.Add(ShowPosition.down, new List<BannerHelper>());
    }
    void InitDialogBox()
    {
        var go = GameObject.Instantiate(dialogModel);
        dialogBox =go.GetComponent<DialogBoxUI>() ;
        if(dialogBox==null)dialogBox=go.AddComponent<DialogBoxUI>() ;
        dialogBox.transform.SetParent(BannerPanel);
        dialogBox.transform.SetAsFirstSibling();
        var rectTransform=dialogBox.transform as RectTransform;
        rectTransform.anchorMin = new Vector2(0, 0);
        rectTransform.anchorMax = new Vector2(1, 1);
        rectTransform.offsetMin = new Vector2(0, 0);
        rectTransform.offsetMax = new Vector2(0, 0);


        dialogBox.transform.localScale = Vector3.one;
        dialogBox.Close();
    }
    public DialogBoxUI Show(string text,string contentText,string confirmText="",Action confirmEvent=null)
    {
        if(dialogBox==null)InitDialogBox();
        dialogBox.Open(text,contentText, confirmText, confirmEvent);
        return dialogBox;
    }
    public void Show(string text,ShowPosition showPosition, bool isWait = true,float continueTime=1.5f,bool isFade=true)
    {
        var b= BannerHelper.Build(text, showPosition, continueTime, isFade);
        if (isWait|| bannerDict[b.ShowPosition].Count==0)
            bannerDict[b.ShowPosition].Add(b);
        else
        {
            bannerDict[b.ShowPosition][0].Stop();
            bannerDict[b.ShowPosition].Insert(1, b);
        }

    }
    private void ShowBanner(BannerHelper bannerHelper)
    {
        //实际显示，根据位置不同之类的显示到不同地方
        if (!PoolManager.Instance.IsPoolAlive("BannerDialogUI"))
            PoolManager.Instance.AddPool<TMP_Text>(() =>
            {
                var res= GameObject.Instantiate(bannerModel).GetComponentInChildren<TMP_Text>();
                res.transform.SetParent(BannerPanel);
                res.transform.localScale = Vector3.one;
                res.raycastTarget = false;
                return res;
            }, (u) =>
            {
                u.CrossFadeAlpha(1, 0, true);
                u.gameObject.SetActive(false);
            }, (u) =>
            {
                u.gameObject.SetActive(true);
                u.CrossFadeAlpha(1, 0, true);
            }, "BannerDialogUI");
        var model = PoolManager.Instance.GetObjectInPool<TMP_Text>("BannerDialogUI");
        var rectTransform=model.transform as RectTransform;
        switch (bannerHelper.ShowPosition)
        {
            case ShowPosition.top:
                rectTransform.anchorMin = new Vector2(0, 1);
                rectTransform.anchorMax = new Vector2(1, 1);
                rectTransform.pivot = new Vector2(0, 1);
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Top, topInval,rectTransform.rect.size.y);
                rectTransform.offsetMin = new Vector2(edgeInval, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(-1*edgeInval, rectTransform.offsetMax.y);
                break;
            case ShowPosition.mid:
                rectTransform.anchorMin = new Vector2(0, 0.5f);
                rectTransform.anchorMax = new Vector2(1, 0.5f);
                rectTransform.pivot = new Vector2(0, 0.5f);
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 0);
                rectTransform.offsetMin = new Vector2(edgeInval, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(-1 * edgeInval, rectTransform.offsetMax.y);
                break;
            case ShowPosition.down:
                rectTransform.anchorMin = new Vector2(0, 0);
                rectTransform.anchorMax = new Vector2(1, 0);
                rectTransform.pivot = new Vector2(0, 0);
                rectTransform.SetInsetAndSizeFromParentEdge(RectTransform.Edge.Bottom, botInval, rectTransform.rect.size.y);
                rectTransform.offsetMin = new Vector2(edgeInval, rectTransform.offsetMin.y);
                rectTransform.offsetMax = new Vector2(-1*edgeInval, rectTransform.offsetMax.y);
            break;      
        }
        model.text = bannerHelper.Text;
        model.fontStyle = bannerHelper.FontStyle;
        model.color = bannerHelper.FontColor;
        model.fontSize = bannerHelper.FontSize;
       
        if (bannerHelper.IsFade)
            model.CrossFadeAlpha(0, bannerHelper.ContinueTime, false);
        bannerHelper.Timer = TimerManager.Instance.AddTimer(() =>
        {
            PoolManager.Instance.RecycleToPool(model, "BannerDialogUI");
        }, 
        bannerHelper.ContinueTime);
    }

    private void EndShowBanner(BannerHelper bannerHelper)
    {
        
    }
    private void Update()
    {
        if (isOpen)
        {
            List<BannerHelper> list = null;
            for(int i=0;i<3;i++)
            {
                list = bannerDict[(ShowPosition)i];
                while (list.Count > 0)
                {
                    if (list[0].IsStarted == false)
                    {
                        list[0].Start();
                        ShowBanner(list[0]);
                        break;
                    }
                    else if (list[0].FinishTime <= Time.time)
                    {
                        EndShowBanner(list[0]);
                        list[0].Recycle();
                        list.RemoveAt(0);
                    }
                    else
                        break;
                }
            }
        }
    }

}
