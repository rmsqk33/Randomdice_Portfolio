using System.Collections.Generic;
using UnityEngine;

public class FBattleObjectCreator : FSingleton<FBattleObjectCreator>
{
    [SerializeField]
    List<Transform> remotePlayerDiceSlotList;
    [SerializeField]
    List<Transform> localPlayerDiceSlotList;
    [SerializeField]
    FLocalPlayerBattleDice dicePrefab;

    public FLocalPlayerBattleDice CreateLocalPlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || localPlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        FLocalPlayerBattleDice dice = GameObject.Instantiate<FLocalPlayerBattleDice>(dicePrefab, localPlayerDiceSlotList[InSlotIndex]);
        dice.Initialize(InDiceID, InEyeCount, InSlotIndex);

        return dice;
    }
}