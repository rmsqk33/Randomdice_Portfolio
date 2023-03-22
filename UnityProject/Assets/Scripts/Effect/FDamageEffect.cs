using UnityEngine;

public class FDamageEffect : FEffect
{
    int damage;
    int radius;
    float interval;
    float duration;
    float elaplsedTime;

    public override void Initialize(int InEffectID, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        FEffectData effectData = FEffectDataManager.Instance.FindEffectData(InEffectID);
        if (effectData == null)
            return;

        base.Initialize(InEffectID, InOwner, InTarget);
        radius = effectData.radius;
        duration = effectData.duration;
        
        if (InOwner.IsOwnLocalPlayer())
        {
            FBattleDiceController battleDiceController = InOwner.FindController<FBattleDiceController>();
            if (battleDiceController != null)
            {
                damage = effectData.damage + effectData.damagePerLevel * battleDiceController.DiceLevel;
            }
        }

        if (duration == 0)
        {
            DamageToTarget();

            Animator anim = GetComponent<Animator>();
            if (anim != null)
            {
                duration = anim.GetCurrentAnimatorStateInfo(0).length;
            }
        }
    }

    public override void Tick(float InDeltaTime)
    {
        if (0 < interval)
        {
            elaplsedTime += InDeltaTime;
            if (interval <= elaplsedTime)
            {
                elaplsedTime = 0;
                DamageToTarget();
            }
        }

        duration -= InDeltaTime;
        if (duration <= 0)
        {
            RemoveEffect();
        }
    }

    private void DamageToTarget()
    {
        FBattleDiceController battleDiceController = owner.FindController<FBattleDiceController>();
        if (battleDiceController == null)
            return;

        int damage = (int)(battleDiceController.IsCritical() ? this.damage * battleDiceController.CriticalDamageRate : this.damage);
        if (0 < radius)
        {
            FObjectManager.Instance.ForeachEnemy((FObjectBase InObject) =>
            {
                if (radius < Vector2.Distance(InObject.WorldPosition, transform.position))
                    return;

                FStatController statController = InObject.FindController<FStatController>();
                if (statController != null)
                {
                    statController.OnDamage(damage);
                }
            });
        }
        else if (target != null)
        {
            FStatController statController = target.FindController<FStatController>();
            if (statController != null)
            {
                statController.OnDamage(damage);
            }
        }
    }

    private void OnCompleteAnimation()
    {
        RemoveEffect();
    }
}
