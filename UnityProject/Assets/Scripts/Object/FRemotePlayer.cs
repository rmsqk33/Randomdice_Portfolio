
public class FRemotePlayer : FObjectBase
{
    public void Initialize()
    {
        UserIndex = 1;

        AddController<FRemotePlayerBattleController>();
        AddController<FSkillAreaController>();
    }
}
