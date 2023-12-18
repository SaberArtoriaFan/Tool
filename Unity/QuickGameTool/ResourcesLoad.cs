using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region
//保持UTF-8
#endregion
public sealed class ResourcesLoad : AssestLoad
{
    protected override void Awake()
    {
        base.Awake();
        DontDestroyOnLoad(this.gameObject);
    }
    protected override T m_Load<T>(string resource)
    {
        if (resource.StartsWith("Assets\\Resources\\"))
            resource = resource.Remove(0, ("Assets\\Resources\\").Length);
        else if (resource.StartsWith("Assets/Resources/"))
            resource = resource.Remove(0, ("Assets/Resources/").Length);
        if (resource.LastIndexOf('.')>0)
        {
           resource= resource.Remove(resource.LastIndexOf('.'));
        }
        var v = Resources.Load<T>(resource);
        if (v == null) Debug.LogError($"加载资源出错，路径->{resource}");

        return v;
    }
}
