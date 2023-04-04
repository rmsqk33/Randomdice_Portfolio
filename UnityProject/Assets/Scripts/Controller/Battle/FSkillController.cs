using FEnum;
using Packet;
using System.Collections.Generic;

public class FSkillController : FControllerBase
{
    List<FSkillBase> skillList = new List<FSkillBase>();

    public FSkillController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public override void Initialize()
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(Owner.ContentID);
        if (diceData == null)
            return;

        diceData.ForeachSkillID((int InID) => {
            skillList.Add(CreateSkill(InID));
        });
    }

    public override void Tick(float InDeltaTime)
    {
        foreach(FSkillBase skill in skillList)
        {
            skill.Tick(InDeltaTime);
        }
    }

    public void Handle_P2P_USE_SKILL(P2P_USE_SKILL InPacket)
    {

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
        }

        return skill;
    }
}
