using FEnum;
using System;
using System.Collections.Generic;
using UnityEngine;

public class FChainEffect : FEffect
{
    class PairCompair : IComparer<KeyValuePair<int, FObjectBase>>
    {
        public int Compare(KeyValuePair<int, FObjectBase> x, KeyValuePair<int, FObjectBase> y)
        {
            return x.Key - y.Key;
        }
    }

    private int damage;
    private int chainCount;
    private float chainDamageRate;
    private Transform hitEffectPrefab;
    private Transform chainPrefab;

    public override void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        base.Initialize(InEffectData, InOwner, InTarget);
        
        damage = FGlobal.CalcDamage(InOwner, InEffectData.damage, InEffectData.damagePerLevel, InEffectData.damagePerBattleLevel);
        chainCount = InEffectData.chainCount;
        chainDamageRate = InEffectData.chainDamageRate;
        chainPrefab = Resources.Load<Transform>(InEffectData.chainPrefab);
        hitEffectPrefab = Resources.Load<Transform>(InEffectData.prefab);

        if (owner.IsOwnLocalPlayer())
        {
            DamageToTarget(target, damage);
        }

        ChainEffect();
    }

    private void ChainEffect()
    {
        int i = 0;
        FObjectBase prevTarget = target;
        FObjectManager.Instance.ForeachSortedEnemy(chainCount + 1, (FObjectBase InObject) =>
        {
            if (prevTarget == InObject)
                return;

            CreateChainEffect(prevTarget, InObject);

            if (owner.IsOwnLocalPlayer())
            {
                DamageToTarget(InObject, (int)(damage - (chainDamageRate * (i + 1)) * damage));
            }

            prevTarget = InObject;
            ++i;
        });
    }

    private void CreateChainEffect(FObjectBase InFrom, FObjectBase InTo)
    {
        Transform effect = Instantiate(hitEffectPrefab, this.transform);
        effect.position = InTo.WorldPosition;

        Transform chain = Instantiate(chainPrefab, this.transform);
        chain.position = InFrom.WorldPosition;

        float distance = Vector2.Distance(InFrom.WorldPosition, InTo.WorldPosition);
        chain.localScale = new Vector2(distance, chain.localScale.y);

        float angle = -Vector2.Angle(Vector2.right, InTo.WorldPosition - InFrom.WorldPosition);
        chain.Rotate(new Vector3(0, 0, 1), angle);
    }
}
