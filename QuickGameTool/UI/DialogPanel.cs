using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
#region
//保持UTF-8
#endregion
public class DialogPanel : AutoSingleton<DialogPanel>
{
    [SerializeField]
    [Disable]
    bool IsGlobal = true;
    [Disable]
    protected string DialogPanelPath = "UI\\Dialog";
    [Disable]
    protected Transform DialogPanelParentTransform = null;
    [Disable]
    protected Canvas MianCanvas = null;

    protected Dictionary<string,GameObject> dialogDict=new Dictionary<string, GameObject> ();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="paneName"></param>
    /// <param name="path"></param>
    /// <param name="type">0为正常，1为设置为首，2为尾</param>
    /// <returns></returns>
    public static IUIBase Open(string paneName,string path=null,byte type=0)
    {
        if(Instance.dialogDict.TryGetValue(paneName,out GameObject g))
        {
            if (g == null)
            {
                Instance.dialogDict.Remove(paneName);
            }
            else
            {
                g.GetComponent<IUIBase>()?.Open();
                return g.GetComponent<IUIBase>();
            }
        }


        var parent = UIManager.Instance.CurrSceneMainCanvas;
        if (parent == null)
        {
            Debug.LogError("当前没有活性的画布！！");
            return null;
        }
        if(Instance.MianCanvas!=parent)
        {
            Instance.MianCanvas = parent;
            Transform parentTR = null;
            foreach (Transform tr in parent.transform)
            {
                if (tr.name.Contains("Dialog"))
                {
                    parentTR = tr;
                    break;
                }
            }
            Instance.DialogPanelParentTransform = parentTR;
            if (Instance.DialogPanelParentTransform == null)
            {
                parentTR = new GameObject("DialogPanel").transform;
                parentTR.SetParent(parent.transform);
                parentTR.SetAsLastSibling();
                parentTR.AddComponent<RectTransform>();
                var rectTr = ((RectTransform)parentTR);
                rectTr.anchorMin = new Vector2(0, 0);
                rectTr.anchorMax = new Vector2(1, 1);
                rectTr.offsetMin= new Vector2(0, 0);
                rectTr.offsetMax= new Vector2(0, 0);
                rectTr.localScale = new Vector2(1, 1);
                rectTr.localScale= new Vector2(1, 1);

                Instance.DialogPanelParentTransform= parentTR;
            }
        }

        if (path == null) path = Instance.DialogPanelPath;
        var go = AssestLoad.Load<GameObject>($"{path}\\{paneName}");
        var panel = GameObject.Instantiate(go, Instance.DialogPanelParentTransform);
        TimerManager.Instance.AddTimer(() => panel.GetComponent<IUIBase>()?.Open(), Time.deltaTime);
        Instance.dialogDict.Add(paneName, panel);

        switch (type)
        {
            case 1:
                panel.transform.SetAsFirstSibling();
                break;
            case 2:
                panel.transform.SetAsLastSibling();
                break;
        }

        return panel.GetComponent<IUIBase>();
    }

    public static void Close(string paneName)
    {
        if (!Instance.dialogDict.ContainsKey(paneName)) return;
        if (Instance.dialogDict[paneName] == null)
        {
            Instance.dialogDict.Remove(paneName);
            return;
        }
        instance.dialogDict[paneName]?.GetComponent<IUIBase>()?.Close();
        //Instance.dialogDict.Remove(paneName);
    }

    protected override void Awake()
    {
        base.Awake();
        if(IsGlobal)
            DontDestroyOnLoad(this.gameObject);
        
    }
    private void OnDestroy()
    {
        foreach(var v in  dialogDict.Values)
        {
            if (v != null)
            {
                GameObject.Destroy(v.gameObject);
            }
        }
    }

}
