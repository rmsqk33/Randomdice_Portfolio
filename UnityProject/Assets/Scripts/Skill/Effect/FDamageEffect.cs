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
            FObjectManager.Instance.ForeachObject((FObjectBase InObject) =>
            {
                FIFFController iffController = InObject.FindController<FIFFController>();
                if (iffController == null)
                    return;

                if (iffController.IsEnumy(IFFType.LocalPlayer) == false)
                    return;

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
