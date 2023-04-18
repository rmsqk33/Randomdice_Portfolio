
public class FAbnormalitySkill : FSkillBase
{
    public FAbnormalitySkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    public override void UseSkill()
    {
        if (target == null)
            return;

        FAbnormalityController abnormalityController = target.FindController<FAbnormalityController>();
        if (abnormalityController == null)
            return;

        abnormalityController.AddAbnormality(owner, abnormalityID);
    }

}
