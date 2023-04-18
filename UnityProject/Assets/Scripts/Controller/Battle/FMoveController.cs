using FEnum;
using UnityEngine;
using UnityEngine.Rendering;

public class FMoveController : FControllerBase, FStatObserver
{
    FPathBase movePath;
    float totalDistance;
    float moveDistance;
    float speed;

    public float RemainDistance { get { return totalDistance - moveDistance; } }

    public FMoveController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public override void Initialize()
    {
        FStatController statController = FindController<FStatController>();
        if(statController != null)
        {
            speed = statController.GetStat(StatType.MoveSpeed);
            statController.AddObserver(this);
        }
    }

    public override void Tick(float InDeltaTime)
    {
        if (movePath == null)
            return;

        float moveDelta = speed * InDeltaTime;
        Owner.WorldPosition = Vector2.MoveTowards(Owner.WorldPosition, movePath.WorldPosition, moveDelta);
        moveDistance += moveDelta;

        if (Owner.WorldPosition == movePath.WorldPosition)
        {
            movePath.OnPass(Owner);
            movePath = movePath.NextPath;
        }
    }

    public void SetStartPoint(FPathBase InPoint)
    {
        Owner.WorldPosition = InPoint.WorldPosition;
        movePath = InPoint.NextPath;
        totalDistance = 0;

        FPathBase point = InPoint;
        while (point.NextPath != null)
        {
            totalDistance += Vector2.Distance(point.NextPath.WorldPosition, point.WorldPosition);
            point = point.NextPath;
        }
    }

    public void OnStatChanged(StatType InType, float InValue)
    {
        if (InType != StatType.MoveSpeed)
            return;

        speed = InValue;
    }
}
