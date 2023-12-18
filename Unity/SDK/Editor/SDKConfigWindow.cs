using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;
#region
//作者:Saber
#endregion
public class SDKConfigWindow : EditorWindow
{
    SDKConfigEditor SDKConfigEditor;

    SDKUtility.SDKConfig config;
    string SDK_NAME => config?.SDKNAME;

    string CURR_SDKNAME = "";
    List<SDKUtility.SDKFunc> funcList = null;
    List<byte> extraDict = new List<byte>();
    Dictionary<byte,bool> parasOpen= new Dictionary<byte,bool>();
    Dictionary<byte,List<SDKUtility.CallbackPara>> parasList = new Dictionary<byte, List<SDKUtility.CallbackPara>>();

    bool[] funcChangeControll = null;
    bool isError = false;
    bool onlyShowOwner = false;
    SDKConfigWindow()
    {
        //Find

        this.titleContent = new GUIContent("SDK配置窗口");
        funcList = new List<SDKUtility.SDKFunc>();
        foreach (var v in Enum.GetValues(typeof(SDKUtility.SDKFunc)))
        {
            funcList.Add((SDKUtility.SDKFunc)v);
            parasOpen.Add((byte)v, false);
        }
        funcChangeControll =new bool[funcList.Count];
        isError = false;

    }
    void Init()
    {
        config = ReadConfig();
        if (config == null)
        {
            //寻找
            FindConfig();
        }

        if (config == null)
            InitConfig();
        else
            VieryConfig();
        //InitExtraDict();
        // SDK_NAME = Read();
        CURR_SDKNAME = SDK_NAME;
    }

    private void FindConfig()
    {
        Debug.Log($"开始寻找任意Resources文件夹下的{SDKUtility.SDKCONFIG_NAME}文件");
        var dir = new DirectoryInfo(Application.dataPath);
        foreach(var son in dir.GetDirectories())
        {
            var res = FindConfigInResources(son,false);
            if (res != null)
            {
                var path = res.DirectoryName;
                path = path.Remove(0, Application.dataPath.Length - ("Assets").Length);
                Debug.Log($"找到了配置文件,文件夹->{path}");
                SDKUtility.SDKCONFIG_PATH = path;
                config= ReadConfig();
            }
        }
    }
    FileInfo FindConfigInResources(DirectoryInfo directoryInfo,bool isParentVaild)
    {
        if (isParentVaild==false&&directoryInfo.Name=="Resources")
            isParentVaild = true;
        if (isParentVaild)
        {
            foreach (var son in directoryInfo.GetFiles(SDKUtility.SDKCONFIG_NAME))
                return son;
        }


        foreach(var son in directoryInfo.GetDirectories())
        {
            var res=FindConfigInResources(son,isParentVaild);
            if(res != null) return res;
        }
        return null;
    }
    private void InitExtraDict()
    {
        //var needList = new List<byte>();
        extraDict.Clear();
        var tempList = new Dictionary<byte, List<SDKUtility.CallbackPara>>(parasList);
        parasList.Clear();
        HashSet<SDKUtility.SDKFunc> map=new HashSet<SDKUtility.SDKFunc>(config.SdkCallArray.Select(u=>(SDKUtility.SDKFunc)u.Id));
        foreach (var v in funcList)
        {
            if (map.Contains(v) == false)
            {
                extraDict.Add((byte)v);
                if (tempList.TryGetValue((byte)v, out var para))
                    parasList.Add((byte)v, para);
                else
                    parasList.Add((byte)v, new List<SDKUtility.CallbackPara>());

            }
        }
                
    }

    private void VieryConfig()
    {
        HashSet<byte> map = new HashSet<byte>(); 
        foreach (var call in config.SdkCallArray)
        {
            var id = call.Id;
            if (Enum.IsDefined(typeof(SDKUtility.SDKFunc), id) == false)
            {
                Debug.LogError($"配置表SDK方法名{id}不合法，一般是修改了[SDKUtility.SDKFunc]枚举导致的！！！,请删除{SDKUtility.SDKCONFIG_FullPath}后重新打开本界面自动生成");
                goto J;
            }
            else if(map.Contains(id))
            {
                Debug.LogError($"配置表SDK方法名{id}出现重复！！！，一般是手动错误修改了Json文件导致的！！！,请删除{SDKUtility.SDKCONFIG_FullPath}后重新打开本界面自动生成");
                goto J;
            }
            map.Add(id);
            if (string.IsNullOrEmpty(call.FuncName))
                call.FuncName = ((SDKUtility.SDKFunc)id).ToString();
        }

        goto H;
    J:
        {
            isError = true;
            //this.Close();
        }
    H:
        return;

    }

    private void InitConfig()
    {
        var callArray = new SDKUtility.AndroidJavaCall[funcList.Count];
        for (int i = 0; i < funcList.Count; i++)
            callArray[i] = new SDKUtility.AndroidJavaCall((byte)funcList[i], funcList[i].ToString());
        config = new SDKUtility.SDKConfig("",callArray);

    }

