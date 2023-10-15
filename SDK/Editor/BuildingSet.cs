using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;

public class BuildSetting
{
    [DidReloadScripts]
    static void OnScriptsEditOver()//代码编译完成时调用
    {
        //注册打包发布的事件；unity在打包发布的时候会判断buildPlayerHandler 是不是为null，为空就执行默认打包方法，不为空就执行注册的事件
        BuildPlayerWindow.RegisterBuildPlayerHandler(OverrideBuildPlayer);
    }

    static void OverrideBuildPlayer(BuildPlayerOptions BPOption)
    {
        if (EditorUtility.DisplayDialog("提示：", "\n确定要以测试服务器的形式打包？", "是", "否"))
        {
            BuildPlayerWindow.DefaultBuildMethods.BuildPlayer(BPOption);//调用unity默认的打包方法。取消打包，不用写其他代码
        }
        else
        {
            UnityEditor.EditorApplication.ExecuteMenuItem("Tool/Bug Reporter");

        }

    }
}


