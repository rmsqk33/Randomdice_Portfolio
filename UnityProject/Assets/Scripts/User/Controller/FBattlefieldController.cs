using Packet;
using System.Collections.Generic;
using UnityEngine;

public class FBattlefieldController : FControllerBase
{
    List<int> acquiredBattleFielDList = new List<int>();

    public FBattlefieldController(FLocalPlayer InOwner) : base(InOwner)
    {
    }

    public void Handle_S_USER_DATA(in S_USER_DATA InPacket)
    {
        foreach(int id in InPacket.battleFieldIDList)
        {
            if (id == 0)
                break;

            acquiredBattleFielDList.Add(id);
        }

        FBattleFieldInventory battleFieldInventory = FindBattleFieldInventoryUI();
        if (battleFieldInventory != null)
            battleFieldInventory.InitInventory();
    }

    public bool IsAcquiredBattleField(int InID)
    {
        return acquiredBattleFielDList.Contains(InID);
    }

    FBattleFieldInventory FindBattleFieldInventoryUI()
    {
        return FUIManager.Instance.FindUI<FBattleFieldInventory>();
    }
}
