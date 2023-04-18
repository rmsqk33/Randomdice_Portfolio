using UnityEngine;
using FEnum;

public class FDamageEffect : FEffect
{
    public override void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        base.Initialize(InEffectData, InOwner, InTarget);

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
