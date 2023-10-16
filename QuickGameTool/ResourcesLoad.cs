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
        return Resources.Load<T>(resource);
    }
}
