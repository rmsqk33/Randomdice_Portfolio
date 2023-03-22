using System;
using System.Collections.Generic;
using TMPro;
using Unity.IO.LowLevel.Unsafe;
using UnityEngine;

public class FObjectBase : MonoBehaviour
{
    private Dictionary<Type, FControllerBase> controllers = new Dictionary<Type, FControllerBase>();
    private List<FObjectStateObserver> observers = new List<FObjectStateObserver>();

    public int ObjectID { get; set; }
    public int ContentID { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }
    public Vector2 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }

    protected virtual void Awake()
    {

    }

    public virtual void Release()
    {
        for (int i = observers.Count - 1; 0 <= i; --i)
        {
            observers[i].OnDestroyObject();
        }
    }

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

    public T FindChildComponent<T>(string InName)
    {
        Transform child = transform.Find(InName);
        if (child != null)
            return child.GetComponent<T>();

        return default(T);
    }

    public bool IsOwnLocalPlayer()
    {
        FIFFController iffController = FindController<FIFFController>();
        if (iffController == null)
            return false;

        return iffController.IsOwnLocalPlayer();
    }

    public void AddObserver(FObjectStateObserver InObserver)
    {
        observers.Add(InObserver);
    }

    public void RemoveObserver(FObjectStateObserver InObserver)
    {
        observers.Remove(InObserver);
    }

}
