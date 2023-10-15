using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using System.IO;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

public class MyFirstWindow : EditorWindow,IPreprocessBuildWithReport
{
    //用于储存记录Bug人的名字
    string bugReporterName = "";
    //用于描述Bug信息
    string description = "";
    //用于储存 Bug 对象
    GameObject buggyGameObject;

    //利用构造函数来设置窗口的名字
    MyFirstWindow()
    {
        this.titleContent = new GUIContent("Bug Reporter");
    }

    public int callbackOrder => 0;

    [MenuItem("Tool/Bug Reporter")]
    static void showWindow()
    {
        EditorWindow.GetWindow(typeof(MyFirstWindow));
    }

    public void OnPreprocessBuild(BuildReport report)
    {
        EditorApplication.ExecuteMenuItem("Tool/Bug Reporter");
    }

    //绘制窗口界面的函数
    private void OnGUI()
    {
        GUILayout.BeginVertical();

        //绘制标题
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("Bug Reporter");

        //绘制文本
        GUILayout.Space(10);
        bugReporterName = EditorGUILayout.TextField("Bug Name", bugReporterName);

        //绘制当前正在编辑的场景
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 12;
        GUI.skin.label.alignment = TextAnchor.UpperLeft;
        GUILayout.Label("Currently Scene:" + EditorSceneManager.GetActiveScene().name);

        //绘制当前时间
        GUILayout.Space(10);
        GUILayout.Label("Time:" + System.DateTime.Now);

        //绘制对象
        GUILayout.Space(10);
        buggyGameObject = (GameObject)EditorGUILayout.ObjectField(
            "Buggy Game Object", buggyGameObject, typeof(GameObject), true);


        //绘制描述文本区域
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("Describtion", GUILayout.MaxWidth(80));
        description = EditorGUILayout.TextArea(description, GUILayout.MaxHeight(75));
        GUILayout.EndHorizontal();

        EditorGUILayout.Space();

        //添加名为"Save Bug"按钮
        if (GUILayout.Button("Save Bug"))
        {
            SaveBug();
        }

        //添加名为"Save Bug With Screenshot"按钮
        if (GUILayout.Button("Save Bug With Screenshot"))
        {
            SaveBugWithScreenshot();
        }

        GUILayout.EndVertical();

    }

    void SaveBug()
    {
        Directory.CreateDirectory("Assets/BugReports/" + bugReporterName);
        var now = System.DateTime.Now.ToString("yyyy年MM月dd HH时mm分ss秒");
        var path = "Assets\\BugReports\\" + bugReporterName + "\\" + now + ".txt";
        StreamWriter sw = new StreamWriter(path);
        sw.WriteLine(bugReporterName);
        sw.WriteLine(System.DateTime.Now.ToString());
        sw.WriteLine(EditorSceneManager.GetActiveScene().name);
        sw.WriteLine(description);
        //刷新缓存
        sw.Flush();
        //关闭流
        sw.Close();
    }

    void SaveBugWithScreenshot()
    {
        SaveBug();
        var now = System.DateTime.Now.ToString("yyyy年MM月dd HH时mm分ss秒");
        var path = "Assets/BugReports/" + bugReporterName + "/" + now + ".png";
        ScreenCapture.CaptureScreenshot(path);
    }
}