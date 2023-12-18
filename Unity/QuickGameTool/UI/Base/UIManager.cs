using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Saber.Base;
public class UIManager : AutoSingleton<UIManager>
{
    Dictionary<string, Dictionary<string, GameObject>> AllWedgate;
    Dictionary<string, IUIBase> allUIPanel;

    Canvas canvas;
    public Canvas CurrSceneMainCanvas { get 
        {
            if (canvas != null && canvas.enabled == true && canvas.gameObject.activeSelf == true) return canvas;
            else
            {
                var canvases = FindObjectsOfType<Canvas>();
                canvas = null;
                foreach(var c in canvases)
                {
                    if(c != null && c.enabled == true && c.gameObject.activeSelf == true)
                    {
                        if(c.gameObject.name.Contains("Main"))
                        {
                            canvas = c;
                            break;
                        }
                        if(canvas==null)
                            canvas = c;
                    }
                }
                return canvas; 
            }
        }
             }


    public void ResetSelf()
    {
        AllWedgate.Clear();
    }
    //注册物体与Base，用字典管理
    #region GetWithRegister
    public GameObject GetWedgateGameObject(string PanelName, string WedgateName)
    {
        if (AllWedgate.ContainsKey(PanelName))
        {
            GameObject WedgateGameObject = AllWedgate[PanelName][WedgateName];
            return WedgateGameObject;
        }
        return null;
    }
    public void RegisterWedgate(string PanelName, string WedgateName, GameObject gameObject)
    {
        if (!AllWedgate.ContainsKey(PanelName))
        {
            AllWedgate[PanelName] = new Dictionary<string, GameObject>();
        }
        AllWedgate[PanelName].Add(WedgateName, gameObject);
    }
    #endregion
    #region Destroy
    public void DestroyWedgate(string PanelName, string WedgateName)
    {
        if (AllWedgate.ContainsKey(PanelName))
        {
            AllWedgate[PanelName].Remove(WedgateName);
        }
    }
    public  void DestroyPanel(string PanelName)
    {
        if (AllWedgate.ContainsKey(PanelName))
        {
            AllWedgate[PanelName].Clear();
            AllWedgate.Remove(PanelName);
        }
        if (Instance.allUIPanel.ContainsKey(PanelName))
            Instance.allUIPanel.Remove(PanelName);
    }
    #endregion
    public static void RegisterPanel(IUIBase uIBase)
    {
        if (Instance.allUIPanel.ContainsKey(uIBase.Name)) {Debug.LogError("层级重复出现！！"+uIBase.Name);return; }
        else Instance.allUIPanel.Add(uIBase.Name, uIBase);
    }
    public static T GetPanel<T>()where T : UIBase<T>
    {
        string s = typeof(T).Name;
        if (Instance.allUIPanel.ContainsKey(s))
            return Instance.allUIPanel[s] as T;
        else { Debug.LogError("该层级未找到！！");return null; }
    }

   
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
        AllWedgate = new Dictionary<string, Dictionary<string, GameObject>>();
        allUIPanel = new Dictionary<string, IUIBase>();
        var v = CurrSceneMainCanvas;
    }

}