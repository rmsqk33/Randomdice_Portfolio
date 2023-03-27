using System.Collections.Generic;
using UnityEngine;

public class FBattleDiceController : FControllerBase
{
    int slotIndex;
    int diceLevel;
    float criticalChance;
    float criticalDamageRate;
    List<Transform> eyeList;

    public int DiceID { get { return Owner.ContentID; } }
    public int DiceLevel { get { return diceLevel; } }
    public int SlotIndex { get { return slotIndex; } }
    public int EyeCount { get { return eyeList.Count; } }
    public float CriticalChance { get { return criticalChance; } }
    public float CriticalDamageRate { get { return criticalDamageRate; } }
    
    public FBattleDiceController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void Initialize(int InSlotIndex, List<Transform> InEyeList)
    {
        slotIndex = InSlotIndex;
        eyeList = InEyeList;
        criticalChance = 0.33f;
        
        if(Owner.IsOwnLocalPlayer())
        {
            FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
            if(diceController != null)
            {
                FDice dice = diceController.FindAcquiredDice(DiceID);
                if(dice != null)
                {
                    diceLevel = dice.level;
                }
            }

            FLocalPlayerStatController localPlayerStatController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
            if(localPlayerStatController != null)
            {
                criticalDamageRate = localPlayerStatController.Critical / 100.0f;
            }
        }
    }

    public void CombineDice(FObjectBase InDestDice)
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController == null)
            return;

        FBattleDiceController diceController = InDestDice.FindController<FBattleDiceController>();
        if (diceController == null)
            return;

        battleController.RemoveSummonDice(diceController.SlotIndex);
        battleController.RemoveSummonDice(SlotIndex);
        battleController.SummonDiceRandomDice(diceController.SlotIndex, diceController.EyeCount + 1);
    }

    public bool IsCombinable(FObjectBase InDestDice)
    {
        FBattleDiceController diceController = InDestDice.FindController<FBattleDiceController>();
        if (diceController == null)
            return false;

        if (diceController.DiceID != DiceID)
            return false;

        if (diceController.EyeCount != EyeCount)
            return false;

        if (FBattleDataManager.Instance.MaxEyeCount <= EyeCount)
            return false;

        return true;
    }

    public void PlayEyeAttackAnim(int InIndex)
    {
        if (InIndex < 0 || eyeList.Count <= InIndex)
            return;

        Animator anim = eyeList[InIndex].GetComponent<Animator>();
        if (anim == null)
            return;

        anim.SetTrigger("attack");
    }

    public Vector2 GetEyePosition(int InIndex)
    {
        if (InIndex < 0 || eyeList.Count <= InIndex)
            return Vector2.zero;
        
        return eyeList[InIndex].transform.position;
    }

    public bool IsCritical()
    {
        return criticalChance <= Random.value;
    }
}
