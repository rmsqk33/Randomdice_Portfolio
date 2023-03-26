using Packet;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FPacketHandler
{
    [RuntimeInitializeOnLoadMethod]
    static void RegistPacketHandler()
    {
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_GUEST_LOGIN, Handle_S_GUEST_LOGIN);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CREATE_GUEST_ACCOUNT, Handle_S_CREATE_GUEST_ACCOUNT);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_USER_DATA, Handle_S_USER_DATA);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_STORE_DICE_LIST, Handle_S_STORE_DICE_LIST);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CHANGE_GOLD, Handle_S_CHANGE_GOLD);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CHANGE_DIA, Handle_S_CHANGE_DIA);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CHANGE_CARD, Handle_S_CHANGE_CARD);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CHANGE_EXP, Handle_S_CHANGE_EXP);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CHANGE_LEVEL, Handle_S_CHANGE_LEVEL);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_PURCHASE_DICE, Handle_S_PURCHASE_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_PURCHASE_BOX, Handle_S_PURCHASE_BOX);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_PURCHASE_BATTLEFIELD, Handle_S_PURCHASE_BATTLEFIELD);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_ADD_DICE, Handle_S_ADD_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_UPGRADE_DICE, Handle_S_UPGRADE_DICE);
        FServerManager.Instance.AddPacketHandler(Packet.PacketType.PACKET_TYPE_S_CHANGE_NAME, Handle_S_CHANGE_NAME);

    }

    static void Handle_S_GUEST_LOGIN(in byte[] InBuffer)
    {
        S_GUEST_LOGIN pkt = new S_GUEST_LOGIN(InBuffer);

        FAccountMananger.Instance.Handle_S_GUEST_LOGIN(pkt.result != 0);
    }

    static void Handle_S_CREATE_GUEST_ACCOUNT(in byte[] InBuffer)
    {
        S_CREATE_GUEST_ACCOUNT pkt = new S_CREATE_GUEST_ACCOUNT(InBuffer);

        FAccountMananger.Instance.Handle_S_CREATE_GUEST_ACCOUNT(pkt.id);
    }

    static void Handle_S_USER_DATA(in byte[] InBuffer)
    {
        S_USER_DATA pkt = new S_USER_DATA(InBuffer);

        FInventoryController inventoryController = FGlobal.localPlayer.FindController<FInventoryController>();
        if (inventoryController != null)
        {
            inventoryController.Handle_S_USER_DATA(pkt);
        }

        FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
        if (diceController != null)
        {
            diceController.Handle_S_USER_DATA(pkt);
        }

        FBattlefieldController battlefieldController = FGlobal.localPlayer.FindController<FBattlefieldController>();
        if (battlefieldController != null)
        {
            battlefieldController.Handle_S_USER_DATA(pkt);
        }

        FPresetController presetController = FGlobal.localPlayer.FindController<FPresetController>();
        if (presetController != null)
        {
            presetController.Handle_S_USER_DATA(pkt);
        }

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if (statController != null)
        {
            statController.Handle_S_USER_DATA(pkt);
        }

#if DEBUG
        if (SceneManager.GetActiveScene().name == "BattleScene")
        {
            FGlobal.localPlayer.AddController<FBattleController>();
        }
#endif
    }

    static void Handle_S_STORE_DICE_LIST(in byte[] InBuffer)
    {
        S_STORE_DICE_LIST pkt = new S_STORE_DICE_LIST(InBuffer);

        FStoreController storeController = FGlobal.localPlayer.FindController<FStoreController>();
        if (storeController != null)
        {
            storeController.Handle_S_STORE_DICE_LIST(pkt);
        }
    }

    static void Handle_S_PURCHASE_DICE(in byte[] InBuffer)
    {
        S_PURCHASE_DICE pkt = new S_PURCHASE_DICE(InBuffer);

        FStoreController storeController = FGlobal.localPlayer.FindController<FStoreController>();
        if (storeController != null)
        {
            storeController.Handle_S_PURCHASE_DICE(pkt);
        }
    }

    static void Handle_S_ADD_DICE(in byte[] InBuffer)
    {
        S_ADD_DICE pkt = new S_ADD_DICE(InBuffer);

        FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
        if (diceController != null)
        {
            diceController.Handle_S_ADD_DICE(pkt);
        }
    }

    static void Handle_S_CHANGE_GOLD(in byte[] InBuffer)
    {
        S_CHANGE_GOLD pkt = new S_CHANGE_GOLD(InBuffer);

        FInventoryController inventoryController = FGlobal.localPlayer.FindController<FInventoryController>();
        if(inventoryController != null)
        {
            inventoryController.Handle_S_CHANGE_GOLD(pkt);
        }
    }

    static void Handle_S_CHANGE_DIA(in byte[] InBuffer)
    {
        S_CHANGE_DIA pkt = new S_CHANGE_DIA(InBuffer);

        FInventoryController inventoryController = FGlobal.localPlayer.FindController<FInventoryController>();
        if (inventoryController != null)
        {
            inventoryController.Handle_S_CHANGE_DIA(pkt);
        }
    }

    static void Handle_S_CHANGE_CARD(in byte[] InBuffer)
    {
        S_CHANGE_CARD pkt = new S_CHANGE_CARD(InBuffer);

        FInventoryController inventoryController = FGlobal.localPlayer.FindController<FInventoryController>();
        if (inventoryController != null)
        {
            inventoryController.Handle_S_CHANGE_CARD(pkt);
        }
    }

    static void Handle_S_CHANGE_EXP(in byte[] InBuffer)
    {
        S_CHANGE_EXP pkt = new S_CHANGE_EXP(InBuffer);

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if (statController != null)
        {
            statController.Handle_S_CHANGE_EXP(pkt);
        }
    }

    static void Handle_S_CHANGE_LEVEL(in byte[] InBuffer)
    {
        S_CHANGE_LEVEL pkt = new S_CHANGE_LEVEL(InBuffer);

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if (statController != null)
        {
            statController.Handle_S_CHANGE_LEVEL(pkt);
        }
    }
    
    static void Handle_S_PURCHASE_BOX(in byte[] InBuffer)
    {
        S_PURCHASE_BOX pkt = new S_PURCHASE_BOX(InBuffer);

        FStoreController storeController = FGlobal.localPlayer.FindController<FStoreController>();
        if(storeController != null)
        {
            storeController.Handle_S_PURCHASE_BOX(pkt);
        }
    }

    static void Handle_S_UPGRADE_DICE(in byte[] InBuffer)
    {
        S_UPGRADE_DICE pkt = new S_UPGRADE_DICE(InBuffer);

        FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
        if (diceController != null)
        {
            diceController.Handle_S_UPGRADE_DICE(pkt);
        }
    }

    static void Handle_S_PURCHASE_BATTLEFIELD(in byte[] InBuffer)
    {
        S_PURCHASE_BATTLEFIELD pkt = new S_PURCHASE_BATTLEFIELD(InBuffer);

        FBattlefieldController battlefieldController = FGlobal.localPlayer.FindController<FBattlefieldController>();
        if (battlefieldController != null)
        {
            battlefieldController.Handle_S_PURCHASE_BATTLEFIELD(pkt);
        }
    }

    static void Handle_S_CHANGE_NAME(in byte[] InBuffer)
    {
        S_CHANGE_NAME pkt = new S_CHANGE_NAME(InBuffer);

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if(statController != null)
        {
            statController.Handle_S_CAHNGE_NAME(pkt);
        }
    }
}