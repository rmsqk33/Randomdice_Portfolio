using FEnum;
using Packet;
using UnityEngine;

public class FSkillBase : FStatObserver
{
    private int effectID;
    private int checkAbnormalityID;
    private float originInterval;
    private FTimer intervalTimer = new FTimer();

    protected float pathMinRate;
    protected float pathMaxRate;

    protected FObjectBase owner;
    protected int skillID;
    protected int projectileID;
    protected SkillTargetType targetType;
    protected FObjectBase target;
    protected int loopCount;

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
        loopCount = InSkillData.loopCount;

        projectileID = InSkillData.projectileID;

        pathMinRate = InSkillData.pathMinRate;
        pathMaxRate = InSkillData.pathMaxRate;
        checkAbnormalityID = InSkillData.checkAbnormalityID;

        OriginInterval = InSkillData.interval;
        intervalTimer.Start();

        Initialize(InSkillData);
    }

    protected virtual void Initialize(FSkillData InSkillData) { }
    public virtual void UseSkillInPath(float InPathRate) { }
    protected virtual void UseSkillLocal()
    {
        if (effectID != 0)
            FEffectManager.Instance.AddEffect(effectID, owner, owner.WorldPosition);
    }

    public virtual void UseSkillRemote()
    {
        if (effectID != 0)
            FEffectManager.Instance.AddEffect(effectID, owner, owner.WorldPosition);
    }

    public virtual void Tick(float InDelta)
    {
        if (intervalTimer.IsElapsedCheckTime())
        {
            if(owner.IsOwnLocalPlayer())
                UseSkillLocal();
            else
                UseSkillRemote();

            intervalTimer.Restart();
        }
    }

    protected void SendUseSkill(FObjectBase InTarget = null)
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

            case SkillTargetType.DiceForEnemy:
                FBattleDiceController battleDiceController = FGlobal.localPlayer.FindController<FBattleDiceController>();
                if (battleDiceController == null)
                    return null;

                newTarget = battleDiceController.FindNotHaveAbnormality(checkAbnormalityID);
                break;
        }

        return newTarget;
    }

    protected float GetRandomPathRate()
    {
        return Random.Range(pathMinRate, pathMaxRate);
    }

    public void OnStatChanged(StatType InType, float InValue)
    {
        if (InType != StatType.AttackSpeed)
            return;

        intervalTimer.Interval = originInterval / InValue;
    }

}
