using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    public static T Instance => instance;
    protected static T instance;
    protected virtual void Awake()
    {
        if (instance != null)
        {
            GameObject.Destroy(this);
            return;
        }

        instance = (T)this;
    }
}
