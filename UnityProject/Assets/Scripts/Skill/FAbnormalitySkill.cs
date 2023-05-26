
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

    protected override void UseSkillLocal()
    {
        base.UseSkillLocal();
    
        for (int i = 0; i < loopCount; ++i)
        {
            target = GetTarget();
            if (target == null)
                break;

            AddAbnormality(target);

            if (owner.IsOwnLocalPlayer())
                SendUseSkill(target);
        }
    }

    public override void UseSkillRemote()
    {
        if (target == null)
            return;

        base.UseSkillRemote();
        AddAbnormality(target);
    }

    private void AddAbnormality(FObjectBase InTarget)
    {
        FAbnormalityController abnormalityController = InTarget.FindController<FAbnormalityController>();
        if (abnormalityController == null)
            return;

        abnormalityController.AddAbnormality(owner, abnormalityID);        
    }
}
