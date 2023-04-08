using FEnum;
using Packet;

public class FSkillBase
{
    protected FObjectBase owner;
    protected int skillID;
    protected int projectileID;
    protected int abnormalityID;

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
        
        interval = InSkillData.interval;

        Initialize(InSkillData);

        if (interval == 0)
            UseSkillInner();
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
            UseSkillInner();
            elapsedTime = 0;
        }
    }

    private void UseSkillInner()
    {
        if (owner.IsOwnLocalPlayer())
        {
            FObjectBase newTarget = GetTarget();
            if (target != newTarget)
            {
                target = newTarget;
                if (target != null)
                    SendOnSkill(target.ObjectID);
                else
                    SendOffSkill();
            }
        }

        if (target == null)
            return;

        UseSkill();
    }

    protected void SendOnSkill(int InTargetID)
    {
        P2P_ON_SKILL pkt = new P2P_ON_SKILL();
        pkt.objectId = owner.ObjectID;
        pkt.skillId = skillID;
        pkt.targetId = InTargetID;

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
        switch (targetType)
        {
            case SkillTargetType.Front: return FObjectManager.Instance.FrontEnemy;
            case SkillTargetType.Myself: return owner;
        }

        return null;
    }
}
