using Packet;
using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FObjectManager : FSceneLoadedSingleton<FObjectManager>
{
    [SerializeField]
    List<FStartPoint> startPointList;

    Dictionary<int, FObjectBase> objectMap = new Dictionary<int, FObjectBase>();
    Dictionary<int, FObjectBase> enemyMap = new Dictionary<int, FObjectBase>();
    int enemySpawnCount = 0;
    int diceInstanceID = 0;

    public FObjectBase FrontEnemy { get; private set; }
    public int EnemyCount { get { return enemyMap.Count; } }
    public int Seed { set { diceInstanceID = (value + 1) * 100000000; } }

    private void Start()
    {
        if (FGlobal.localPlayer.IsHost == false)
            startPointList.Reverse();
    }

    public int CreateLocalPlayerBattleDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
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
                moveController.Speed = enemyData.moveSpeed;
            }

            FEnemyStatController statController = newEnemy.FindController<FEnemyStatController>();
            if (statController != null)
            {
                statController.HP = enemyData.hp + enemyData.hpIncreaseBySpawnCount * (enemySpawnCount / 2);
                statController.SP = enemyData.sp;
            }

            AddObject(enemySpawnCount++, newEnemy);
            enemyMap.Add(newEnemy.ObjectID, newEnemy);
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
            if (enemyMap.ContainsKey(InID))
            {
                if (FrontEnemy != null && FrontEnemy.ObjectID == InID)
                    FrontEnemy = null;

                enemyMap.Remove(InID);
            }

            objectMap[InID].Release();
            GameObject.Destroy(objectMap[InID].gameObject);
            objectMap.Remove(InID);
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

    private void Update()
    {
        UpdateFrontEnemy();
    }

    private void UpdateFrontEnemy()
    {
        foreach (var pair in enemyMap)
        {
            FObjectBase enemy = pair.Value;

            FIFFController iffController = enemy.FindController<FIFFController>();
            if (iffController.IsEnumy(IFFType.LocalPlayer) == false)
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
