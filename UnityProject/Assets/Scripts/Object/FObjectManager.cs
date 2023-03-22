using System.Collections.Generic;
using UnityEngine;

public class FObjectManager : FSingleton<FObjectManager>
{
    [SerializeField]
    List<FStartPoint> startPointList;

    Dictionary<int, FEnemy> enemyMap = new Dictionary<int, FEnemy>();
    int enemySpawnCount;
    
    public FEnemy FrontEnemy { get; private set; }
    public int EnemyCount { get { return enemyMap.Count; } }

    public void CreateEnemy(int InID)
    {
        FEnemyData enemyData = FBattleDataManager.Instance.FindEnemyData(InID);
        if (enemyData == null)
            return;

        foreach (FStartPoint startPoint in startPointList)
        {
            FEnemy newEnemy = Instantiate<FEnemy>(Resources.Load<FEnemy>(enemyData.prefabPath), transform);
            newEnemy.ContentID = InID;

            FIFFController iffController = newEnemy.FindController<FIFFController>();
            if (iffController != null)
            {
                iffController.IFFType = startPoint.IffType;
            }

            FMoveController moveController = newEnemy.FindController<FMoveController>();
            if (moveController != null)
            {
                moveController.SetStartPoint(startPoint);
                moveController.Speed = enemyData.moveSpeed;
            }

            FStatController statController = newEnemy.FindController<FStatController>();
            if (statController != null)
            {
                statController.HP = enemyData.hp + enemyData.hpIncreaseBySpawnCount * (enemySpawnCount / 2);
                statController.SP = enemyData.sp;
            }

            newEnemy.ObjectID = enemySpawnCount++;

            enemyMap.Add(newEnemy.ObjectID, newEnemy);
        }
    }

    public void RemoveEnemey(int InID)
    {
        if (enemyMap.ContainsKey(InID))
        {
            if (FrontEnemy.ObjectID == InID)
                FrontEnemy = null;

            enemyMap[InID].Release();
            GameObject.Destroy(enemyMap[InID].gameObject);
            enemyMap.Remove(InID);
        }
    }

    public delegate void ForeachEnemyDelegate(FObjectBase InObject);
    public void ForeachEnemy(ForeachEnemyDelegate InFunc)
    { 
        foreach(var pair in enemyMap)
        {
            InFunc(pair.Value);
        }
    }

    private void Update()
    {
        UpdateFrontEnemy();
    }

    private void UpdateFrontEnemy()
    {
        if (enemyMap.Count == 0)
            return;

        foreach (var pair in enemyMap)
        {
            FEnemy enemy = pair.Value;

            FIFFController enemyIFFController = enemy.FindController<FIFFController>();
            FIFFController localPlayerIFFController = FGlobal.localPlayer.FindController<FIFFController>();
            if (localPlayerIFFController.IsEnumy(enemyIFFController.IFFType) == false)
                continue;

            if (FrontEnemy == null)
            {
                FrontEnemy = enemy;
                continue;
            }

            FMoveController moveController = enemy.FindController<FMoveController>();
            FMoveController frontEnemyMoveController = FrontEnemy.FindController<FMoveController>();
            if (moveController.RemainDistance < frontEnemyMoveController.RemainDistance)
            {
                FrontEnemy = enemy;
            }
        }
    }
}
