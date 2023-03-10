using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using UnityEngine.UI;

public class FLocalPlayerBattleBoardUI : FUIBase
{
    [SerializeField]
    List<Transform> diceSlotParentList;
    [SerializeField]
    FBattleDiceUI dicePrefab;

    Dictionary<int, FBattleDiceUI> diceMap = new Dictionary<int, FBattleDiceUI>();

    public FBattleDiceUI CreateDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || diceSlotParentList.Count <= InSlotIndex)
            return null;

        if (diceMap.ContainsKey(InSlotIndex))
            return null;

        FBattleDiceUI dice = GameObject.Instantiate<FBattleDiceUI>(dicePrefab, diceSlotParentList[InSlotIndex]);
        dice.SetDice(InDiceID, InEyeCount, InSlotIndex);
        diceMap.Add(InSlotIndex, dice);

        return dice;
    }

    public void RemoveDice(int InSlotIndex)
    {
        if (diceMap.ContainsKey(InSlotIndex) == false)
            return;

        GameObject.Destroy(diceMap[InSlotIndex].gameObject);
        diceMap.Remove(InSlotIndex);
    }

    public void OnBegieDrag(int InSlotIndex)
    {
        ActiveDiceCombinable(InSlotIndex);
    }

    public void OnEndDrag()
    {
        DeactiveDiceCombinable();
    }

    void ActiveDiceCombinable(int InSlotIndex)
    {
        FLocalPlayerBattleController battleController = FLocalPlayer.Instance.FindController<FLocalPlayerBattleController>();
        if (battleController == null)
            return;

        FBattleDice battleDice = battleController.FindSummonDice(InSlotIndex);
        battleController.ForeachSummonDice((FBattleDice InDice) => {
            if (InSlotIndex == InDice.SlotIndex)
                return;

            if(battleDice.IsCombinable(InDice) == false)
            {
                diceMap[InDice.SlotIndex].SetEnable(false);
            }
        });
    }

    void DeactiveDiceCombinable()
    {
        foreach (var pair in diceMap)
        {
            pair.Value.SetEnable(true);
        }
    }
}