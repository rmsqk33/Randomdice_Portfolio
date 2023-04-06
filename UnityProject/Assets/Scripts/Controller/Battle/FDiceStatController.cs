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

    public void Initialize(int InEyeCount, int InDiceLevel, float InCriticalDamageRate)
    {
        eyeCount = InEyeCount;
        diceLevel = InDiceLevel;
        criticalDamageRate = InCriticalDamageRate;
        criticalChance = 0.33f;
    }
    
    public bool IsCritical()
    {
        return criticalChance <= Random.value;
    }
}
