using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
#region
//保持UTF-8
#endregion
public class ClickManager : Singleton<ClickManager>
{
    bool isClickOnUI = false;
    private  GraphicRaycaster raycaster = null;


    [SerializeField]
    /// <summary>
    /// 0表示开冰箱，1表示关冰箱，2表示开抽屉，3表示关抽屉，4表示放物体，5表示返还物体
    /// </summary>
    int clickType = 0;

    bool isCanClick = true;
    Dictionary<int, Func<RaycastHit,bool>> clickEventDict=new Dictionary<int, Func<RaycastHit,bool>>();
    Dictionary<int, Func<RaycastHit, bool>> dragEventDict = new Dictionary<int, Func<RaycastHit, bool>>();

    public bool IsClickOnUI { get => isClickOnUI; }
    bool IsMouseOnUI()
    {
        List<RaycastResult> allResults = new List<RaycastResult>();
        //�ж�����Ƿ���UI��
        allResults.Clear();
        PointerEventData data = new PointerEventData(EventSystem.current);
        data.position = Input.mousePosition;
        data.pressPosition = Input.mousePosition;
        //Debug.Log(AllResults.Count);
        if (raycaster == null) raycaster = GraphicRaycaster.FindObjectOfType<GraphicRaycaster>();
        raycaster.Raycast(data, allResults);
        //Debug.Log("鼠标在UI上" + allResults.Count);
        return allResults.Count >= 1;
    }
    // Update is called once per frame
    void Update()
    {
        isClickOnUI=IsMouseOnUI();
        if (isClickOnUI) return;
        if (isCanClick == false) return;
        int count = Input.touchCount;
        for(int i = 0; i < count; i++)
        {
            var v= Input.GetTouch(i);
            
            if (v.phase == TouchPhase.Ended)
            {
                Ray ray=Camera.main.ScreenPointToRay(v.position);
                RaycastHit[] res = new RaycastHit[12];
                if (Physics.RaycastNonAlloc(ray, res, 100) > 0)
                {
                    foreach(var r in res)
                    {
                        if (r.Equals(default)||r.collider?.isTrigger==false) continue;
                        if (CheckRayHit(r)) return;
                    }
                }
                break;
            }
            else if(v.phase== TouchPhase.Moved) 
            {
                Ray ray = Camera.main.ScreenPointToRay(v.position);
                RaycastHit[] res = new RaycastHit[12];
                if (Physics.RaycastNonAlloc(ray, res, 100) > 0)
                {
                    foreach (var r in res)
                    {
                        if (r.Equals(default) || r.collider?.isTrigger == false) continue;
                        if (CheckDrag(r)) return;
                    }
                }
                break;
            }

        }
    }

    public void ChangeClickType(int type)
    {
        this.clickType = type;
    }
    public void AddClickEvent(int id, Func<RaycastHit,bool> func)
    {
        if (clickEventDict.ContainsKey(id))
        {
            Debug.LogError("不能重复加入事件");
            return;
        }else
            clickEventDict.Add(id, func);
    }

    public void AddDragEvent(int id, Func<RaycastHit, bool> func)
    {
        if (dragEventDict.ContainsKey(id))
        {
            Debug.LogError("不能重复加入事件");
            return;
        }
        else
            dragEventDict.Add(id, func);
    }
    public void ApplyPlayAnim(float delay)
    {
        if (isCanClick == false) return;
        isCanClick = false;
        TimerManager.Instance.AddTimer(() =>
        {
            isCanClick = true;
        }, delay);
    }
    private bool CheckRayHit(RaycastHit r)
    {
        if (clickEventDict.TryGetValue(clickType,out var v))
        {
            return clickEventDict[clickType](r);
        }else
            return true;
    }
    private bool CheckDrag(RaycastHit r)
    {
        if (dragEventDict.TryGetValue(clickType, out var v))
        {
            return dragEventDict[clickType](r);
        }
        else
            return true;
    }
}
