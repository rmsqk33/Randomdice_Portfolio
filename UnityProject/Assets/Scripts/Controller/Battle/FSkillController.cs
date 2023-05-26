using FEnum;
using System.Collections.Generic;

public class FSkillController : FControllerBase
{
    Dictionary<int, FSkillBase> skillMap = new Dictionary<int, FSkillBase>();

    public FSkillController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void Initialize(List<int> InSkillIDList)
    {
        foreach(int id in InSkillIDList)
        {
            skillMap.Add(id, CreateSkill(id));
        }
    }

    public override void Tick(float InDeltaTime)
    {
        foreach(var pair in skillMap)
        {
            pair.Value.Tick(InDeltaTime);
        }
    }

    public void OnUseSkill(int InSkillID, int InTargetID)
    {
        if(skillMap.ContainsKey(InSkillID))
        {
            FSkillBase skill = skillMap[InSkillID];
            skill.Target = FObjectManager.Instance.FindObject(InTargetID);
            skill.UseSkillRemote();
        }
    }

    public void OnSkillInPath(int InSkillID, float InPathRate)
    {
        if (skillMap.ContainsKey(InSkillID))
        {
            skillMap[InSkillID].UseSkillInPath(InPathRate);
        }
    }

    public delegate void ForeachSkillDelegate(FSkillBase InSkill);
    public void ForeachSkill(ForeachSkillDelegate InFunc)
    {
        foreach(var pair in skillMap)
        {
            InFunc(pair.Value);
        }
    }

    private FSkillBase CreateSkill(int InID)
    {
        FSkillData skillData = FSkillDataManager.Instance.FindSkillData(InID);
        if (skillData == null)
            return null;

        FSkillBase skill = null;
        switch (skillData.skillType)
        {
            case SkillType.Basic: skill = new FBasicAttackSkill(Owner, skillData); break;
            case SkillType.Abnormal: skill = new FAbnormalitySkill(Owner, skillData); break;
            case SkillType.Summon: skill = new FSummonSkill(Owner, skillData); break;
        }

        return skill;
    }
}
