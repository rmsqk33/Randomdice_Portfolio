using Packet;

public class FSkillBase
{
    protected FObjectBase owner;
    protected int skillID;

    protected FObjectBase target;
    protected bool toggle;

    public FObjectBase Target { set { target = value; } }
    public bool Toggle { set { toggle = value; } }

    public FSkillBase(FObjectBase InOwner, FSkillData InSkillData)
    {
        owner = InOwner;
        skillID = InSkillData.id;
        toggle = owner.IsOwnLocalPlayer();

        Initialize(InSkillData);
    }

    protected virtual void Initialize(FSkillData InSkillData) { }
    public virtual void Tick(float InDelta) { }
    public virtual void UseSkill() { }

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
}
