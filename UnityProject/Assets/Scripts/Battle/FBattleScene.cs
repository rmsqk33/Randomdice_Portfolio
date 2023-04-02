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
        else
        {
            FGlobal.localPlayer.AddController<FBattleController>();
        }
#else
        FGlobal.localPlayer.AddController<FBattleController>();
#endif
    }

    private void OnDestroy()
    {
        FGlobal.localPlayer.RemoveController<FBattleController>();
    }
}
