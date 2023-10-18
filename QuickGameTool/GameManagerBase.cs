using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region
//保持UTF-8
#endregion
public abstract class GameManagerBase : Singleton<GameManagerBase>
{
    #region 关卡
    protected string levelDataName;


    [SerializeField]
    protected bool isTest;

    protected bool isNewPlayer;


    public abstract int LevelNum { get; }
    public bool IsNewPlayer { get => isNewPlayer; }


    //开始游戏
    protected virtual void ReadLevel()
    {

    }
    public virtual void SaveGame()
    {

    }

    public virtual void StartGame()
    {

    }
    #endregion

}
