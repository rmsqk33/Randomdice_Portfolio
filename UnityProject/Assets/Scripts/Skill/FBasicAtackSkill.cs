using FEnum;

public class FBasicSkill : FSkillBase
{
    int damage;
    int eyeCount;
    int attackEyeIndex;

    int projectileID;
    SkillTargetType targetType;
    float attackInterval;
    float elapsedTime;

    public FBasicSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    protected override void Initialize(FSkillData InSkillData) 
    {
        FBattleDiceController battleDiceController = owner.FindController<FBattleDiceController>();
        if (battleDiceController == null)
            return;

        eyeCount = battleDiceController.EyeCount;

        projectileID = InSkillData.projectileID;
        targetType = InSkillData.targetType;
        attackInterval = InSkillData.interval / eyeCount;
    }

    public override void Tick(float InDelta)
    {
        elapsedTime += InDelta;
        
        if(attackInterval <= elapsedTime)
        {
            UseSkill();

            elapsedTime = 0;
        }
    }

    void UseSkill()
    {
        FBattleDiceController battleDiceController = owner.FindController<FBattleDiceController>();
        if (battleDiceController == null)
            return;

        FObjectBase target = GetTarget();
        if (target == null)
            return;

        battleDiceController.PlayEyeAttackAnim(attackEyeIndex);

        FEffectManager.Instance.AddProjectile(projectileID, owner, battleDiceController.GetEyePosition(attackEyeIndex), target);
        attackEyeIndex = (attackEyeIndex + 1) % eyeCount;
    }

    FObjectBase GetTarget()
    {
        switch(targetType)
        {
            case SkillTargetType.Front: return FObjectManager.Instance.FrontEnemy;
        }

        return null;
    }
}
