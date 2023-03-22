
public class FLocalPlayer : FObjectBase
{
    public bool Host { get; set; }

    protected override void Awake()
    {
        base.Awake();

        AddController<FInventoryController>();
        AddController<FDiceController>();
        AddController<FBattlefieldController>();
        AddController<FPresetController>();
        AddController<FLocalPlayerStatController>();
        AddController<FStoreController>();
        AddController<FIFFController>();

        FindController<FIFFController>().IFFType = FEnum.IFFType.LocalPlayer;
    }
}
