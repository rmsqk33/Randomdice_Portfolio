using FEnum;
using System;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

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
    private float chainRadius;
    private Transform effectPrefab;
    private Transform chainPrefab;

    public override void Initialize(FEffectData InEffectData, FObjectBase InOwner, FObjectBase InTarget = null)
    {
        base.Initialize(InEffectData, InOwner, InTarget);
        
        chainCount = InEffectData.chainCount;
        chainDamageRate = InEffectData.chainDamageRate;
        chainRadius = InEffectData.chainRadius;
        chainPrefab = Resources.Load<Transform>(InEffectData.chainPrefab);
        effectPrefab = Resources.Load<Transform>(InEffectData.prefab);

        DamageToTarget();
    }

    private void DamageToTarget()
    {
        if (owner.IsOwnLocalPlayer() == false)
            return;

        DamageToTarget(target, damage);

        List<FObjectBase> chainTargetList = GetChainTargetList();
        if (0 < chainTargetList.Count)
        {
            FObjectBase prevTarget = target;
            for(int i = 0; i < chainTargetList.Count; ++i)
            {
                FObjectBase chainTarget = chainTargetList[i];

                CreateChainEffect(prevTarget, chainTarget);
                DamageToTarget(chainTarget, (int)(damage - (chainDamageRate * (i + 1)) * damage));

                prevTarget = chainTarget;
            }
        }
    }

    private List<FObjectBase> GetChainTargetList()
    {
        List<KeyValuePair<int, FObjectBase>> objectDistanceList = new List<KeyValuePair<int, FObjectBase>>();
        FObjectManager.Instance.ForeachObject((FObjectBase InObject) => {
            if (target == InObject)
                return;

            FIFFController iffController = InObject.FindController<FIFFController>();
            if (iffController == null)
                return;

            if (iffController.IsEnumy(IFFType.LocalPlayer) == false)
                return;

            objectDistanceList.Add(new KeyValuePair<int, FObjectBase>((int)Vector2.Distance(target.WorldPosition, InObject.WorldPosition), InObject));
        });

        objectDistanceList.Sort(new PairCompair());

        List<FObjectBase> retValue = new List<FObjectBase>();
        for(int i = 0; i < Math.Min(chainCount, objectDistanceList.Count); ++i)
        {
            if (chainRadius * (i + 1) < objectDistanceList[i].Key)
                break;

            retValue.Add(objectDistanceList[i].Value);
        }

        return retValue;
    }

    private void CreateChainEffect(FObjectBase InFrom, FObjectBase InTo)
    {
        Transform effect = Instantiate(effectPrefab, this.transform);
        effect.localScale = new Vector2(effect.localScale.x / this.transform.localScale.x, effect.localScale.y / this.transform.localScale.y);
        effect.position = InTo.WorldPosition;

        Transform chain = Instantiate(chainPrefab, this.transform);
        chain.position = InFrom.WorldPosition;

        float distance = Vector2.Distance(InFrom.WorldPosition, InTo.WorldPosition);
        chain.localScale = new Vector2(distance / this.transform.localScale.x, chain.localScale.y / this.transform.localScale.y);

        float angle = -Vector2.Angle(Vector2.right, InTo.WorldPosition - InFrom.WorldPosition);
        chain.Rotate(new Vector3(0, 0, 1), angle);
    }
}
