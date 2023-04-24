using UnityEngine;

public class FDamageEffect : FEffect
{
    int damage;

    public override void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        base.Initialize(InEffectData, InOwner, InTarget);

        damage = FGlobal.CalcEffectValue(InOwner, InEffectData.value, InEffectData.valuePerLevel, InEffectData.valuePerBattleLevel);

        DamageToTarget();
    }

    private void DamageToTarget()
    {
        if (owner.IsOwnLocalPlayer() == false)
            return;
        
        if (0 < radius)
        {
            FObjectManager.Instance.ForeachSortedEnemy((FObjectBase InObject) =>
            {
                if (radius + InObject.transform.localScale.x * 0.5 < Vector2.Distance(InObject.WorldPosition, WorldPosition))
                    return;

                DamageToTarget(InObject, damage);
            });
        }
        else if (target != null)
        {
            DamageToTarget(target, damage);
        }
    }
}
