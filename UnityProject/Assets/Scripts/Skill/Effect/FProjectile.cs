using FEnum;
using Packet;
using UnityEngine;

public class FProjectile : MonoBehaviour, FObjectStateObserver
{
    private int speed;
    private int effectID;
    private int abnormalityID;
    private int damage;
    private Vector2 targetPosition;
    private FObjectBase owner;
    private FObjectBase target;

    public int InstanceID { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }

    public void Initialize(int InProjectileID, FObjectBase InOwner, Vector2 InTargetPosition)
    {
        Initialize(InProjectileID, InOwner);

        targetPosition = InTargetPosition;
    }

    public void Initialize(int InProjectileID, FObjectBase InOwner, FObjectBase InTarget)
    {
        Initialize(InProjectileID, InOwner);

        target = InTarget;
        target.AddObserver(this);
    }

    private void Initialize(int InProjectileID, FObjectBase InOwner)
    {
        owner = InOwner;

        FProjectileData projectileData = FEffectDataManager.Instance.FindProjectileData(InProjectileID);
        if (projectileData != null)
        {
            speed = projectileData.speed;
            effectID = projectileData.effectID;
            abnormalityID = projectileData.abnormalityID;

            damage = (int)FGlobal.CalcEffectValue(InOwner, projectileData.damage, projectileData.damagePerLevel, projectileData.damagePerBattleLevel);
        }
    }

    public void OnDestroyObject()
    {
        Remove();
    }

    public void Tick(float InDeltaTile)
    {
        Vector2 targetPos = target == null ? targetPosition : target.WorldPosition;

        float moveDelta = speed * InDeltaTile;
        WorldPosition = Vector2.MoveTowards(WorldPosition, targetPos, moveDelta);
        if((Vector2)WorldPosition == targetPos)
        {
            if (damage != 0)
                DamageToTarget(target, damage);

            if (effectID != 0)
                ActiveEffect();

            if (abnormalityID != 0)
                ActiveAbnormality();

            Remove();
        }
    }

    private void DamageToTarget(FObjectBase InTarget, int InDamage)
    {
        if (InTarget == null)
            return;

        FStatController statController = owner.FindController<FStatController>();
        if (statController == null)
            return;

        FStatController targetStatController = InTarget.FindController<FStatController>();
        if (targetStatController == null)
            return;

        bool critical = statController.IsCritical();
        int damage = (int)(critical ? InDamage * statController.GetStat(StatType.CriticalDamage) : InDamage);

        FCombatTextManager.Instance.AddText(critical ? CombatTextType.Critical : CombatTextType.Normal, damage, InTarget);
        targetStatController.OnDamage(InDamage);

        P2P_DAMAGE pkt = new P2P_DAMAGE();
        pkt.objectId = InTarget.ObjectID;
        pkt.damage = damage;
        pkt.critical = critical;

        FServerManager.Instance.SendMessage(pkt);
    }

    private void ActiveEffect()
    {
        if(target)
            FEffectManager.Instance.AddEffect(effectID, owner, target);
        else
            FEffectManager.Instance.AddEffect(effectID, owner, targetPosition);
    }

    private void ActiveAbnormality()
    {
        if (target == null)
            return;

        FAbnormalityController abnormalityController = target.FindController<FAbnormalityController>();
        if (abnormalityController == null)
            return;

        abnormalityController.AddAbnormality(owner, abnormalityID);
    }

    private void Remove()
    {
        if (target != null)
        {
            target.RemoveObserver(this);
        }

        FEffectManager.Instance.RemoveProjectile(InstanceID);
    }
}
