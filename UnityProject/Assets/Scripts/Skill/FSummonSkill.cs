
public class FSummonSkill : FSkillBase
{
    public FSummonSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    public override void UseSkillInPath(float InPathRate)
    {
        base.UseSkillInPath(InPathRate);

        FEffectManager.Instance.AddProjectile(projectileID, owner, owner.WorldPosition, InPathRate);
    }
}



