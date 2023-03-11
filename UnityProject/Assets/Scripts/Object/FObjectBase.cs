using System;
using System.Collections.Generic;
using UnityEngine;

public class FObjectBase : MonoBehaviour
{
    private Dictionary<Type, FControllerBase> controllers = new Dictionary<Type, FControllerBase>();

    private void Update()
    {
        foreach (var pair in controllers)
        {
            pair.Value.Tick(Time.deltaTime);
        }
    }

    public void AddController<T>()
    {
        Type type = typeof(T);
        FControllerBase controller = (FControllerBase)Activator.CreateInstance(type, args: this);
        controllers.Add(type, controller);
        controller.Initialize();
    }

    public T FindController<T>()
    {
        Type type = typeof(T);
        if (controllers.ContainsKey(type))
            return (T)Convert.ChangeType(controllers[type], type);

        return default(T);
    }
}
