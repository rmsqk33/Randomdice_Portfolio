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

    private int chainCount;
    private float chainDamageRate;
    private Transform hitEffectPrefab;
    private Transform chainPrefab;

    public override void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        base.Initialize(InEffectData, InOwner, InTarget);
        
        chainCount = InEffectData.chainCount;
        chainDamageRate = InEffectData.chainDamageRate;
        chainPrefab = Resources.Load<Transform>(InEffectData.chainPrefab);
        hitEffectPrefab = Resources.Load<Transform>(InEffectData.prefab);

        if (owner.IsOwnLocalPlayer())
        {
            DamageToTarget(target, (int)effectValue);
        }

        ChainEffect();
    }

    private void ChainEffect()
    {
        FObjectBase prevTarget = target;

        List<FObjectBase> targetList = FObjectManager.Instance.GetSortedEnemyList(target, chainCount);
        for(int i = 0; i < targetList.Count; ++i)
        {
            CreateChainEffect(prevTarget, targetList[i]);

            if (owner.IsOwnLocalPlayer())
            {
                DamageToTarget(targetList[i], (int)(effectValue - (chainDamageRate * (i + 1)) * effectValue));
            }

            prevTarget = targetList[i];
        }
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
