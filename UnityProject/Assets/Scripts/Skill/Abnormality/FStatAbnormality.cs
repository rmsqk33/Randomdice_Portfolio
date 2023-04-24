using FEnum;
using UnityEngine;

public class FStatAbnormality : FAbnormality
{
    StatType statType;
    float originStat;

    protected override void Initialize(FAbnormalityData InAbnormalityData)
    {
        statType = InAbnormalityData.statType;

        FStatController statController = target.FindController<FStatController>();
        if (statController != null)
        {
            originStat = statController.GetStat(statType);
        }
    }

    protected override void OnEffect(FAbnormalityOverlapData InAbnormalityData)
    {
        FStatController statController = target.FindController<FStatController>();
        if (statController == null)
            return;

        statController.SetStat(statType, originStat + originStat * (effectValue * InAbnormalityData.effectPercentage));
    }

    protected override void OffEffect()
    {
        FStatController statController = target.FindController<FStatController>();
        if (statController != null)
        {
            statController.SetStat(statType, originStat);
        }
    }
}
