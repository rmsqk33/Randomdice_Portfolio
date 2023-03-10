using System;
using System.Collections.Generic;
using UnityEngine;

public class FLocalPlayer : FSingleton<FLocalPlayer>
{
    private Dictionary<Type, FControllerBase> controllers = new Dictionary<Type, FControllerBase>();

    protected override void Awake() 
    {
        base.Awake();

        AddController<FInventoryController>();
        AddController<FDiceController>();
        AddController<FBattlefieldController>();
        AddController<FPresetController>();
        AddController<FStatController>();
        AddController<FStoreController>();
    }

    private void Update()
    {
        foreach(var pair in controllers)
        {
            pair.Value.Tick(Time.deltaTime);
        }
    }

    public void AddController<T>()
    {
        Type type = typeof(T);
        FControllerBase controller = (FControllerBase)Activator.CreateInstance(type, args: Instance);
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
