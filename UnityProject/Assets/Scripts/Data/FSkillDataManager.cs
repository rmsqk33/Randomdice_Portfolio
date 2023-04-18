using FEnum;
using System.Collections.Generic;

public class FSkillData
{
    public readonly int id;
    public readonly SkillType skillType;
    public readonly SkillTargetType targetType;
    public readonly int projectileID;
    public readonly int abnormalityID;
    public readonly float interval;
    public readonly float duration;

    public FSkillData(FDataNode InNode)
    {
        id = InNode.GetIntAttr("id");
        skillType = (SkillType)InNode.GetIntAttr("skillType");
        targetType = (SkillTargetType)InNode.GetIntAttr("targetType");
        projectileID = InNode.GetIntAttr("projectileID");
        abnormalityID = InNode.GetIntAttr("abnormalityID");
        interval = InNode.GetFloatAttr("interval");
        duration = InNode.GetFloatAttr("duration");
    }
}

public class FSkillDataManager : FNonObjectSingleton<FSkillDataManager>
{
    Dictionary<int, FSkillData> skillDataMap = new Dictionary<int, FSkillData>();

    public void Initialize()
    {
        List<FDataNode> skillDataNodeList =  FDataCenter.Instance.GetDataNodesWithQuery("SkillDataList.SkillData");
        foreach(FDataNode node in skillDataNodeList)
        {
            FSkillData skillData = new FSkillData(node);
            skillDataMap.Add(skillData.id, skillData);
        }
    }

    public FSkillData FindSkillData(int InID)
    {
        if (skillDataMap.ContainsKey(InID))
            return skillDataMap[InID];

        return null;
    }
}
