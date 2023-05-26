using FEnum;
using UnityEngine;

public class FSummonSkill : FSkillBase
{
    int summonEnemyID;

    public FSummonSkill(FObjectBase InOwner, FSkillData InSkillData) : base(InOwner, InSkillData)
    {
    }

    protected override void Initialize(FSkillData InSkillData)
    {
        summonEnemyID = InSkillData.summonEnemyID;
        
        FStatController statController = owner.FindController<FStatController>();
        if (statController == null)
            return;

        Transform eyeParent = statController.FindChildComponent<Transform>("Eye");
        if (eyeParent == null)
            return;

        if (InSkillData.skillType == SkillType.SummonBasic)
            OriginInterval = InSkillData.interval / statController.GetIntStat(StatType.DiceEye);
    }

    protected override void UseSkillLocal()
    {
        base.UseSkillLocal();

        for (int i = 0; i < loopCount; ++i)
        {
            float pathRate = GetRandomPathRate();
            UseSkillInPath(pathRate);
            SendSkillInPath(pathRate);
        }
    }

    public override void UseSkillInPath(float InPathRate)
    {
        if (projectileID != 0)
            FEffectManager.Instance.AddProjectile(projectileID, owner, owner.WorldPosition, InPathRate);
        else if (summonEnemyID != 0 && FGlobal.localPlayer.IsHost)
            FObjectManager.Instance.CreateEnemy(summonEnemyID, owner.SummonOwner);
    }
}



