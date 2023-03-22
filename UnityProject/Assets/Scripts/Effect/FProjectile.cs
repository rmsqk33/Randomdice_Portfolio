using Unity.VisualScripting;
using UnityEngine;

public class FProjectile : MonoBehaviour, FObjectStateObserver
{
    private int speed;
    private int effectID;
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
            if (effectID != 0)
                ActiveEffect();

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

    private void Remove()
    {
        if (target != null)
        {
            target.RemoveObserver(this);
        }

        FEffectManager.Instance.RemoveProjectile(InstanceID);
    }
}
