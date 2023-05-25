using System;
using System.Collections.Generic;
using UnityEngine;

public class FSkillAreaController : FControllerBase, FObjectStateObserver
{
    List<FObjectBase> enemyList = new List<FObjectBase>();

    public int PlayerIndex { get; set; }
    public FObjectBase FrontEnemy { get { return enemyList.Count != 0 ? enemyList[0] : null; } }

    public FSkillAreaController(FObjectBase InObject) : base(InObject)
    {
        FSkillArea[] skillAreaList = GameObject.FindObjectsOfType<FSkillArea>();
        foreach (FSkillArea skillArea in skillAreaList)
        {
            if (skillArea.index == Owner.UserIndex)
            {
                skillArea.SkillAreaController = this;
                break;
            }
        }
    }

    public override void Tick(float InDeltaTime)
    {
        SortEnemy();
    }

    public void OnEnterArea(FObjectBase inObject)
    {
        inObject.AddObserver(this);
        enemyList.Add(inObject);
    }

    public void OnDestroyObject(FObjectBase InObject)
    {
        enemyList.Remove(InObject);
    }

    public FObjectBase FindNotHaveAbnormality(int InAbnormalityID)
    {
        foreach (FObjectBase enemy in enemyList)
        {
            FAbnormalityController abnormalityController = enemy.FindController<FAbnormalityController>();
            if (abnormalityController == null)
                continue;

            if (abnormalityController.HasAbnormality(InAbnormalityID) == false)
                return enemy;
        }
        return null;
    }

    public List<FObjectBase> GetEnemyList(FObjectBase InStartObject, int InCount)
    {
        List<FObjectBase> retList = new List<FObjectBase>();

        int startIndex = enemyList.FindIndex(i => i.ObjectID == InStartObject.ObjectID);
        if (startIndex != -1)
        {
            for (int i = startIndex + 1; i < Math.Min(enemyList.Count, startIndex + InCount + 1); ++i)
            {
                retList.Add(enemyList[i]);
            }
        }

        return retList;
    }

    public delegate void ForeachEnemyDelegate(FObjectBase InObject);
    public void ForeachEnemy(ForeachEnemyDelegate InFunc)
    {
        foreach (FObjectBase enemy in enemyList)
        {
            InFunc(enemy);
        }
    }

    private void SortEnemy()
    {
        for (int i = 0; i < enemyList.Count - 1; ++i)
        {
            FObjectBase A = enemyList[i];
            FObjectBase B = enemyList[i + 1];

            float remainDistanceA = A.FindController<FMoveController>().RemainDistance;
            float remainDistanceB = B.FindController<FMoveController>().RemainDistance;

            if (remainDistanceB < remainDistanceA)
            {
                FObjectBase temp = enemyList[i];
                enemyList[i] = enemyList[i + 1];
                enemyList[i + 1] = temp;

                B.SortingOrder = A.SortingOrder + 1;
            }
        }
    }
}
