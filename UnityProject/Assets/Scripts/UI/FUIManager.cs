using System;
using System.Collections.Generic;
using UnityEngine;

public class FUIManager : FSingleton<FUIManager>
{
    Dictionary<Type, FUIBase> uiMap = new Dictionary<Type, FUIBase>();

    public T FindUI<T>() where T : FUIBase
    {
        Type type = typeof(T);
        if (uiMap.ContainsKey(type) == false)
        {
            uiMap.Add(type, GameObject.FindObjectOfType<T>());
        }
        else if (uiMap[type] == null)
        {
            uiMap[type] = GameObject.FindObjectOfType<T>();
        }

        return (T)uiMap[type];
    }
}
