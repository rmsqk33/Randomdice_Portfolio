using UnityEngine;

public class FDiceStatController : FControllerBase
{
    int diceLevel;
    int eyeCount;
    float criticalChance;
    float criticalDamageRate;

    public int DiceID { get { return Owner.ContentID; } }
    public int DiceLevel { get { return diceLevel; } }
    public int EyeCount { get { return eyeCount; } }
    public float CriticalChance { get { return criticalChance; } }
    public float CriticalDamageRate { get { return criticalDamageRate; } }
    
    public FDiceStatController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void Initialize(int InEyeCount)
    {
        eyeCount = InEyeCount;
        criticalChance = 0.33f;
        
        if(IsOwnLocalPlayer())
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
    
    public bool IsCritical()
    {
        return criticalChance <= Random.value;
    }
}
