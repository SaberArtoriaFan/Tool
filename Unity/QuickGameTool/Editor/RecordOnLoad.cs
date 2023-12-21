using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.PackageManager.UI;
using UnityEngine;
#region
//保持UTF-8
#endregion
namespace Ad
{
    public class RecordWindow : EditorWindow
    {
        public bool isOnlyRead = false;
        string content;
        string time;
        bool isChange = false;
        RecordWindow()
        {
            //Find
            this.titleContent = new GUIContent("开发者日记");
            position = new Rect(this.position.xMin,position.yMin, 800, 500);
            GetInfo();
            Check();
        }

        private void Check()
        {

        }

        void GetInfo()
        {
            var c = JsonUtil.ReadData<Record>(RecordOnLoad.RecordPath);
            if(c != null)
            {
                content=c.text;
                time=c.time;
            }
            else
            {
                content = string.Empty;
                time = string.Empty;
            }
        }
        private void OnGUI()
        {
            GUILayout.BeginVertical();

            //绘制标题
            GUILayout.Space(10);
            GUI.skin.label.fontSize = 24;
            GUI.skin.label.alignment = TextAnchor.MiddleCenter;
            GUILayout.Label("开发者日记");

            if (isOnlyRead)
            {
                GUILayout.Space(20);
                GUI.skin.label.fontSize = 15;
                GUI.skin.label.alignment = TextAnchor.MiddleCenter;

                GUILayout.Label("更改这个[开发者日志]的按钮在[Tools/Record/Write]喔,当你开发完之后可以写下你埋的坑，hhh");
                GUILayout.Space(10);

                GUI.skin.label.fontSize = 17;
                GUILayout.Label($"以下是内容---->记录时间[{time}]");
                GUILayout.Space(5);


                GUI.skin.label.fontSize = 12;
                GUI.skin.label.alignment = TextAnchor.MiddleLeft;


                GUILayout.Label(content);
            }
            else
            {
                var windowHeigh = this.position.height;
                var str= GUILayout.TextArea(content,GUILayout.Height(windowHeigh/3*2));
                if (str != content)
                    isChange = true;
                content = str;
            }


            if (isOnlyRead == false&&isChange)
            {
                GUI.skin.button.alignment = TextAnchor.MiddleCenter;
                if (GUILayout.Button("保存修改", GUILayout.Width(100)))
                {
                    JsonUtil.Save(RecordOnLoad.RecordPath, new Record() { text = content,time=DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss")},false);
                    isChange = false;
                    EditorPrefs.SetBool(RecordOnLoad.RecordChangeKey,true);
                }
            }


            GUILayout.EndVertical();
        }
    }

    [Serializable]
    public class Record
    {
        public string text;
        public string time;
    }
    public class RecordOnLoad
    {
        public static readonly string RecordPath = Path.GetFullPath(Path.Combine(Application.dataPath, "SDK", "Record.json"));
        public static readonly string RecordKey = "RecordOpen";
        public static readonly string RecordChangeKey = "RecordChange";

        [InitializeOnLoadMethod]
        public static void Record()
        {
            if (EditorPrefs.GetBool(RecordKey,true) ==false)
                return;


            if (EditorPrefs.GetBool(RecordChangeKey,true) == false)
                return;



            if (!EditorPrefs.HasKey(RecordPath))
            {
                var str = JsonUtil.ReadData<Record>(RecordPath)?.text;
                Debug.Log("查询是否有开发者记录！！"+str);

                if (string.IsNullOrEmpty(str) == false)
                {
                    Debug.Log("aaa");
                    EditorPrefs.SetBool(RecordOnLoad.RecordChangeKey, false);

                    var window = EditorWindow.GetWindow<RecordWindow>();
                    //window.ShowPopup();
                    window.isOnlyRead = true;
                }
                EditorPrefs.SetBool(RecordPath, true);

            }


        }
        [InitializeOnLoadMethod]
        private static void RegisterQuitEvent()
        {
            EditorApplication.quitting -= OnEditorQuitting;
            EditorApplication.quitting += OnEditorQuitting;
        }

        private static void OnEditorQuitting()
        {
            EditorPrefs.DeleteKey(RecordPath);
        }
        [MenuItem("Tools/Record/Write")]
        public static void OpenRecordWindow()
        {
            var window = EditorWindow.GetWindow<RecordWindow>();
            //window.ShowPopup();
        }
        [MenuItem("Tools//Record/Test")]
        public static void Test()
        {
            OnEditorQuitting();

            AssetDatabase.Refresh();
        }
        [MenuItem("Tools/Record/OpenOrClose")]
        public static void OpenOrClose()
        {
            string text = EditorPrefs.GetBool(RecordKey, true) ? "关闭" : "开启";
            if (EditorUtility.DisplayDialog("设置[开发者日记]", $"是否-->{text} [开发者日记功能]", "确定", "算了"))
            {
                EditorPrefs.SetBool(RecordKey, !EditorPrefs.GetBool(RecordKey, true));
            }
        }
    }
}


