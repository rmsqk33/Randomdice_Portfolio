using FEnum;
using Packet;
using UnityEngine;

public class FSkillBase : FStatObserver
{
    private int checkAbnormalityID;
    private bool toggle;
    private float originInterval;
    private FTimer intervalTimer = new FTimer();

    protected FObjectBase owner;
    protected int skillID;
    protected int projectileID;
    protected int abnormalityID;
    protected SkillTargetType targetType;
    protected FObjectBase target;

    protected float OriginInterval
    {
        set
        {
            FStatController statController = owner.FindController<FStatController>();
            if (statController != null)
            {
                originInterval = value;
                intervalTimer.Interval = originInterval / statController.GetStat(StatType.AttackSpeed);
            }
        }
    }

    public FObjectBase Target { set { target = value; } }
    public bool Toggle { set { toggle = value; } }

    public FSkillBase(FObjectBase InOwner, FSkillData InSkillData)
    {
        FStatController statController = InOwner.FindController<FStatController>();
        if (statController == null)
            return;

        statController.AddObserver(this);

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

        OriginInterval = InSkillData.interval;
        intervalTimer.Start();

        Initialize(InSkillData);
    }

    protected virtual void Initialize(FSkillData InSkillData) { }
    public virtual void UseSkill()
    {
        if (owner.IsOwnLocalPlayer() == false)
            return;

        FObjectBase newTarget = GetTarget();
        if (target == newTarget)
            return;

        target = newTarget;
        if (target != null)
            SendOnSkill(target);
        else
            SendOffSkill();
    }

    public virtual void UseSkillInPath(float InPathRate)
    {
        if (owner.IsOwnLocalPlayer() == false)
            return;

        SendSkillInPath(InPathRate);
    }

    public virtual void Tick(float InDelta)
    {
        if (toggle == false)
            return;

        if (intervalTimer.IsElapsedCheckTime())
        {
            if (targetType == SkillTargetType.RandomPath)
                UseSkillInPath(Random.value);
            else
                UseSkill();

            intervalTimer.Restart();
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

    protected void SendSkillInPath(float InPathRate)
    {
        P2P_USE_SKILL_IN_PATH pkt = new P2P_USE_SKILL_IN_PATH();
        pkt.objectId = owner.ObjectID;
        pkt.skillId = skillID;
        pkt.pathRate = InPathRate;

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

    public void OnStatChanged(StatType InType, float InValue)
    {
        if (InType != StatType.AttackSpeed)
            return;

        intervalTimer.Interval = originInterval / InValue;
    }
}
