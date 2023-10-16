using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region
//保持UTF-8
#endregion
public interface IGameInfo
{

    public int Level { get ; set; }
}
public abstract class GameManager : Singleton<GameManager>
{
    #region 关卡
    protected string levelDataName;

    protected IGameInfo gameInfo;
    [SerializeField]
    protected bool isTest;
    protected bool isNewPlayer;
    protected Action<IGameInfo> SaveGameInfoEvent;

    public int LevelNum { get => gameInfo.Level; }
    public bool IsNewPlayer { get => isNewPlayer;  }
    public IGameInfo GameInfo { get => gameInfo;  }

    protected abstract IGameInfo GetGameInfoAtFristEnter();
    //开始游戏
   protected virtual void ReadLevel<T>() where T: class,IGameInfo
    {
        gameInfo = JsonUtil.ReadData<T>(levelDataName);
        if (gameInfo == null)
        {
            Debug.Log("首次登录，新建存档！");
            gameInfo = GetGameInfoAtFristEnter();
            JsonUtil.Saver(levelDataName, gameInfo);
            isNewPlayer = true;
        }
    }
    public virtual void SaveGame()
    {
        SaveGameInfoEvent?.Invoke(gameInfo);
        JsonUtil.Saver(levelDataName, gameInfo);
    }
    #endregion
    #region Unity回调
    protected override void Awake()
    {
        base.Awake();
        levelDataName = $"{Application.persistentDataPath}/{Application.productName}_levelData";

    }
    protected virtual void Start()
    {

    }


    protected virtual void OnApplicationPause(bool pause)
    {
        if (pause)
        {
            SaveGameInfoEvent?.Invoke(gameInfo);
            JsonUtil.Saver(levelDataName, gameInfo);
        }

    }

    protected virtual void OnApplicationQuit()
    {
        SaveGameInfoEvent?.Invoke(gameInfo);
        SaveGameInfoEvent = null;
        JsonUtil.Saver(levelDataName, gameInfo);
    }
    #endregion

}
