using FEnum;
using Packet;

public class FSkillBase
{
    protected FObjectBase owner;
    protected int skillID;
    protected int projectileID;
    protected int abnormalityID;
    protected int checkAbnormalityID;

    protected SkillTargetType targetType;
    protected FObjectBase target;
    protected bool toggle;

    protected float interval;
    protected float elapsedTime;

    public FObjectBase Target { set { target = value; } }
    public bool Toggle { set { toggle = value; } }

    public FSkillBase(FObjectBase InOwner, FSkillData InSkillData)
    {
        owner = InOwner;
        skillID = InSkillData.id;
        targetType = InSkillData.targetType;
        toggle = owner.IsOwnLocalPlayer();

        projectileID = InSkillData.projectileID;
        abnormalityID = InSkillData.abnormalityID;

        if (targetType == SkillTargetType.NoneAbnormalityFront)
        {
            checkAbnormalityID = InSkillData.abnormalityID;
            if (checkAbnormalityID == 0)
            {
                FProjectileData projectileData = FEffectDataManager.Instance.FindProjectileData(InSkillData.projectileID);
                if (projectileData != null)
                {
                    checkAbnormalityID = projectileData.abnormalityID;
                }
            }
        }

        interval = InSkillData.interval;

        Initialize(InSkillData);

        if (interval == 0)
        {
            UseSkill();
        }
    }

    protected virtual void Initialize(FSkillData InSkillData) { }
    public virtual void UseSkill() { }

    public virtual void Tick(float InDelta)
    {
        if (toggle == false)
            return;

        if (interval == 0)
            return;

        elapsedTime += InDelta;

        if (interval <= elapsedTime)
        {
            UseSkill();
            elapsedTime = 0;
        }
    }

    protected void SendOnSkill(FObjectBase InTarget = null)
    {
        P2P_ON_SKILL pkt = new P2P_ON_SKILL();
        pkt.objectId = owner.ObjectID;
        pkt.skillId = skillID;
        pkt.targetId = InTarget != null ? InTarget.ObjectID : 0;

        FServerManager.Instance.SendMessage(pkt);
    }

    protected void SendOffSkill()
    {
        P2P_OFF_SKILL pkt = new P2P_OFF_SKILL();
        pkt.objectId = owner.ObjectID;
        pkt.skillId = skillID;

        FServerManager.Instance.SendMessage(pkt);
    }

    protected FObjectBase GetTarget()
    {
        if (owner.SummonOwner == null)
            return null;

        FSkillAreaController skillAreaController = owner.SummonOwner.FindController<FSkillAreaController>();
        if (skillAreaController == null)
            return null;

        FObjectBase newTarget = null;
        switch (targetType)
        {
            case SkillTargetType.Front:
                newTarget = skillAreaController.FrontEnemy;
                break;
            case SkillTargetType.Myself: newTarget = owner; break;
            case SkillTargetType.NoneAbnormalityFront:
                newTarget = skillAreaController.FindNotHaveAbnormality(checkAbnormalityID);
                if (newTarget == null)
                    newTarget = skillAreaController.FrontEnemy;

                break;
        }

        return newTarget;
    }
}
