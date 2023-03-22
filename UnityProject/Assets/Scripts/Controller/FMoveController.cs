using TMPro;
using UnityEngine;
using UnityEngine.Rendering;

public class FMoveController : FControllerBase
{
    private FPathBase movePath;
    private SortingGroup sortingGroup;
    private float totalDistance;
    private float moveDistance;

    public float Speed { get; set; }
    public float RemainDistance { get { return totalDistance - moveDistance; } }

    public FMoveController(FObjectBase InOwner) : base(InOwner)
    {
        sortingGroup = Owner.GetComponent<SortingGroup>();
    }

    public override void Tick(float InDeltaTime)
    {
        if (movePath == null)
            return;

        float moveDelta = Speed * InDeltaTime;
        Owner.WorldPosition = Vector2.MoveTowards(Owner.WorldPosition, movePath.WorldPosition, moveDelta);
        moveDistance += moveDelta;

        if(sortingGroup != null)
        {
            int sortingOrder = (int)(-RemainDistance);
            if (sortingGroup.sortingOrder != sortingOrder)
            {
                sortingGroup.sortingOrder = sortingOrder;
            }
        }

        if (Owner.WorldPosition == movePath.WorldPosition)
        {
            movePath.OnPass(Owner);
            if (movePath.NextPath != null)
            {
                movePath = movePath.NextPath;
            }
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
}
