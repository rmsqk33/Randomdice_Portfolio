using FEnum;
using System.Collections.Generic;

public class FSkillController : FControllerBase
{
    Dictionary<int, FSkillBase> skillMap = new Dictionary<int, FSkillBase>();

    public FSkillController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public override void Initialize()
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(Owner.ContentID);
        if (diceData == null)
            return;

        diceData.ForeachSkillID((int InID) => {
            skillMap.Add(InID, CreateSkill(InID));
        });
    }

    public override void Tick(float InDeltaTime)
    {
        foreach(var pair in skillMap)
        {
            pair.Value.Tick(InDeltaTime);
        }
    }

    public void OnSkill(int InSkillID, int InTargetID)
    {
        if(skillMap.ContainsKey(InSkillID))
        {
            FSkillBase skill = skillMap[InSkillID];
            skill.Target = FObjectManager.Instance.FindObject(InTargetID);
            skill.Toggle = true;
            skill.UseSkill();
        }
    }

    public void OffSkill(int InSkillID)
    {
        if (skillMap.ContainsKey(InSkillID))
        {
            skillMap[InSkillID].Toggle = false;
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
        }

        return skill;
    }
}
