
public class FSummonSkill : FSkillBase
{
    public FSummonSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    public override void UseSkillInPath(float InPathRate)
    {
        FEffectManager.Instance.AddProjectile(projectileID, owner, owner.WorldPosition, InPathRate);
    }
}



