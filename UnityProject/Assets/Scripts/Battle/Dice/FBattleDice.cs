using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FBattleDice
{
    FBattleDiceUI diceSlot;

    public int SlotIndex { get { return diceSlot.SlotIndex; } }
    public int DiceID { get; set; }
    public int EyeCount { get; set; }

    public FBattleDice(int InDiceID, int InEyeCount, FBattleDiceUI InSlot)
    {
        DiceID = InDiceID;
        EyeCount = InEyeCount;
        diceSlot = InSlot;
    }

    public virtual void CombineDice(FBattleDice InDestDice)
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if(battleController != null)
        {
            battleController.RemoveSummonDice(InDestDice.SlotIndex);
            battleController.RemoveSummonDice(SlotIndex);
            battleController.SummonDiceRandomDice(InDestDice.SlotIndex, InDestDice.EyeCount + 1);
        }
    }

    public virtual bool IsCombinable(FBattleDice InDestDice)
    {
        if (InDestDice.DiceID != DiceID)
            return false;

        if (InDestDice.EyeCount != EyeCount)
            return false;

        if (FBattleDataManager.Instance.MaxEyeCount <= EyeCount)
            return false;

        return true;
    }
}
