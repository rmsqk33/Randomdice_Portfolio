using Packet;
using System.Collections.Generic;
using UnityEngine;

public class FRemotePlayerBattleController : FControllerBase
{
    struct EquipDiceInfo
    {
        public int diceID;
        public int level;
    }

    int level;
    string name;
    EquipDiceInfo[] equipDiceList = new EquipDiceInfo[FGlobal.MAX_PRESET];

    public int Level { get { return level; } }
    public string Name { get { return name; } }

    public FRemotePlayerBattleController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void Handle_P2P_PLAYER_DATA(P2P_PLAYER_DATA InPacket)
    {
        level = InPacket.level;
        name = InPacket.name;
        for (int i = 0; i < FGlobal.MAX_PRESET; ++i)
        {
            EquipDiceInfo diceInfo = new EquipDiceInfo();
            diceInfo.diceID = InPacket.diceIdList[i];
            diceInfo.level = 1;

            equipDiceList[i] = diceInfo;
        }

        FRemotePlayerBattlePanelUI ui = FindBattlePanelUI();
        if(ui != null)
        {
            ui.Initialize();
        }
    }

    public void SetDiceLevel(int InIndex, int InLevel)
    {
        if (InIndex < 0 || equipDiceList.Length <= InIndex)
            return;

        equipDiceList[InIndex].level = InLevel;

        FRemotePlayerBattlePanelUI ui = FindBattlePanelUI();
        if (ui != null)
        {
            ui.SetDiceLevel(InIndex, InLevel);
        }
    }

    public delegate void ForeachEquipBattleDiceDelegate(int InDiceID, int InLevel);
    public void ForeachEquipBattleDice(ForeachEquipBattleDiceDelegate InFunc)
    {
        foreach(EquipDiceInfo dice in equipDiceList)
        {
            InFunc(dice.diceID, dice.level);
        }
    }

    private FRemotePlayerBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FRemotePlayerBattlePanelUI>();
    }
}
