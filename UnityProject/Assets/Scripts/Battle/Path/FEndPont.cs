
public class FEndPont : FPathBase
{
    public override void OnPass(FObjectBase InObject)
    {
        FObjectManager.Instance.RemoveEnemey(InObject.ObjectID);

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if(battleController != null)
        {
            --battleController.Life;
        }
    }
}
