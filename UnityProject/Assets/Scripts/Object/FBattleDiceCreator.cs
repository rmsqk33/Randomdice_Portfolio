using System.Collections.Generic;
using UnityEngine;

public class FBattleDiceCreator : FSceneLoadedSingleton<FBattleDiceCreator>
{
    [SerializeField]
    List<Transform> localPlayerDiceSlotList;
    [SerializeField]
    FBattleDice localPlayerDicePrefab;
    [SerializeField]
    List<Transform> remotePlayerDiceSlotList;
    [SerializeField]
    FRemotePlayerBattleDice remotePlayerDicePrefab;

    public FBattleDice CreateLocalPlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || localPlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        FBattleDice dice = GameObject.Instantiate<FBattleDice>(localPlayerDicePrefab, localPlayerDiceSlotList[InSlotIndex]);
        dice.Initialize(InDiceID, InEyeCount, InSlotIndex);

        return dice;
    }

    public FRemotePlayerBattleDice CreateRemotePlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || remotePlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        FRemotePlayerBattleDice dice = GameObject.Instantiate<FRemotePlayerBattleDice>(remotePlayerDicePrefab, remotePlayerDiceSlotList[InSlotIndex]);
        dice.Initialize(InDiceID, InEyeCount);

        return dice;
    }
}