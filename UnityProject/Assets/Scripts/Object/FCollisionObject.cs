using System.Collections.Generic;
using UnityEngine;

public class FCollisionObject : FObjectBase
{
    FObjectBase owner;
    int abnoramlityID;
    float duration;
    FTimer timer;

    List<int> crashedObjectIDList = new List<int>();

    public void Initialize(int InCollisionObjectID, FObjectBase InOwner)
    {
        owner = InOwner;
    }

    public override void Release()
    {
        base.Release();

        foreach(int id in crashedObjectIDList)
        {
            FObjectBase objectBase = FObjectManager.Instance.FindObject(id);
            if (objectBase == null)
                continue;

            FAbnormalityController abnormalityController = objectBase.FindController<FAbnormalityController>();
            if (abnormalityController == null)
                continue;

            abnormalityController.RemoveAbnormality(abnoramlityID);
        }
    }

    void OnCollisionEnter(Collision o)
    {
        FObjectBase crashedObject = o.gameObject.GetComponent<FObjectBase>();
        if (crashedObject == null)
            return;

        FAbnormalityController abnormaltiyController = crashedObject.FindController<FAbnormalityController>();
        if (abnormaltiyController == null)
            return;

        abnormaltiyController.AddAbnormality(owner, abnoramlityID);
        crashedObjectIDList.Add(crashedObject.ObjectID);
    }

    void OnCollisionExit(Collision o)
    {
        FObjectBase crashedObject = o.gameObject.GetComponent<FObjectBase>();
        if (crashedObject == null)
            return;

        FAbnormalityController abnormaltiyController = crashedObject.FindController<FAbnormalityController>();
        if (abnormaltiyController == null)
            return;

        abnormaltiyController.RemoveAbnormality(abnoramlityID);
        crashedObjectIDList.Remove(crashedObject.ObjectID);
    }
}
