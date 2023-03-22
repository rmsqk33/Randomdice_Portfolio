using TMPro;
using UnityEngine;

public class FEnemy : FObjectBase
{
    protected override void Awake()
    {
        base.Awake();

        AddController<FIFFController>();
        AddController<FMoveController>();
        AddController<FStatController>();
    }
}
