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
public abstract class GameManager<T> : GameManagerBase where T: class,IGameInfo
{
    #region 关卡

    protected T gameInfo;

    protected bool isNeedSaveGame = true;

    public event Action<T> SaveGameInfoEvent;

    public override int LevelNum { get => gameInfo.Level; }
    public T GameInfo { get => gameInfo;  }

    /// <summary>
    /// 定制自己的GameInfo，并决定他的初始化方式
    /// </summary>
    /// <returns></returns>
    protected abstract T GetGameInfoAtFristEnter();
    //开始游戏
   protected override void ReadLevel()
    {
        gameInfo = JsonUtil.ReadData<T>(levelDataName);

        if (gameInfo == null)
        {
            Debug.Log("首次登录，新建存档！");
            gameInfo = GetGameInfoAtFristEnter();
            if(gameInfo == null)isNeedSaveGame = false;
            if(isNeedSaveGame)
                JsonUtil.Saver(levelDataName, gameInfo);
            isNewPlayer = true;
        }
    }
    public override void SaveGame()
    {
        if (isNeedSaveGame&&gameInfo!=null)
        {

            SaveGameInfoEvent?.Invoke(gameInfo);
            JsonUtil.Saver(levelDataName, gameInfo);
        }
    }

    #endregion
    #region Unity回调
    protected override void Awake()
    {
        base.Awake();
        levelDataName = $"{Application.persistentDataPath}/{Application.productName}_levelData";
        Debug.Log(levelDataName + "数据保存地址");
    }
    protected virtual void Start()
    {
        
    }


    protected virtual void OnApplicationPause(bool pause)
    {
        if (pause&&isNeedSaveGame&&gameInfo!=null)
        {
            SaveGameInfoEvent?.Invoke(gameInfo);
            JsonUtil.Saver(levelDataName, gameInfo);
        }

    }

    protected virtual void OnApplicationQuit()
    {
        if (!isNeedSaveGame || gameInfo == null) return;
        SaveGameInfoEvent?.Invoke(gameInfo);
        SaveGameInfoEvent = null;
        JsonUtil.Saver(levelDataName, gameInfo);
    }

    #endregion

}
