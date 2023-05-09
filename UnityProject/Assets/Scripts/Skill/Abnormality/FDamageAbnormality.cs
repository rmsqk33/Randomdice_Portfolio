using FEnum;

public class FDamageAbnormality : FAbnormality
{
    float criticalDamage;
    float criticalChance;

    bool localPlayerOwner;

    protected override void Initialize(FAbnormalityData InAbnormalityData)
    {
        localPlayerOwner = owner.IsOwnLocalPlayer();
        if (localPlayerOwner)
        {
            FStatController statController = owner.FindController<FStatController>();
            if (statController == null)
                return;

            FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
            if (battleController == null)
                return;

            FEquipBattleDice battleDice = battleController.FindEquipBattleDice(owner.ContentID);
            if (battleDice == null)
                return;

            criticalDamage = statController.GetStat(StatType.CriticalDamage);
            criticalChance = statController.GetStat(StatType.CriticalChance);
        }
    }

    protected override void OnEffect(FAbnormalityOverlapData InAbnormalityData)
    {
        if (localPlayerOwner)
        {
            FObjectManager.Instance.DamageToTarget(target, (int)effectValue, criticalChance, criticalDamage);
        }
    }

    protected override void OffEffect()
    {

    }
}
