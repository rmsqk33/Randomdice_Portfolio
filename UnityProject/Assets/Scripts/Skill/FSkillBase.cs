using FEnum;
using Packet;
using UnityEngine;
using UnityEngine.UIElements;

public class FSkillBase : FStatObserver
{
    private int effectID;
    private int checkAbnormalityID;
    private bool toggle;
    private float originInterval;
    private FTimer intervalTimer = new FTimer();

    private float pathMinRate;
    private float pathMaxRate;

    protected FObjectBase owner;
    protected int skillID;
    protected int projectileID;
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
        effectID = InSkillData.effectID;
        targetType = InSkillData.targetType;
        toggle = owner.IsOwnLocalPlayer();

        projectileID = InSkillData.projectileID;

        pathMinRate = InSkillData.pathMinRate;
        pathMaxRate = InSkillData.pathMaxRate;
        checkAbnormalityID = InSkillData.checkAbnormalityID;

        OriginInterval = InSkillData.interval;
        intervalTimer.Start();

        Initialize(InSkillData);
    }

    protected virtual void Initialize(FSkillData InSkillData) { }
    public virtual void UseSkill()
    {
        if (owner.IsOwnLocalPlayer() == false)
            return;

        if (effectID != 0)
            FEffectManager.Instance.AddEffect(effectID, owner, owner.WorldPosition);

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

        if (effectID != 0)
            FEffectManager.Instance.AddEffect(effectID, owner, owner.WorldPosition);

        SendSkillInPath(InPathRate);
    }

    public virtual void Tick(float InDelta)
    {
        if (toggle == false)
            return;

        if (intervalTimer.IsElapsedCheckTime())
        {
            if (targetType == SkillTargetType.Path)
                UseSkillInPath(Random.Range(pathMinRate, pathMaxRate));
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
        FObjectBase newTarget = null;
        switch (targetType)
        {
            case SkillTargetType.Front:
                if (owner.SummonOwner == null)
                    return null;

                FSkillAreaController skillAreaController = owner.SummonOwner.FindController<FSkillAreaController>();
                if (skillAreaController == null)
                    return null;

                if (checkAbnormalityID != 0)
                    newTarget = skillAreaController.FindNotHaveAbnormality(checkAbnormalityID);
                 
                if (newTarget == null)
                    newTarget = skillAreaController.FrontEnemy;

                break;

            case SkillTargetType.Myself: 
                newTarget = owner; 
                break;

            case SkillTargetType.Dice:
                FBattleDiceController battleDiceController = FGlobal.localPlayer.FindController<FBattleDiceController>();
                if (battleDiceController == null)
                    return null;

                newTarget = battleDiceController.FindNotHaveAbnormality(checkAbnormalityID);
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
