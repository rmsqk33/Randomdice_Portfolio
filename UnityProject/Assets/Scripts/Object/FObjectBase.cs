using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class FObjectBase : MonoBehaviour
{
    private Dictionary<Type, FControllerBase> controllers = new Dictionary<Type, FControllerBase>();
    private List<FObjectStateObserver> observers = new List<FObjectStateObserver>();
    private SortingGroup sortingGroup;
    
    public int ObjectID { get; set; }
    public int ContentID { get; set; }
    public FObjectBase SummonOwner { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }
    public Vector2 LocalPosition { get { return transform.localPosition; } set { transform.localPosition = value; } }
    public int SortingOrder 
    {
        set 
        { 
            if (sortingGroup != null) 
                sortingGroup.sortingOrder = value; 
        }

        get 
        {
            if (sortingGroup != null)
                return sortingGroup.sortingOrder;

            return 0;
        }
    }

    protected virtual void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    public virtual void Release()
    {
        foreach(var pair in controllers)
        {
            pair.Value.Release();
        }

        for (int i = observers.Count - 1; 0 <= i; --i)
        {
            observers[i].OnDestroyObject(this);
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
        if (controllers.ContainsKey(type))
            return;

        FControllerBase controller = (FControllerBase)Activator.CreateInstance(type, args: this);
        controllers.Add(type, controller);
        controller.Initialize();
    }

    public void RemoveController<T>()
    {
        Type type = typeof(T);
        if (controllers.ContainsKey(type))
        {
            controllers[type].Release();
            controllers.Remove(type);
        }
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
        return SummonOwner == FGlobal.localPlayer;
    }

    public void AddObserver(FObjectStateObserver InObserver)
    {
        observers.Add(InObserver);
    }

    public void RemoveObserver(FObjectStateObserver InObserver)
    {
        observers.Remove(InObserver);
    }

    public virtual void SetEnable(bool InEanble)
    {

    }
}
