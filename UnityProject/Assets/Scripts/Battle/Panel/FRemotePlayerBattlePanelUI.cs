using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FRemotePlayerBattlePanelUI : FUIBase
{
    [SerializeField]
    TextMeshProUGUI nickname;
    [SerializeField]
    Image classIcon;
    [SerializeField]
    List<FRemotePlayerEquipBattleDiceSlot> diceSlotList;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        if (FGlobal.remotePlayer == null)
            return;

        FRemotePlayerBattleController remotePlayerBattleController = FGlobal.remotePlayer.FindController<FRemotePlayerBattleController>();
        if (remotePlayerBattleController == null)
            return;
     
        nickname.text = remotePlayerBattleController.Name;
        classIcon.sprite = Resources.Load<Sprite>(FDataCenter.Instance.GetStringAttribute("UserClass.Class[@class=" + remotePlayerBattleController.Level + "]@icon"));

        int i = 0;
        remotePlayerBattleController.ForeachEquipBattleDice((int InDiceID, int InLevel) =>
        {
            FRemotePlayerEquipBattleDiceSlot diceSlot = diceSlotList[i];
            diceSlot.SetDice(InDiceID);
            diceSlot.SetLevel(InLevel);

            ++i;
        });
    }

    public void SetDiceLevel(int InIndex, int InLevel)
    {
        if (InIndex < 0 || diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetLevel(InLevel);
    }
}
