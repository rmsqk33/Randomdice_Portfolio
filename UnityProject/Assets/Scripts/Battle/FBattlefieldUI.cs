using System.Collections.Generic;
using UnityEngine;

public class FBattlefieldUI : FUIBase
{
    [SerializeField]
    List<Transform> remotePlayerDiceSlotList;
    [SerializeField]
    List<Transform> localPlayerDiceSlotList;
    [SerializeField]
    FBattleDiceUI dicePrefab;

    Dictionary<int, FBattleDiceUI> remotePlayerDiceMap = new Dictionary<int, FBattleDiceUI>();
    Dictionary<int, FBattleDiceUI> localPlayerDiceMap = new Dictionary<int, FBattleDiceUI>();

    public FBattleDiceUI CreateRemotePlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || remotePlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        if (remotePlayerDiceMap.ContainsKey(InSlotIndex))
            return null;

        FBattleDiceUI dice = GameObject.Instantiate<FBattleDiceUI>(dicePrefab, remotePlayerDiceSlotList[InSlotIndex]);
        dice.SetDice(InDiceID, InEyeCount, InSlotIndex);
        remotePlayerDiceMap.Add(InSlotIndex, dice);

        return dice;
    }

    public void RemoveRemotePlayerDice(int InSlotIndex)
    {
        if (remotePlayerDiceMap.ContainsKey(InSlotIndex) == false)
            return;

        GameObject.Destroy(remotePlayerDiceMap[InSlotIndex].gameObject);
        remotePlayerDiceMap.Remove(InSlotIndex);
    }

    public FBattleDiceUI CreateLocalPlayerDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        if (InSlotIndex < 0 || localPlayerDiceSlotList.Count <= InSlotIndex)
            return null;

        if (localPlayerDiceMap.ContainsKey(InSlotIndex))
            return null;

        FBattleDiceUI dice = GameObject.Instantiate<FBattleDiceUI>(dicePrefab, localPlayerDiceSlotList[InSlotIndex]);
        dice.SetDice(InDiceID, InEyeCount, InSlotIndex);
        localPlayerDiceMap.Add(InSlotIndex, dice);

        return dice;
    }

    public void RemoveLocalPlayerDice(int InSlotIndex)
    {
        if (localPlayerDiceMap.ContainsKey(InSlotIndex) == false)
            return;

        GameObject.Destroy(localPlayerDiceMap[InSlotIndex].gameObject);
        localPlayerDiceMap.Remove(InSlotIndex);
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
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController == null)
            return;

        FBattleDice battleDice = battleController.FindSummonDice(InSlotIndex);
        battleController.ForeachSummonDice((FBattleDice InDice) => {
            if (InSlotIndex == InDice.SlotIndex)
                return;

            if(battleDice.IsCombinable(InDice) == false)
            {
                localPlayerDiceMap[InDice.SlotIndex].SetEnable(false);
            }
        });
    }

    void DeactiveDiceCombinable()
    {
        foreach (var pair in localPlayerDiceMap)
        {
            pair.Value.SetEnable(true);
        }
    }
}