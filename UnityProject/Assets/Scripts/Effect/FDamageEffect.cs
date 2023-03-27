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

        FBattleDiceController battleDiceController = owner.FindController<FBattleDiceController>();
        if (battleDiceController == null)
            return;

        bool critical = battleDiceController.IsCritical();
        int damage = (int)(critical ? this.damage * battleDiceController.CriticalDamageRate : this.damage);
        if (0 < radius)
        {
            FObjectManager.Instance.ForeachEnemy((FObjectBase InObject) =>
            {
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

    private void DamageToTarget(FObjectBase InTarget, int InDamage, bool InCritical)
    {
        FStatController statController = InTarget.FindController<FStatController>();
        if (statController != null)
        {
            statController.OnDamage(InDamage);
        }

        FCombatTextManager.Instance.AddText(InCritical ? CombatTextType.Critical : CombatTextType.Normal, InDamage, InTarget);
    }

    private int CalcDamage(FEffectData InData)
    {
        if (owner.IsOwnLocalPlayer() == false)
            return 0;

        FBattleDiceController battleDiceController = owner.FindController<FBattleDiceController>();
        if (battleDiceController == null)
            return 0;

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController == null)
            return 0;

        FEquipBattleDice battleDice = battleController.FindBattleDicePreset(battleDiceController.DiceID);
        if (battleDice == null)
            return 0;

        return InData.damage + InData.damagePerLevel * battleDiceController.DiceLevel + InData.damagePerBattleLevel * battleDice.level;
    }
}
