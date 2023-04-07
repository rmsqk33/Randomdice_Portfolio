using System.Collections.Generic;

public class FAbnormalityController : FControllerBase
{
    List<FAbnormality> abnormalityList = new List<FAbnormality>();

    public FAbnormalityController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void AddAbnormality(int InID)
    {
        int index = FindAbnormalityIndex(InID);
        if (index != -1)
        {
            abnormalityList[index].Overlap();
        }
        else
        {
            FAbnormality abnormality = new FAbnormality();
            abnormality.Initialize(Owner, InID);

            abnormalityList.Add(abnormality);
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
