using System;
using System.Collections.Generic;
using UnityEngine;

public class FLocalPlayer : FObjectBase
{
    void Awake() 
    {
        AddController<FInventoryController>();
        AddController<FDiceController>();
        AddController<FBattlefieldController>();
        AddController<FPresetController>();
        AddController<FStatController>();
        AddController<FStoreController>();
    }
}
