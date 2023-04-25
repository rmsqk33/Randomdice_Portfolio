using Packet;

public class FEndPont : FPath
{
    public override void OnPass(FObjectBase InObject)
    {
        if (FGlobal.localPlayer.IsHost)
        {
            FObjectManager.Instance.RemoveObjectAndSendP2P(InObject.ObjectID);

            FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
            if (waveController != null)
            {
                --waveController.Life;
            }
        }
    }
}
