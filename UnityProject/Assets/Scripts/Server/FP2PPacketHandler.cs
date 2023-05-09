using FEnum;
using Packet;
using Unity.VisualScripting;
using UnityEngine;

public class FP2PPacketHandler
{
    [RuntimeInitializeOnLoadMethod]
    static void RegistPacketHandler()
    {
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_PLAYER_DATA, Handle_P2P_PLAYER_DATA);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_CHANGE_DICE_LEVEL, Handle_P2P_CHANGE_DICE_LEVEL);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_SPAWN_REMOTE_DICE, Handle_P2P_SPAWN_REMOTE_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_C_REQUEST_SPAWN_DICE, Handle_C_REQUEST_SPAWN_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_S_REQUEST_SPAWN_DICE, Handle_S_REQUEST_SPAWN_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_DESPAWN_OBJECT, Handle_P2P_DESPAWN_OBJECT);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_CHANGE_LIFE, Handle_P2P_CHANGE_LIFE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_SPAWN_ENEMY, Handle_P2P_SPAWN_ENEMY);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_CHANGE_WAVE, Handle_P2P_CHANGE_WAVE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_DAMAGE, Handle_P2P_DAMAGE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_READY_BATTLE, Handle_P2P_READY_BATTLE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_ON_SKILL, Handle_P2P_ON_SKILL);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_USE_SKILL_IN_PATH, Handle_P2P_USE_SKILL_IN_PATH);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_OFF_SKILL, Handle_P2P_OFF_SKILL);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_SPAWN_COLLISION_OBJECT, Handle_P2P_SPAWN_COLLISION_OBJECT);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_P2P_REQUEST_COLLISION_OBJECT, Handle_P2P_REQUEST_COLLISION_OBJECT);
    }

    static void Handle_P2P_READY_BATTLE(in byte[] InBuffer)
    {
        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if (statController == null)
            return;

        FPresetController presetController = FGlobal.localPlayer.FindController<FPresetController>();
        if (presetController == null)
            return;

        FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
        if (diceController == null)
            return;

        P2P_PLAYER_DATA playerDataPkt = new P2P_PLAYER_DATA();
        playerDataPkt.name = statController.Name;
        playerDataPkt.level = statController.Level;
        playerDataPkt.criticalDamageRate = statController.CriticalDamageRate;

        int i = 0;
        presetController.ForeachDicePreset(presetController.SelectedPresetIndex, (int InDiceID) =>
        {
            playerDataPkt.diceList[i].id = InDiceID;

            FDice dice = diceController.FindAcquiredDice(InDiceID);
            if(dice != null)
            {
                playerDataPkt.diceList[i].level = dice.level;
            }
            ++i;
        });

        FServerManager.Instance.SendMessage(playerDataPkt);
    }

    static void Handle_P2P_PLAYER_DATA(in byte[] InBuffer)
    {
        P2P_PLAYER_DATA pkt = new P2P_PLAYER_DATA(InBuffer);

        FObjectManager.Instance.CreateRemotePlayer();

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
            battleController.SetDiceBattleLevel(pkt.index, pkt.level);
        }
    }

    static void Handle_P2P_SPAWN_REMOTE_DICE(in byte[] InBuffer)
    {
        P2P_SPAWN_REMOTE_DICE pkt = new P2P_SPAWN_REMOTE_DICE(InBuffer);

        FObjectManager.Instance.CreateRemotePlayerBattleDice(pkt.objectId, pkt.diceId, pkt.eyeCount, pkt.index);
    }

    static void Handle_C_REQUEST_SPAWN_DICE(in byte[] InBuffer)
    {
        P2P_C_REQUEST_SPAWN_DICE pkt = new P2P_C_REQUEST_SPAWN_DICE(InBuffer);

        FObjectManager.Instance.CreateRemotePlayerBattleDice(pkt.diceId, pkt.eyeCount, pkt.index);
    }

    static void Handle_S_REQUEST_SPAWN_DICE(in byte[] InBuffer)
    {
        P2P_S_REQUEST_SPAWN_DICE pkt = new P2P_S_REQUEST_SPAWN_DICE(InBuffer);

        FBattleDiceController diceController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if(diceController != null)
        {
            FObjectManager.Instance.CreateLocalPlayerBattleDice(pkt.objectId, pkt.diceId, pkt.eyeCount, pkt.index);
            diceController.AddBattleDice(pkt.objectId, pkt.diceId, pkt.eyeCount, pkt.index);
        }
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

        FPath startPoint = FPathManager.Instance.FindStartPoint(pkt.spawnPointIndex);
        if (startPoint == null)
            return;

        FObjectManager.Instance.CreateEnemy(pkt.instanceId, pkt.enemyId, startPoint);
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
         
            FStatController statController = target.FindController<FStatController>();
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

    static void Handle_P2P_USE_SKILL_IN_PATH(in byte[] InBuffer)
    {
        P2P_USE_SKILL_IN_PATH pkt = new P2P_USE_SKILL_IN_PATH(InBuffer);

        FObjectBase objectBase = FObjectManager.Instance.FindObject(pkt.objectId);
        if (objectBase == null)
            return;

        FSkillController skillController = objectBase.FindController<FSkillController>();
        if (skillController == null)
            return;

        skillController.OnSkillInPath(pkt.skillId, pkt.pathRate);
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

    static void Handle_P2P_SPAWN_COLLISION_OBJECT(in byte[] InBuffer)
    {
        P2P_SPAWN_COLLISION_OBJECT pkt = new P2P_SPAWN_COLLISION_OBJECT(InBuffer);

        FObjectBase owner = FObjectManager.Instance.FindObject(pkt.ownerObjectId);
        if(owner != null)
        {
            FObjectManager.Instance.CreateCollisionObject(pkt.objectId, pkt.collisionObjectId, owner, pkt.pathRate);
        }
    }

    static void Handle_P2P_REQUEST_COLLISION_OBJECT(in byte[] InBuffer)
    {
        P2P_REQUEST_COLLISION_OBJECT pkt = new P2P_REQUEST_COLLISION_OBJECT(InBuffer);

        FObjectBase owner = FObjectManager.Instance.FindObject(pkt.ownerObjectId);
        if (owner != null)
        {
            FObjectManager.Instance.CreateCollisionObject(pkt.collisionObjectId, owner, pkt.pathRate);
        }
    }
}