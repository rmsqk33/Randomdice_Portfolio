using System;
using System.Collections.Generic;

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

    private void AddController<T>()
    {
        Type type = typeof(T);
        controllers.Add(type, (FControllerBase)Activator.CreateInstance(type, args:Instance));
    }

    public T FindController<T>()
    {
        Type type = typeof(T);
        if (controllers.ContainsKey(type))
            return (T)Convert.ChangeType(controllers[type], type);

        return default(T);
    }
}
