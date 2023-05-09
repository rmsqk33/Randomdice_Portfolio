using System.Collections.Generic;
using UnityEngine;
using FEnum;

public class FCollisionObject : FObjectBase
{
    int abnoramlityID;
    FTimer durationTimer;

    List<int> crashedObjectIDList = new List<int>();

    public void Initialize(FCollisionData InData, FObjectBase InOwner)
    {
        FStatController ownerStatController = InOwner.FindController<FStatController>();
        if (ownerStatController == null)
            return;

        ContentID = InOwner.ContentID;
        SummonOwner = InOwner;
        abnoramlityID = InData.abnormalityID;
        durationTimer = new FTimer(InData.duration);
        durationTimer.Start();

        transform.localScale = new Vector2(InData.size, InData.size);

        AddController<FStatController>();

        FStatController statController = FindController<FStatController>();
        statController.SetStat(StatType.Level, ownerStatController.GetStat(StatType.Level));
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

    private void Update()
    {
        if(durationTimer.IsElapsedCheckTime())
        {
            FObjectManager.Instance.RemoveObjectAndSendP2P(ObjectID);
        }
    }

    void OnTriggerEnter2D(Collider2D o)
    {
        FObjectBase crashedObject = o.gameObject.GetComponent<FObjectBase>();
        if (crashedObject == null)
            return;

        FAbnormalityController abnormaltiyController = crashedObject.FindController<FAbnormalityController>();
        if (abnormaltiyController == null)
            return;

        abnormaltiyController.AddAbnormality(this, abnoramlityID);
        crashedObjectIDList.Add(crashedObject.ObjectID);
    }

    void OnTriggerExit2D(Collider2D o)
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