    [MenuItem(SDKUtility.SDKCONFIG_WINDOW_PATH)]
    static void showWindow()
    {
        var window = EditorWindow.GetWindow<SDKConfigWindow>();
        window.SDKConfigEditor=SDKEditorUtility.SafeGetSDKPathConfig();
        window.Init();
    }
    //override 
    //绘制窗口界面的函数
    private void OnGUI()
    {
        GUILayout.BeginVertical();

        //绘制标题
        GUILayout.Space(10);
        GUI.skin.label.fontSize = 24;
        GUI.skin.label.alignment = TextAnchor.MiddleCenter;
        GUILayout.Label("SDK配置 1.1(json读取版)");
        if (isError)
        {
            GUILayout.Space(50);
            GUILayout.Label("SDK配置出错，详情请查看Console的Log日志");
            GUILayout.EndVertical();
            return;
        }

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
        GUILayout.Label($"SDK文件保存路径---->{SDKConfigEditor.SDKCONFIG_PATH}\\{SDKConfigEditor.SDKCONFIG_NAME}");
        GUILayout.Space(10);
        GUILayout.Label($"SDK包当前名称---->[{CURR_SDKNAME}]");

        //绘制描述文本区域
        GUILayout.Space(10);
        GUILayout.BeginHorizontal();
        GUILayout.Label("安卓SDK名称(输入时请注意空格)", GUILayout.MaxWidth(225));
        GUILayout.Space(10);
        config.SDKNAME = EditorGUILayout.TextArea(SDK_NAME, GUILayout.MaxHeight(20));
        GUILayout.EndHorizontal();

        //config的其他函数参数配置
        GUILayout.BeginHorizontal();
        GUILayout.Label("SDK函数名称配置~~~",GUILayout.MaxWidth(225));
        GUILayout.Label("最多接受参数(-1为不限制)");

        if (GUILayout.Button(onlyShowOwner ? "显示全部可配置参数" : "只显示已配置参数", GUILayout.MaxWidth(225)))onlyShowOwner = !onlyShowOwner;
        GUILayout.EndHorizontal();

        GUILayout.Space(10);

        for (int i = 0; i < config.SdkCallArray.Length; i++)
        {
            var id = config.SdkCallArray[i].Id;

            GUILayout.BeginHorizontal();
            GUILayout.Space(10);

            if (GUILayout.Button("-", GUILayout.Width(30)))
            {
                //删除这行记录
                var newArray=new List<SDKUtility.AndroidJavaCall>(config.SdkCallArray);
                newArray.RemoveAt(i);
                config.SdkCallArray = newArray.ToArray();
                //config.RemoveRepeatPara(id);
                GUILayout.EndHorizontal();
                goto F;

            }
            //if (GUILayout.Button(parasOpen[id] ? "⬆⬆" : "VV", GUILayout.Width(40))) parasOpen[id] = !parasOpen[id];

            GUILayout.Label($"{((SDKUtility.SDKFunc)config.SdkCallArray[i].Id)}:", GUILayout.MaxWidth(225));
            GUILayout.Space(5);
            EditorGUI.BeginDisabledGroup(!funcChangeControll[i]);
            var maxPara = EditorGUILayout.IntField(config.SdkCallArray[i].maxParametersNum == int.MaxValue ? -1 : config.SdkCallArray[i].maxParametersNum,GUILayout.MaxWidth(30), GUILayout.MaxHeight(20));
            if(maxPara!=-1)
                config.SdkCallArray[i].maxParametersNum = maxPara;
            GUILayout.Space(40);

            config.SdkCallArray[i].FuncName = EditorGUILayout.TextArea(config.SdkCallArray[i].FuncName, GUILayout.MaxHeight(20));
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button(funcChangeControll[i] ? "X" : "O",GUILayout.Width(40))) funcChangeControll[i] = !funcChangeControll[i];
            GUILayout.EndHorizontal();
            if (parasOpen[config.SdkCallArray[i].Id])
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(20);
                //if (GUILayout.Button("添加参数", GUILayout.MaxWidth(100)))
                //{
                //    var newArray = new List<SDKUtility.CallbackPara>(config.SdkCallbackPara);
                //    newArray.Add(new SDKUtility.CallbackPara() { funcID = id });
                //    config.SdkCallbackPara = newArray.ToArray();
                //    GUILayout.EndHorizontal();
                //    goto F;
                //}
                //if (GUILayout.Button("添加参数", GUILayout.MaxWidth(50))) parasList[v].Add(new SDKUtility.CallbackPara());
                GUILayout.EndHorizontal();

                //这里改成获取方法名是自己的参数
                //var paras = config.GetFuncCallBackParas(id);
                //for (int j = 0; j <paras .Length; j++)
                //{
                //    var p = paras[j];
                //    GUILayout.BeginHorizontal();
                //    GUILayout.Space(30);
                //    //var ds = SDKUtility.SDKCallbackPara.fail;
                //    p.id = (byte)((SDKUtility.SDKCallbackPara)EditorGUILayout.EnumPopup((SDKUtility.SDKCallbackPara)p.id, GUILayout.MaxHeight(20), GUILayout.MaxWidth(225)));
                //    p.callName = EditorGUILayout.TextArea(p.callName, GUILayout.MaxHeight(20));
                //    if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                //    {
                //        var newArray = new List<SDKUtility.CallbackPara>(config.SdkCallbackPara);
                //        newArray.Remove(p);
                //        config.SdkCallbackPara = newArray.ToArray();
                //        GUILayout.EndHorizontal();
                //        goto F;
                //    }

                //    GUILayout.EndHorizontal();
                //}
            }
        }

        //回调的地方

        if (onlyShowOwner == false)
        {
            InitExtraDict();   
            GUILayout.BeginVertical();
            GUILayout.Space(5);
            GUILayout.Label("可添加---");
            GUILayout.EndVertical();
            GUILayout.Space(5);
            foreach (var v in extraDict)
            {
                GUILayout.BeginHorizontal();
                GUILayout.Space(10);
                if (GUILayout.Button(parasOpen[v] ? "⬆⬆" : "VV", GUILayout.Width(40))) parasOpen[v] = !parasOpen[v];
                GUILayout.Label($"{((SDKUtility.SDKFunc)v)}:", GUILayout.MaxWidth(225));
                GUILayout.Space(5);
                //EditorGUI.BeginDisabledGroup(!funcChangeControll[i]);
                string s = EditorGUILayout.TextArea(string.Empty, GUILayout.MaxHeight(20));
                //EditorGUI.EndDisabledGroup();
                if (GUILayout.Button("+", GUILayout.Width(30)))
                {
                    var newNameKeys = new SDKUtility.AndroidJavaCall[config.SdkCallArray.Length + 1];
                    Array.Copy(config.SdkCallArray, newNameKeys, config.SdkCallArray.Length);
                    newNameKeys[newNameKeys.Length - 1] = new SDKUtility.AndroidJavaCall(v,s);
                    config.SdkCallArray = newNameKeys;
                    GUILayout.EndHorizontal();
                    goto F;
                }
                GUILayout.EndHorizontal();
                //if (parasOpen[v])
                //{
                //    GUILayout.BeginHorizontal();
                //    GUILayout.Space(20);
                //    if (GUILayout.Button("添加参数", GUILayout.MaxWidth(100))) parasList[v].Add(new SDKUtility.CallbackPara() { funcID=v});
                //    //if (GUILayout.Button("添加参数", GUILayout.MaxWidth(50))) parasList[v].Add(new SDKUtility.CallbackPara());
                //    GUILayout.EndHorizontal();

                //    for(int i = 0; i < parasList[v].Count;i++)
                //    {
                //        var p = parasList[v][i];
                //        GUILayout.BeginHorizontal();
                //        GUILayout.Space(30);
                //        //var ds = SDKUtility.SDKCallbackPara.fail;
                //        p.id= (byte)((SDKUtility.SDKCallbackPara)EditorGUILayout.EnumPopup((SDKUtility.SDKCallbackPara)p.id, GUILayout.MaxHeight(20),GUILayout.MaxWidth(225)));
                //        p.callName = EditorGUILayout.TextArea(p.callName, GUILayout.MaxHeight(20));
                //        if (GUILayout.Button("-", GUILayout.MaxWidth(20)))
                //        {
                //            parasList[v].Remove(p);
                //            GUILayout.EndHorizontal();
                //            goto F;
                //        }

                //        GUILayout.EndHorizontal();
                //    }
                //}
            }
        }




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
     F:
        GUILayout.EndVertical();

    }

    private void Save()
    {
        
        if (string.IsNullOrEmpty(SDK_NAME))
        {
            Debug.LogError("SDK包名不能为空！");
            return;
        }

        config.SDKNAME = SDK_NAME.Replace(" ", "");
        config.SDKNAME = SDK_NAME.Replace("\r\n", "");
        config.SDKNAME = SDK_NAME.Replace("\n", "");

        if (!Directory.Exists(SDKConfigEditor.SDKCONFIG_PATH))
            Directory.CreateDirectory(SDKConfigEditor.SDKCONFIG_PATH);
        //var now = System.DateTime.Now.ToString("yyyy年MM月dd HH时mm分ss秒");
        var path = Path.Combine(SDKConfigEditor.SDKCONFIG_PATH,SDKConfigEditor.SDKCONFIG_NAME);
        var info= JsonUtility.ToJson(config);
        using(var streamWriter=new StreamWriter(path))
        {
            streamWriter.Write(info);
            streamWriter.Flush();
            streamWriter.Close();
            streamWriter.Dispose();
        }
        Debug.Log("保存成功！" + SDK_NAME);
        CURR_SDKNAME = Read();
    }

    string Read() => ReadConfig()?.SDKNAME;

   SDKUtility.SDKConfig ReadConfig()=>SDKUtility.ReadConfig(this.SDKConfigEditor);
}
    
    
