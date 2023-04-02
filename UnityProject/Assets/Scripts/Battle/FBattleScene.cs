using UnityEngine;

public class FBattleScene : MonoBehaviour
{
    private void Start()
    {
#if DEBUG
        if (!FServerManager.Instance.IsConnectedServer)
        {
            FServerManager.Instance.ConnectMainServer();
            FAccountMananger.Instance.TryLogin();
        }
#else
        FGlobal.localPlayer.AddController<FBattleDiceController>();
        FGlobal.localPlayer.AddController<FBattleWaveController>();
        FGlobal.localPlayer.FindController<FBattleWaveController>().StartBattle(FBattleDataManager.Instance.CoopBattleID);
#endif
    }

    private void OnDestroy()
    {
        FGlobal.localPlayer.RemoveController<FBattleDiceController>();
    }
}
