using Packet;
using System.Collections.Generic;
using UnityEngine;

public class FInventoryController : FControllerBase
{
    List<int> acquiredBattleFielDList = new List<int>();

    public int Gold { get; private set; }
    public int Dia { get; private set; }

    public FInventoryController(FLocalPlayer InOwner) : base(InOwner)
    {
    }

    public void Handle_S_USER_DATA(in S_USER_DATA InPacket)
    {
        Gold = InPacket.gold;
        Dia = InPacket.dia;
    }

    public void Handle_S_CHANGE_GOLD(in S_CHANGE_GOLD InPacket)
    {
        Gold = InPacket.gold;

        FLobbyUserInfoUI ui = FindLobbyUserInfoUI();
        if (ui != null)
        {
            ui.Gold = Gold;
        }
    }

    public void Handle_S_CHANGE_DIA(in S_CHANGE_DIA InPacket)
    {
        Dia = InPacket.dia;

        FLobbyUserInfoUI ui = FindLobbyUserInfoUI();
        if(ui != null)
        {
            ui.Dia = Dia;
        }
    }

    FLobbyUserInfoUI FindLobbyUserInfoUI()
    {
        return FUIManager.Instance.FindUI<FLobbyUserInfoUI>();
    }
}
