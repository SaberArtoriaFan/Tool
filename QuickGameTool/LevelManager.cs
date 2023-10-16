using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region
//保持UTF-8
#endregion



public  class LevelManager : Singleton<LevelManager>
{
    public event Action<int> OnLevelEnterEvent;
    public event Action<int> OnLevelExitEvent;


    protected int currLevel = 1;
    protected int currMaxLevel = 1;
    protected int maxLevel = 10;

    public int CurrLevel { get => currLevel;  }
    public int CurrMaxLevel { get => currMaxLevel;  }
    public int MaxLevel { get => maxLevel;  }

    protected virtual void OnDestroy()
    {
        OnLevelEnterEvent = null;
        OnLevelExitEvent = null;

    }
    //进入关卡
    public virtual bool EnterCurrLevel()
    {
        if (currLevel > maxLevel)
        {
            Debug.LogError("当前关卡已超出");
            return false;
        }

        OnLevelEnterEvent?.Invoke(currLevel);

        return true;
    }
    //读取关卡
    public virtual void ReadLevel(int curr)
    {
        this.currLevel = curr;
    }
    //退出关卡
    public virtual void ExitLevel(bool isComplete)
    {
        OnLevelExitEvent?.Invoke(currLevel);
        if (isComplete)
            CompleteLevel();
    }
    //下一关
    public virtual bool NextLevel(bool isComplete)
    {
        ExitLevel(isComplete);
        currLevel++;
        return EnterCurrLevel();
    }
    public virtual void CompleteLevel()
    {
        currMaxLevel=Mathf.Max(currLevel, maxLevel);
        if(currLevel>GameManager.Instance.LevelNum)
            GameManager.Instance.SaveGame();
        Debug.Log($"通关{currLevel}");
    }
}
