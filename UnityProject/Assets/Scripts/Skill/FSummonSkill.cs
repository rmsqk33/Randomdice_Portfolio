
public class FSummonSkill : FSkillBase
{
    int summonEnemyID;
    int summonCount;

    public FSummonSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    protected override void Initialize(FSkillData InSkillData)
    {
        summonEnemyID = InSkillData.summonEnemyID;
        summonCount = InSkillData.summonCount;
    }

    public override void UseSkillInPath(float InPathRate)
    {
        base.UseSkillInPath(InPathRate);

        if (projectileID != 0)
            FEffectManager.Instance.AddProjectile(projectileID, owner, owner.WorldPosition, InPathRate);
        else if (summonEnemyID != 0 && FGlobal.localPlayer.IsHost)
            FObjectManager.Instance.CreateEnemy(summonEnemyID, owner.SummonOwner);

    }
}



