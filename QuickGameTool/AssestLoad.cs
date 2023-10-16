using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#region
//保持UTF-8
#endregion
public abstract class AssestLoad : Singleton<AssestLoad> 
{
    protected abstract T m_Load<T>(string resource) where T: Object;

    public static T Load<T>(string resource) where T : Object=>Instance.m_Load<T>(resource);
}
