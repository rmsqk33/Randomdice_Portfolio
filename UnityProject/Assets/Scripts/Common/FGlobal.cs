
using FEnum;
using Unity.VisualScripting;
using UnityEngine;

public class FGlobal
{
    public static readonly int MAX_SUMMON_DICE = 15;

    public static readonly int MAX_PRESET_PAGE = 5;
    public static readonly int MAX_PRESET = 5;

    public static FLocalPlayer localPlayer = null;
    public static FRemotePlayer remotePlayer = null;

    public static float DiceCriticalChange = 0.33f;

    public static float CalcEffectValue(FObjectBase InObject, float InValue, float InValuePerLevel, float InValuePerBattleLevel)
    {
        FStatController statController = InObject.FindController<FStatController>();
        if (statController == null)
            return 0;

        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController == null)
            return 0;

        FEquipBattleDice battleDice = battleController.FindEquipBattleDice(InObject.ContentID);
        if (battleDice == null)
            return 0;

        return InValue + InValuePerLevel * statController.GetIntStat(StatType.Level) + InValuePerBattleLevel * battleDice.level;
    }
}
