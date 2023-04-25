using UnityEngine;

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

        if (owner.SummonOwner == null)
            return;

        FSkillAreaController skillAreaController = owner.SummonOwner.FindController<FSkillAreaController>();
        if (skillAreaController == null)
            return;

        if (0 < radius)
        {
            skillAreaController.ForeachEnemy((FObjectBase InObject) =>
            {
                if (radius + InObject.transform.localScale.x * 0.5 < Vector2.Distance(InObject.WorldPosition, WorldPosition))
                    return;

                FObjectManager.Instance.DamageToTarget(owner, InObject, (int)effectValue);
            });
        }
        else if (target != null)
        {
            FObjectManager.Instance.DamageToTarget(owner, target, (int)effectValue);
        }
    }
}
