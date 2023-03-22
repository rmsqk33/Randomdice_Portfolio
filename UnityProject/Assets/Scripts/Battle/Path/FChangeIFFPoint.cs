using FEnum;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FChangeIFFPoint : FPathBase
{
    [SerializeField]
    IFFType iffType;

    public override void OnPass(FObjectBase InObject)
    {
        FIFFController iffController = InObject.FindController<FIFFController>();
        if (iffController != null)
        {
            iffController.IFFType = iffType;
        }
    }
}
