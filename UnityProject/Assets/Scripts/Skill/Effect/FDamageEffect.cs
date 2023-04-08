using UnityEngine;
using FEnum;

public class FDamageEffect : FEffect
{
    int damage;

    public override void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        base.Initialize(InEffectData, InOwner, InTarget);
        damage = CalcDamage(InEffectData);

        DamageToTarget();
    }

    private void DamageToTarget()
    {
        if (owner.IsOwnLocalPlayer() == false)
            return;

        FStatController statController = owner.FindController<FStatController>();
        if (statController == null)
            return;

        bool critical = statController.IsCritical();
        int damage = (int)(critical ? this.damage * statController.GetStat(StatType.CriticalDamage) : this.damage);
        if (0 < radius)
        {
            FObjectManager.Instance.ForeachObject((FObjectBase InObject) =>
            {
                FIFFController iffController = InObject.FindController<FIFFController>();
                if (iffController == null)
                    return;

                if (iffController.IsEnumy(IFFType.LocalPlayer) == false)
                    return;

                if (radius + InObject.transform.localScale.x * 0.5 < Vector2.Distance(InObject.WorldPosition, WorldPosition))
                    return;

                DamageToTarget(InObject, damage, critical);
            });
        }
        else if (target != null)
        {
            DamageToTarget(target, damage, critical);
        }
    }

    private int CalcDamage(FEffectData InData)
    {
        if (owner.IsOwnLocalPlayer() == false)
            return 0;

        FStatController statController = owner.FindController<FStatController>();
        if (statController == null)
            return 0;

        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController == null)
            return 0;

        FEquipBattleDice battleDice = battleController.FindEquipBattleDice(owner.ContentID);
        if (battleDice == null)
            return 0;

        return InData.damage + InData.damagePerLevel * statController.GetIntStat(StatType.Level) + InData.damagePerBattleLevel * battleDice.level;
    }
}
