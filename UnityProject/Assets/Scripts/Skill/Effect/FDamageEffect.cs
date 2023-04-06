using UnityEngine;
using FEnum;

public class FDamageEffect : FEffect
{
    int damage;
    int radius;
    float duration;

    public override void Initialize(int InEffectID, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        FEffectData effectData = FEffectDataManager.Instance.FindEffectData(InEffectID);
        if (effectData == null)
            return;

        base.Initialize(InEffectID, InOwner, InTarget);
        radius = effectData.radius;
        duration = effectData.duration;
        damage = CalcDamage(effectData);
        

        if (duration == 0)
        {
            DamageToTarget();

            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                duration = anim.GetCurrentAnimatorStateInfo(0).length;
            }
        }

        StartCoroutine(RemoveEffect(duration));
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

                if (radius < Vector2.Distance(InObject.WorldPosition, transform.position))
                    return;

                DamageToTarget(target, damage, critical);
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