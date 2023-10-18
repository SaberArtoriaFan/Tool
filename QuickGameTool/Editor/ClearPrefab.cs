
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.TextCore.Text;
using Debug = UnityEngine.Debug;
#region
//保持UTF-8
#endregion

public class ClearPrefab
{

    [MenuItem("Test/BuildClipNames")]
    public static void BuildAnimationClip()
    {
        string audioResourcePath = "\\Resources\\AudioClip";

        DirectoryInfo root = new DirectoryInfo(Application.dataPath + audioResourcePath);
        FileInfo[] filesInfo = root.GetFiles();
        List<string> fileName = new List<string>();
        foreach (var v in filesInfo)
        {
            if (v.Name.EndsWith(".wav") || v.Name.EndsWith(".mp3"))
            {
                string s = v.Name.Remove(v.Name.Length - 4);
                //Debug.Log(s);
                fileName.Add(s);
                //Debug.Log(clip.name);
            }

        }
        JsonUtil.Saver($"{Application.dataPath}{audioResourcePath}\\AudioNames", new AudioClipName(fileName.ToArray()));
    }
    //[MenuItem("Test/DeleteAllPlayPref")]
    //public static void DeleteAll()
    //{
    //    PlayerPrefs.DeleteAll();
    //    PlayerPrefs.Save();
    //}
    [MenuItem("Test/DeletePlayInfo")]
    public static void ClearCache()
    {

        string v = $"{Application.persistentDataPath}/{Application.productName}_levelData.json";

        IOUtil.DeleteFileOrDire(v);
    }
    [MenuItem("Test/CleanMissingScript")]
    public static void Begin()
    {
        string[] filePaths = Directory.GetFiles(Application.dataPath, "*.prefab", SearchOption.AllDirectories);
        int sum = 0;
        for (int i = 0; i < filePaths.Length; i++)
        {
            string path = filePaths[i].Replace(Application.dataPath, "Assets");
            GameObject objPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            //GameObject obj = PrefabUtility.InstantiatePrefab(objPrefab) as GameObject;
            //判断是否存在于Hierarchy面板上
            if (objPrefab.hideFlags == HideFlags.None)
            {
                var components = objPrefab.GetComponentsInChildren<Component>(true);
                for (int j = 0; j < components.Length; j++)
                {
                    if (components[j] == null)
                    {
                        DeleteRecursive(objPrefab, (go) =>
                        {

                            GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
                        });
                        Debug.Log("清除了物体：" + objPrefab.name + " 的missing脚本");
                        sum++;
                        break;
                    }
                }
            }

        }
        AssetDatabase.Refresh();

        Debug.Log("清除完成,清理个数：" + sum);
    }
    /// <summary>
    /// </summary>
    /// <param name="obj"></param>
    /// <param name="doDelete"></param>
    static void DeleteRecursive(GameObject obj, System.Action<GameObject> doDelete)
    {
        doDelete(obj);

        for (int i = 0; i < obj.transform.childCount; i++)
        {
            DeleteRecursive(obj.transform.GetChild(i).gameObject, doDelete);
        }
    }

    //[MenuItem("Tools/删除Missing脚本（不可还原）")]
    //public static void StartDelete()
    //{
    //    var obj = Selection.activeGameObject;
    //    if (obj == null) return;
    //    // GameObjectUtility.RemoveMonoBehavioursWithMissingScript(obj);
    //    // //DeleteMissingComp(obj);
    //    DeleteRecursive(obj, (go) =>
    //    {

    //        GameObjectUtility.RemoveMonoBehavioursWithMissingScript(go);
    //    });

    //}

    [MenuItem("Test/SetUiName")]
    public static void T()
    {
        var obj = Selection.activeGameObject;
        if (obj == null) return;
        DeleteRecursive(obj, (v) =>
        {
            if (v.name.StartsWith("Btn") && v.name.EndsWith("_N") == false)
            {
                v.name = v.name + "_N";
                UnityEditor.EditorUtility.SetDirty(v);
            }
        });
        AssetDatabase.Refresh();

    }
}


