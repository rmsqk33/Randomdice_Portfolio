using FEnum;
using Packet;
using UnityEngine;

public class FProjectile : MonoBehaviour, FObjectStateObserver
{
    private static float CURVE_SECTION = 0.1f;
    private static float MIN_SPEED = 100;

    private int speed;
    private int effectID;
    private int abnormalityID;
    private int collisionObjectID;
    private int damage;
    private int distance;
    private ProjectileMoveType moveType;
    private Vector2 targetPosition;
    private FObjectBase owner;
    private FObjectBase target;

    public int InstanceID { get; set; }
    public Vector2 WorldPosition { get { return transform.position; } set { transform.position = value; } }

    public void Initialize(int InProjectileID, FObjectBase InOwner, Vector2 InTargetPosition)
    {
        Initialize(InProjectileID, InOwner);
    
        targetPosition = InTargetPosition;
        if (moveType == ProjectileMoveType.Curve)
            distance = (int)Vector2.Distance(WorldPosition, targetPosition);
    }

    public void Initialize(int InProjectileID, FObjectBase InOwner, FObjectBase InTarget)
    {
        Initialize(InProjectileID, InOwner);

        target = InTarget;
        target.AddObserver(this);
        if (moveType == ProjectileMoveType.Curve)
            distance = (int)Vector2.Distance(WorldPosition, target.WorldPosition);
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
            collisionObjectID = projectileData.collisionObjectID;
            damage = (int)FGlobal.CalcEffectValue(InOwner, projectileData.damage, projectileData.damagePerLevel, projectileData.damagePerBattleLevel);
            moveType = projectileData.moveType;
        }
    }

    public void OnDestroyObject()
    {
        Remove();
    }

    public void Tick(float InDeltaTime)
    {
        Vector2 targetPos = target == null ? targetPosition : target.WorldPosition;

        float moveDelta = speed * InDeltaTime;
        if (moveType == ProjectileMoveType.Curve)
        {
            float remainDistance = Vector2.Distance(targetPos, WorldPosition);
            float section = remainDistance / distance;
            moveDelta = section < CURVE_SECTION ? moveDelta : Mathf.Max(moveDelta * section, MIN_SPEED * InDeltaTime);
        }

        WorldPosition = Vector2.MoveTowards(WorldPosition, targetPos, moveDelta);
        if((Vector2)WorldPosition == targetPos)
        {
            if (damage != 0)
                FObjectManager.Instance.DamageToTarget(owner, target, damage);

            if (effectID != 0)
                ActiveEffect();

            if (abnormalityID != 0)
                ActiveAbnormality();

            if (collisionObjectID != 0)
                ActiveCollisionObject();

            Remove();
        }
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

    private void ActiveCollisionObject()
    {

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
