using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
#region
//保持UTF-8
#endregion
public class SDKConfigEditor : ScriptableObject
{
    [SerializeField]
    public string SDKCONFIG_PATH= "Assets\\Resources";
    [SerializeField]
    public string SDKCONFIG_NAME= "SDK_CONFIG.json";
}
public static class SDKEditorUtility
{
    public static SDKConfigEditor SafeGetSDKPathConfig()
    {
        SDKConfigEditor asset = null;
#if UNITY_EDITOR
        if (AdManager.Instance == null)
        {
            try
            {
                asset = AssetDatabase.LoadAssetAtPath<SDKConfigEditor>(Path.Combine(SDKUtility.Default_SDKCONFIG_PATH, SDKUtility.Default_SDKCONFIG_PATH_FileName));
            }
            catch
            {

            }
            if (asset == null)
            {
                asset = ScriptableObject.CreateInstance<SDKConfigEditor>();
                AssetDatabase.CreateAsset(asset, Path.Combine(SDKUtility.Default_SDKCONFIG_PATH, SDKUtility.Default_SDKCONFIG_PATH_FileName));//在传入的路径中创建资源
                AssetDatabase.SaveAssets(); //存储资源
                AssetDatabase.Refresh(); //刷新
            }
            return asset;
        }
#endif

        string path = Path.Combine(SDKUtility.Default_SDKCONFIG_PATH, SDKUtility.Default_SDKCONFIG_PATH_FileName);
        path = path.Remove(0, path.LastIndexOf("Resources") + ("Resources").Length + 1);
        path= path.Replace(SDKUtility.Default_SDKCONFIG_PATH_FileName, Path.GetFileNameWithoutExtension(SDKUtility.Default_SDKCONFIG_PATH_FileName));
        Debug.Log($"加载SDKEditor路径->{path}");
        asset = Resources.Load<SDKConfigEditor>(path);
        return asset;

        //do something
    }

    public static void SafeSave(this SDKConfigEditor sDKConfigEditor)
    {
#if UNITY_EDITOR
        EditorUtility.SetDirty(sDKConfigEditor);
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
#else

#endif
    }


}
[AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
public class SDKCallbackAttribute : PropertyAttribute
{
    public string[] paras;

    public SDKCallbackAttribute(params string[] paras)
    {
        this.paras = paras;
    }
}