using FEnum;
using Packet;
using UnityEngine;

public class FP2PPacketHandler
{
    [RuntimeInitializeOnLoadMethod]
    static void RegistPacketHandler()
    {
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_PLAYER_DATA, Handle_P2P_PLAYER_DATA);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_CHANGE_DICE_LEVEL, Handle_P2P_CHANGE_DICE_LEVEL);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_SPAWN_DICE, Handle_P2P_SPAWN_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_DESPAWN_OBJECT, Handle_P2P_DESPAWN_OBJECT);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_CHANGE_LIFE, Handle_P2P_CHANGE_LIFE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_SPAWN_ENEMY, Handle_P2P_SPAWN_ENEMY);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_CHANGE_WAVE, Handle_P2P_CHANGE_WAVE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_DAMAGE, Handle_P2P_DAMAGE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_READY_BATTLE, Handle_P2P_READY_BATTLE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_ON_SKILL, Handle_P2P_ON_SKILL);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_OFF_SKILL, Handle_P2P_OFF_SKILL);
    }

    static void Handle_P2P_READY_BATTLE(in byte[] InBuffer)
    {
        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        FPresetController presetController = FGlobal.localPlayer.FindController<FPresetController>();

        P2P_PLAYER_DATA playerDataPkt = new P2P_PLAYER_DATA();
        playerDataPkt.name = statController.Name;
        playerDataPkt.level = statController.Level;

        int i = 0;
        presetController.ForeachDicePreset(presetController.SelectedPresetIndex, (int InDiceID) =>
        {
            playerDataPkt.diceIdList[i] = InDiceID;
            ++i;
        });

        FServerManager.Instance.SendMessage(playerDataPkt);
    }

    static void Handle_P2P_PLAYER_DATA(in byte[] InBuffer)
    {
        P2P_PLAYER_DATA pkt = new P2P_PLAYER_DATA(InBuffer);

        if (FGlobal.remotePlayer == null)
        {
            GameObject gameObject = new GameObject("remotePlayer");
            FGlobal.remotePlayer = gameObject.AddComponent<FRemotePlayer>();
        }

        FRemotePlayerBattleController battleController = FGlobal.remotePlayer.FindController<FRemotePlayerBattleController>();
        if(battleController != null)
        {
            battleController.Handle_P2P_PLAYER_DATA(pkt);
        }
    }

    static void Handle_P2P_CHANGE_DICE_LEVEL(in byte[] InBuffer)
    {
        P2P_CHANGE_DICE_LEVEL pkt = new P2P_CHANGE_DICE_LEVEL(InBuffer);

        FRemotePlayerBattleController battleController = FGlobal.remotePlayer.FindController<FRemotePlayerBattleController>();
        if (battleController != null)
        {
            battleController.SetDiceLevel(pkt.index, pkt.level);
        }
    }

    static void Handle_P2P_SPAWN_DICE(in byte[] InBuffer)
    {
        P2P_SPAWN_DICE pkt = new P2P_SPAWN_DICE(InBuffer);

        FObjectManager.Instance.CreateRemotePlayerBattleDice(pkt.objectId, pkt.diceId, pkt.eyeCount, pkt.index);
    }

    static void Handle_P2P_DESPAWN_OBJECT(in byte[] InBuffer)
    {
        P2P_DESPAWN_OBJECT pkt = new P2P_DESPAWN_OBJECT(InBuffer);

        FObjectManager.Instance.RemoveObject(pkt.objectId);
    }

    static void Handle_P2P_CHANGE_LIFE(in byte[] InBuffer)
    {
        P2P_CHANGE_LIFE pkt = new P2P_CHANGE_LIFE(InBuffer);

        FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
        if (waveController != null)
        {
            waveController.Life = pkt.life;
        }
    }

    static void Handle_P2P_SPAWN_ENEMY(in byte[] InBuffer)
    {
        P2P_SPAWN_ENEMY pkt = new P2P_SPAWN_ENEMY(InBuffer);

        FObjectManager.Instance.CreateEnemy(pkt.enemyId);
    }

    static void Handle_P2P_CHANGE_WAVE(in byte[] InBuffer)
    {
        P2P_CHANGE_WAVE pkt = new P2P_CHANGE_WAVE(InBuffer);

        FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
        if (waveController != null)
        {
            waveController.SetNextWave(pkt.wave);
        }
    }

    static void Handle_P2P_DAMAGE(in byte[] InBuffer)
    {
        P2P_DAMAGE pkt = new P2P_DAMAGE(InBuffer);

        FObjectBase target = FObjectManager.Instance.FindObject(pkt.objectId);
        if (target != null)
        {
            FCombatTextManager.Instance.AddText(pkt.critical ? CombatTextType.Critical : CombatTextType.Normal, pkt.damage, target);
         
            FEnemyStatController statController = target.FindController<FEnemyStatController>();
            if (statController != null)
            {
                statController.OnDamage(pkt.damage);
            }
        }
    }

    static void Handle_P2P_ON_SKILL(in byte[] InBuffer)
    {
        P2P_ON_SKILL pkt = new P2P_ON_SKILL(InBuffer);
        
        FObjectBase objectBase = FObjectManager.Instance.FindObject(pkt.objectId);
        if (objectBase == null)
            return;
         
        FSkillController skillController = objectBase.FindController<FSkillController>();
        if (skillController == null)
            return;

        skillController.OnSkill(pkt.skillId, pkt.targetId);
    }

    static void Handle_P2P_OFF_SKILL(in byte[] InBuffer)
    {
        P2P_OFF_SKILL pkt = new P2P_OFF_SKILL(InBuffer);

        FObjectBase objectBase = FObjectManager.Instance.FindObject(pkt.objectId);
        if (objectBase == null)
            return;

        FSkillController skillController = objectBase.FindController<FSkillController>();
        if (skillController == null)
            return;

        skillController.OffSkill(pkt.skillId);
    }
}