using System.Collections.Generic;
using UnityEngine.Rendering;

public class FAbnormalityController : FControllerBase
{
    List<FAbnormality> abnormalityList = new List<FAbnormality>();

    public FAbnormalityController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void AddAbnormality(FObjectBase InOwner, int InID)
    {
        int index = FindAbnormalityIndex(InID);
        if (index != -1)
        {
            abnormalityList[index].Overlap();
        }
        else
        {
            abnormalityList.Add(CreateAbnormality(InOwner, InID));
        }
    }

    public void RemoveAbnormality(int InID)
    {
        int index = FindAbnormalityIndex(InID);
        if (index != -1)
        {
            abnormalityList[index].Release();
            abnormalityList.RemoveAt(index);
        }
    }

    public override void Tick(float InDeltaTime)
    {
        for (int i = abnormalityList.Count - 1; 0 <= i; --i)
        {
            abnormalityList[i].Tick(InDeltaTime);
        }
    }

    public bool HasAbnormality(int InID)
    {
        return FindAbnormalityIndex(InID) != -1;
    }

    private FAbnormality CreateAbnormality(FObjectBase InOwner, int InID)
    {
        FAbnormalityData abnormalityData = FAbnormalityDataManager.Instance.FindAbnormalityData(InID);
        if (abnormalityData == null)
            return null;

        FAbnormality abnormality = null;

        switch (abnormalityData.type)
        {
            case FEnum.AbnormalityType.Stat: abnormality = new FStatAbnormality(); break;
            case FEnum.AbnormalityType.Damage: abnormality = new FDamageAbnormality(); break;
        }

        abnormality.Initialize(Owner, InOwner, abnormalityData);

        return abnormality;
    }

    private int FindAbnormalityIndex(int InID)
    {
        for(int i = 0; i < abnormalityList.Count; ++i)
        {
            if (abnormalityList[i].ID == InID)
                return i;
        }

        return -1;
    }

}
