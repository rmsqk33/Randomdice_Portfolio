using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class FRemotePlayerBattleBoardUI : FUIBase
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
        dice.SetDice(InDiceID, InEyeCount, InSlotIndex, false);
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
}
