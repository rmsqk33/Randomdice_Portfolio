using FEnum;

public class FLockAbnormality : FAbnormality, FObjectStateObserver
{
    int eyeCount;

    protected override void Initialize(FAbnormalityData InAbnormalityData)
    {
        owner.AddObserver(this);
    }

    protected override void OnEffect(FAbnormalityOverlapData InAbnormalityData)
    {
        FStatController statController = target.FindController<FStatController>();
        if (statController != null)
        {
            target.enabled = false;
            eyeCount = statController.GetIntStat(StatType.DiceEye);
            statController.SetStat(StatType.DiceEye, 0);
        }
    }

    protected override void OffEffect()
    {
        FStatController statController = target.FindController<FStatController>();
        if(statController != null)
        {
            target.enabled = true;
            statController.SetStat(StatType.DiceEye, eyeCount);
        }
    }

    public void OnDestroyObject(FObjectBase InObject)
    {
        RemoveAbnormality();
    }
}
