using FEnum;
using UnityEngine;

public abstract class FAbnormality 
{
    protected FObjectBase target;
    protected FObjectBase owner;
    protected float effectValue;

    FTimer durationTimer;
    FTimer intervalTimer;
    
    int abnormalityID;
    int maxOverlap;
    int overlap;

    GameObject effect;

    public int ID { get { return abnormalityID; } }

    protected abstract void Initialize(FAbnormalityData InAbnormalityData);
    protected abstract void OnEffect(FAbnormalityOverlapData InAbnormalityData);
    protected abstract void OffEffect();
    
    public void Initialize(FObjectBase InTarget, FObjectBase InOwner, FAbnormalityData InAbnormalityData) 
    {
        abnormalityID = InAbnormalityData.id;
        target = InTarget;
        owner = InOwner;
        overlap = 1;
        maxOverlap = InAbnormalityData.maxOverlap;
        effectValue = FGlobal.CalcEffectValue(owner, InAbnormalityData.value, InAbnormalityData.valuePerLevel, InAbnormalityData.valuePerBattleLevel);

        if (0 < InAbnormalityData.duration)
        {
            durationTimer = new FTimer(InAbnormalityData.duration);
            durationTimer.Start();
        }
        

        if(0 < InAbnormalityData.interval)
        {
            intervalTimer = new FTimer(InAbnormalityData.interval);
            intervalTimer.Start();
        }

        Initialize(InAbnormalityData);

        OnEffect();
    }

    public void Release()
    {
        OffEffect();
        if (effect != null)
        {
            GameObject.Destroy(effect);
        }
    }

    public void Tick(float InDelta)
    {
        if (intervalTimer != null)
        {
            if (intervalTimer.IsElapsedCheckTime())
            {
                FAbnormalityOverlapData abnormalityData = FAbnormalityDataManager.Instance.FindAbnormalityOverlapData(abnormalityID, overlap);
                if (abnormalityData != null)
                {
                    OnEffect(abnormalityData);
                }
                intervalTimer.Restart();
            }
        }

        if (durationTimer != null)
        {
            if (durationTimer.IsElapsedCheckTime())
            {
                FAbnormalityController abnormalityController = target.FindController<FAbnormalityController>();
                if (abnormalityController != null)
                {
                    abnormalityController.RemoveAbnormality(abnormalityID);
                }
            }
        }
    }

    public void  Overlap()
    {
        durationTimer.Restart();

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

        if (abnormalityData.effectImage != null)
        {
            if (effect != null)
                GameObject.Destroy(effect);

            effect = new GameObject("effect");
            SpriteRenderer spriteRenderer = effect.AddComponent<SpriteRenderer>();
            spriteRenderer.sprite = Resources.Load<Sprite>(abnormalityData.effectImage);
            spriteRenderer.sortingLayerName = "Object";
            effect.transform.SetParent(target.transform, false);
        }

        OnEffect(abnormalityData);
    }
}
