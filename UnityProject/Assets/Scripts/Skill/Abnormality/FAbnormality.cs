using FEnum;
using System;
using UnityEngine;
using UnityEngine.UI;

public class FAbnormality 
{
    FObjectBase target;
    FTimer durationTimer = new FTimer();
    
    int abnormalityID;
    int maxOverlap;
    int overlap;

    StatType statType;
    float originStat;
    GameObject effect;

    public int ID { get { return abnormalityID; } }

    public void Initialize(FObjectBase InTarget, int InID) 
    {
        FAbnormalityData abnormalityData = FAbnormalityDataManager.Instance.FindAbnormalityData(InID);
        if (abnormalityData == null)
            return;

        abnormalityID = InID;
        target = InTarget;
        overlap = 1;
        maxOverlap = abnormalityData.maxOverlap;
        durationTimer.Interval = abnormalityData.duration;

        statType = abnormalityData.statType;

        FStatController statController = InTarget.FindController<FStatController>();
        if(statController != null)
        {
            originStat = statController.GetStat(statType);
        }

        OnEffect();
    }

    public void Release()
    {
        OffEffect();
    }

    public void Tick(float InDelta)
    {
        if (durationTimer.Interval == 0)
            return;

        durationTimer.Tick(InDelta);
        if (durationTimer.IsElapsedCheckTime())
        {
            FAbnormalityController abnormalityController = target.FindController<FAbnormalityController>();
            if(abnormalityController != null)
            {
                abnormalityController.RemoveAbnormality(abnormalityID);
            }
        }
    }

    public void  Overlap()
    {
        durationTimer.Start();

        if (overlap == maxOverlap)
            return;

        ++overlap;

        OnEffect();
    }

    private void OnEffect()
    {
        FAbnormalityOverlapData abnormalityData = FAbnormalityDataManager.Instance.FindAbnormalityOverlapData(abnormalityID, overlap);
        if (abnormalityData == null)
            return;

        FStatController statController = target.FindController<FStatController>();
        if (statController == null)
            return;

        statController.SetStat(statType, originStat * abnormalityData.effectPercentage);
        if (abnormalityData.effectImage != null)
        {
            if(effect != null)
                GameObject.Destroy(effect);

            effect = new GameObject("effect");
            SpriteRenderer spriteRenderer = effect.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(abnormalityData.effectImage);
            spriteRenderer.sortingLayerName = "Object";
            effect.transform.SetParent(target.transform, false);
        }
    }

    private void OffEffect()
    {
        FStatController statController = target.FindController<FStatController>();
        if (statController != null)
        {
            statController.SetStat(statType, originStat);
        }

        if (effect != null)
        {
            GameObject.Destroy(effect);
        }
    }
}
