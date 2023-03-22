using UnityEngine;

public class FMoveController : FControllerBase
{
    FPathBase movePath;
    
    public float Speed { get; set; }
    public float MoveDistance { get; private set; }

    public FMoveController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public override void Tick(float InDeltaTime)
    {
        if (movePath == null)
            return;

        float moveDelta = Speed * InDeltaTime;
        Owner.WorldPosition = Vector2.MoveTowards(Owner.WorldPosition, movePath.WorldPosition, moveDelta);
        MoveDistance += moveDelta;

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

        MoveDistance = 0;
    }
}
