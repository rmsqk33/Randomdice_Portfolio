
public class FRemotePlayer : FObjectBase
{
    public void Initialize()
    {
        AddController<FRemotePlayerBattleController>();
        AddController<FSkillAreaController>();
    }
}
