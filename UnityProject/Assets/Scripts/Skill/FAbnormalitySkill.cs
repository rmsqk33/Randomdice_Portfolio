
public class FAbnormalitySkill : FSkillBase
{
    private int abnormalityID;

    public FAbnormalitySkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    protected override void Initialize(FSkillData InSkillData)
    {
        abnormalityID = InSkillData.abnormalityID;
    }

    public override void UseSkill()
    {
        base.UseSkill();

        if (target == null)
            return;

        FAbnormalityController abnormalityController = target.FindController<FAbnormalityController>();
        if (abnormalityController == null)
            return;

        abnormalityController.AddAbnormality(owner, abnormalityID);
    }

}
