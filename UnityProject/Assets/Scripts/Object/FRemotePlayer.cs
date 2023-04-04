
public class FRemotePlayer : FObjectBase
{
    protected override void Awake()
    {
        base.Awake();

        AddController<FRemotePlayerBattleController>();
    }
}
