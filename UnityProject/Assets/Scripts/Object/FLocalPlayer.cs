
public class FLocalPlayer : FObjectBase
{
    public bool IsHost { get; set; }

    protected override void Awake()
    {
        base.Awake();

        AddController<FInventoryController>();
        AddController<FDiceController>();
        AddController<FBattlefieldController>();
        AddController<FPresetController>();
        AddController<FLocalPlayerStatController>();
        AddController<FStoreController>();
    }
}
