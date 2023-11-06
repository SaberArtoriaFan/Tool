using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
#region
//作者:Saber
#endregion
public class SDKConfigWindow : EditorWindow
{
    string SDK_NAME = "";

    string CURR_SDKNAME = "";
    SDKConfigWindow()
    {
        this.titleContent = new GUIContent("SDK配置窗口");
        SDK_NAME = Read();
        CURR_SDKNAME = SDK_NAME;
    }
    [MenuItem(SDKUtility.SDKCONFIG_WINDOW_PATH)]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(SDKConfigWindow));
    }

    //绘制窗口界面的函数
    private void OnGUI()
    {
        GUILayout.BeginVertical();

        //绘制标题
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("SDK配置");
        //设置字体
        GUI.skin.label.fontSize = 12;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        ////绘制文本
        //GUILayout.Space(10);
        //bugReporterName = EditorGUILayout.TextField("Bug Name", bugReporterName);

        ////绘制当前正在编辑的场景
        //GUILayout.Space(10);

        //GUILayout.Label("Currently Scene:" + EditorSceneManager.GetActiveScene().name);

        ////绘制当前时间
        //GUILayout.Space(10);
        //GUILayout.Label("Time:" + System.DateTime.Now);

        ////绘制对象
        //GUILayout.Space(10);
        //buggyGameObject = (GameObject)EditorGUILayout.ObjectField(
        //    "Buggy Game Object", buggyGameObject, typeof(GameObject), true);

        //绘制SDK配置文件保存路径
        GUILayout.Space(10);
        GUILayout.Label($"SDK文件保存路径---->{SDKUtility.SDKCONFIG_PATH}\\{SDKUtility.SDKCONFIG_NAME}");
        GUILayout.Space(10);
        GUILayout.Label($"SDK包当前名称---->[{CURR_SDKNAME}]");

        //绘制描述文本区域
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("安卓SDK名称(输入时请注意空格)", GUILayout.MaxWidth(225));
        GUILayout.Space(10);

        SDK_NAME = EditorGUILayout.TextArea(SDK_NAME, GUILayout.MaxHeight(20));
        //SDK_NAME = SDK_NAME.Replace("\r\n", "");
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //添加名为"Save Bug"按钮
        if (GUILayout.Button("保存修改"))
        {
            Save();
        }

        ////添加名为"Save Bug With Screenshot"按钮
        //if (GUILayout.Button("Save Bug With Screenshot"))
        //{
        //    SaveBugWithScreenshot();
        //}

        GUILayout.EndVertical();

    }

    private void Save()
    {
        
        if (string.IsNullOrEmpty(SDK_NAME))
        {
            Debug.LogError("不能保存为空！");
            return;
        }

        SDK_NAME = SDK_NAME.Replace(" ", "");
        SDK_NAME = SDK_NAME.Replace("\r\n", "");
        SDK_NAME = SDK_NAME.Replace("\n", "");

        if (!Directory.Exists(SDKUtility.SDKCONFIG_PATH))
            Directory.CreateDirectory(SDKUtility.SDKCONFIG_PATH);
        //var now = System.DateTime.Now.ToString("yyyy年MM月dd HH时mm分ss秒");
        var path = $"{SDKUtility.SDKCONFIG_PATH}\\{SDKUtility.SDKCONFIG_NAME}";
        JsonUtil.Saver(path, new SDKUtility.SDKConfig(SDK_NAME));
        Debug.Log("保存成功！" + SDK_NAME);
        CURR_SDKNAME = Read();
    }

    string Read() => SDKUtility.ReadSDKPackageName();
}
    
    
