using System.Collections.Generic;
using UnityEngine;

public class FBattleDiceCreator : FSingleton<FBattleDiceCreator>
{
    [SerializeField]
    List<Transform> remotePlayerDiceSlotList;
    [SerializeField]
    List<Transform> localPlayerDiceSlotList;
    [SerializeField]
    FBattleDice dicePrefab;

    public FBattleDice CreateLocalPlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || localPlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        FBattleDice dice = GameObject.Instantiate<FBattleDice>(dicePrefab, localPlayerDiceSlotList[InSlotIndex]);
        dice.Initialize(InDiceID, InEyeCount, InSlotIndex);

        return dice;
    }

    public FBattleDice CreateRemotePlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || localPlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        FBattleDice dice = GameObject.Instantiate<FBattleDice>(dicePrefab, localPlayerDiceSlotList[InSlotIndex]);
        dice.Initialize(InDiceID, InEyeCount, InSlotIndex);

        return dice;
    }
}