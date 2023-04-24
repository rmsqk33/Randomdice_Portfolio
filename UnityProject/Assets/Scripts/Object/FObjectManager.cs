using Packet;
using FEnum;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FObjectManager : FSceneLoadedSingleton<FObjectManager>
{
    [SerializeField]
    List<FStartPoint> startPointList;

    Dictionary<int, FObjectBase> objectMap = new Dictionary<int, FObjectBase>();
    List<FObjectBase> sortedEnemyList = new List<FObjectBase>();
    List<FObjectBase> removeObjectList = new List<FObjectBase>();

    int enemySpawnCount = 0;
    int diceInstanceID = 0;

    public FObjectBase FrontEnemy { get; private set; }
    public int EnemyCount { get { return sortedEnemyList.Count; } }
    public int Seed { set { diceInstanceID = (value + 1) * 100000000; } }

    private void Start()
    {
        if (FGlobal.localPlayer.IsHost == false)
            startPointList.Reverse();
    }

    public int CreateLocalPlayerBattleDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        InDiceID = 4;

        FObjectBase dice = FBattleDiceCreator.Instance.CreateLocalPlayerDice(InDiceID, InEyeCount, InSlotIndex);
        AddObject(diceInstanceID++, dice);

        P2P_SPAWN_DICE pkt = new P2P_SPAWN_DICE();
        pkt.objectId = dice.ObjectID;
        pkt.diceId = InDiceID;
        pkt.eyeCount = InEyeCount;
        pkt.index = InSlotIndex;
        FServerManager.Instance.SendMessage(pkt);

        return dice.ObjectID;
    }

    public int CreateRemotePlayerBattleDice(int InObjectID, int InDiceID, int InEyeCount, int InSlotIndex)
    {
        FObjectBase dice = FBattleDiceCreator.Instance.CreateRemotePlayerDice(InDiceID, InEyeCount, InSlotIndex);
        AddObject(InObjectID, dice);
        return dice.ObjectID;
    }

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
            }

            FStatController statController = newEnemy.FindController<FStatController>();
            if (statController != null)
            {
                statController.SetStat(StatType.HP, enemyData.hp + enemyData.hpIncreaseBySpawnCount * (enemySpawnCount / 2));
                statController.SetStat(StatType.SP, enemyData.sp);
                statController.SetStat(StatType.MoveSpeed, enemyData.moveSpeed);
            }

            AddObject(enemySpawnCount++, newEnemy);
            sortedEnemyList.Add(newEnemy);
        }

        if (FGlobal.localPlayer.IsHost)
        {
            P2P_SPAWN_ENEMY pkt = new P2P_SPAWN_ENEMY();
            pkt.enemyId = InID;
            FServerManager.Instance.SendMessage(pkt);
        }
    }

    private void AddObject(int InObjectID, FObjectBase InDice)
    {
        InDice.ObjectID = InObjectID;
        objectMap.Add(InObjectID, InDice);
    }

    public void RemoveObject(int InID)
    {
        if (objectMap.ContainsKey(InID))
        {
            removeObjectList.Add(objectMap[InID]);
        }
    }

    public void RemoveObjectAndSendP2P(int InID)
    {
        RemoveObject(InID);

        P2P_DESPAWN_OBJECT pkt = new P2P_DESPAWN_OBJECT();
        pkt.objectId = InID;
        FServerManager.Instance.SendMessage(pkt);
    }

    public FObjectBase FindObject(int InID)
    {
        if (objectMap.ContainsKey(InID))
        {
            return objectMap[InID];
        }
        return null;
    }

    public delegate void ForeachObjectDelegate(FObjectBase InObject);
    public void ForeachObject(ForeachObjectDelegate InFunc)
    {
        foreach (var pair in objectMap)
        {
            InFunc(pair.Value);
        }
    }

    public delegate void ForeachSortedEnemyDelegate(FObjectBase InObject);
    public void ForeachSortedEnemy(int InCount, ForeachSortedEnemyDelegate InFunc)
    {
        int i = 0;
        foreach (FObjectBase obj in sortedEnemyList)
        {
            if(obj.FindController<FIFFController>().IsEnumy(IFFType.LocalPlayer))
            {
                InFunc(obj);

                ++i;
                if (i == InCount)
                    break;
            }
        }
    }

    public void ForeachSortedEnemy(ForeachSortedEnemyDelegate InFunc)
    {
        foreach (FObjectBase obj in sortedEnemyList)
        {
            if (obj.FindController<FIFFController>().IsEnumy(IFFType.LocalPlayer))
            {
                InFunc(obj);
            }
        }
    }

    private void Update()
    {
        SortEnemyByRemainDistance();
        RemoveObjectAfterTick();
    }

    private void SortEnemyByRemainDistance()
    {
        bool dirty = false;
        for(int i = 0; i < sortedEnemyList.Count - 1; ++i)
        {
            FObjectBase A = sortedEnemyList[i];
            FObjectBase B = sortedEnemyList[i + 1];

            float remainDistanceA = A.FindController<FMoveController>().RemainDistance;
            float remainDistanceB = B.FindController<FMoveController>().RemainDistance;

            if(remainDistanceB < remainDistanceA)
            {
                FObjectBase temp = sortedEnemyList[i];
                sortedEnemyList[i] = sortedEnemyList[i + 1];
                sortedEnemyList[i + 1] = temp;

                A.SortingOrder = i + 1;
                B.SortingOrder = i;

                dirty = true;
            }
        }

        if(dirty || FrontEnemy == null)
        {
            foreach(FObjectBase obj in sortedEnemyList)
            {
                if(obj.FindController<FIFFController>().IsEnumy(IFFType.LocalPlayer))
                {
                    FrontEnemy = obj;
                    break;
                }
            }
        }
    }

    private void RemoveObjectAfterTick()
    {
        foreach(FObjectBase InObject in removeObjectList)
        {
            sortedEnemyList.Remove(InObject);

            InObject.Release();
            GameObject.Destroy(InObject.gameObject);
            objectMap.Remove(InObject.ObjectID);
        }
        removeObjectList.Clear();
    }
}
