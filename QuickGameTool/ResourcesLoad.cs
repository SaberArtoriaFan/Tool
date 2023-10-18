using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region
//保持UTF-8
#endregion
public sealed class ResourcesLoad : AssestLoad
{
    protected override T m_Load<T>(string resource)
    {
        var v = Resources.Load<T>(resource);
        if (v == null) Debug.LogError($"加载资源出错，路径->{resource}");

        return v;
    }
}
