using Packet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FBattleScene : MonoBehaviour
{
    private void Start()
    {
#if DEBUG
        if (FServerManager.Instance.InitUser == false)
            return;
#endif
        InitBattle();

        P2P_READY_BATTLE pkt = new P2P_READY_BATTLE();
        FServerManager.Instance.SendMessage(pkt);

        SceneManager.activeSceneChanged += LeaveScene;
    }

    public static void InitBattle()
    {
        FGlobal.localPlayer.AddController<FBattleDiceController>();
        FGlobal.localPlayer.AddController<FBattleWaveController>();
        FGlobal.localPlayer.AddController<FSkillAreaController>();
        FGlobal.localPlayer.FindController<FBattleWaveController>().StartBattle(FBattleDataManager.Instance.CoopBattleID);
        FPathManager.Instance.OnStartBattle();
    }

    private void LeaveScene(Scene InScene1, Scene InScene2)
    {
        FGlobal.localPlayer.RemoveController<FBattleDiceController>();
        FGlobal.localPlayer.RemoveController<FBattleWaveController>();
        FGlobal.localPlayer.RemoveController<FSkillAreaController>();

        FObjectManager.Instance.Clear();
    }
}
