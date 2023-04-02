
public class FEndPont : FPathBase
{
    public override void OnPass(FObjectBase InObject)
    {
        FObjectManager.Instance.RemoveEnemey(InObject.ObjectID);

        FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
        if(waveController != null)
        {
            --waveController.Life;
        }
    }
}
