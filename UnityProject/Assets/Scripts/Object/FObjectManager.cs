using Packet;
using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FObjectManager : FSceneLoadedSingleton<FObjectManager>
{
    [SerializeField]
    List<FStartPoint> startPointList;

    Dictionary<int, FObjectBase> objectMap = new Dictionary<int, FObjectBase>();
    List<FObjectBase> sortedEnemyList = new List<FObjectBase>();
    List<FObjectBase> removeObjectList = new List<FObjectBase>();

    int enemySpawnCount = 0;
    int instanceID = 0;

    public FObjectBase FrontEnemy { get; private set; }
    public int EnemyCount { get { return sortedEnemyList.Count; } }

    private void Start()
    {
        if (FGlobal.localPlayer.IsHost == false)
            startPointList.Reverse();
    }

    public int CreateLocalPlayerBattleDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        FObjectBase dice = FBattleDiceCreator.Instance.CreateLocalPlayerDice(InDiceID, InEyeCount, InSlotIndex);
        AddObject(instanceID++, dice);

        P2P_SPAWN_REMOTE_DICE pkt = new P2P_SPAWN_REMOTE_DICE();
        pkt.objectId = dice.ObjectID;
        pkt.diceId = InDiceID;
        pkt.eyeCount = InEyeCount;
        pkt.index = InSlotIndex;
        FServerManager.Instance.SendMessage(pkt);

        return dice.ObjectID;
    }

    public void CreateLocalPlayerBattleDice(int InInstanceID, int InDiceID, int InEyeCount, int InSlotIndex)
    {
        FObjectBase dice = FBattleDiceCreator.Instance.CreateLocalPlayerDice(InDiceID, InEyeCount, InSlotIndex);
        AddObject(InInstanceID, dice);
    }

    public void CreateRemotePlayerBattleDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        FObjectBase dice = FBattleDiceCreator.Instance.CreateRemotePlayerDice(InDiceID, InEyeCount, InSlotIndex);
        AddObject(instanceID++, dice);

        P2P_S_REQUEST_SPAWN_DICE pkt = new P2P_S_REQUEST_SPAWN_DICE();
        pkt.objectId = dice.ObjectID;
        pkt.diceId = InDiceID;
        pkt.eyeCount = InEyeCount;
        pkt.index = InSlotIndex;

        FServerManager.Instance.SendMessage(pkt);
    }

    public void CreateRemotePlayerBattleDice(int InObjectID, int InDiceID, int InEyeCount, int InSlotIndex)
    {
        FObjectBase dice = FBattleDiceCreator.Instance.CreateRemotePlayerDice(InDiceID, InEyeCount, InSlotIndex);
        AddObject(InObjectID, dice);
    }

    public void CreateEnemy(int InID)
    {
        FEnemyData enemyData = FObjectDataManager.Instance.FindEnemyData(InID);
        if (enemyData == null)
            return;

        for (int i = 0; i < startPointList.Count; ++i)
        {
            if (FGlobal.localPlayer.IsHost)
            {
                P2P_SPAWN_ENEMY pkt = new P2P_SPAWN_ENEMY();
                pkt.instanceId = instanceID;
                pkt.enemyId = InID;
                pkt.spawnPointIndex = i;
                FServerManager.Instance.SendMessage(pkt);
            }

            CreateEnemy(instanceID++, InID, i);
        }
    }

    public void CreateEnemy(int InInstanceID, int InID, int InSpawnPointIndex)
    {
        if (InSpawnPointIndex < 0 || startPointList.Count <= InSpawnPointIndex)
            return;

        FEnemyData enemyData = FObjectDataManager.Instance.FindEnemyData(InID);
        if (enemyData == null)
            return;

        FEnemy newEnemy = Instantiate<FEnemy>(Resources.Load<FEnemy>(enemyData.prefabPath), transform);
        newEnemy.Initialize(enemyData, startPointList[InSpawnPointIndex], enemySpawnCount++ / 2);
        newEnemy.SortingOrder = -InInstanceID;

        sortedEnemyList.Add(newEnemy);
        AddObject(InInstanceID, newEnemy);
    }

    public void CreateCollisionObject(int InID, FObjectBase InOwner, Vector2 InPosition)
    {
        FCollisionData collisionData = FObjectDataManager.Instance.FindCollisionData(InID);
        if (collisionData == null)
            return;

        FCollisionObject collision = Instantiate<FCollisionObject>(Resources.Load<FCollisionObject>(collisionData.prefab), transform);
        collision.WorldPosition = InPosition;

    }

    private void AddObject(int InObjectID, FObjectBase InObject)
    {
        InObject.ObjectID = InObjectID;
        objectMap.Add(InObjectID, InObject);
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

    public List<FObjectBase> GetSortedEnemyList(FObjectBase InStartObject, int InCount)
    {
        List<FObjectBase> retList = new List<FObjectBase>();

        int startIndex = sortedEnemyList.FindIndex(i => i.ObjectID == InStartObject.ObjectID);
        if (startIndex != -1)
        {
            for (int i = startIndex + 1; i < sortedEnemyList.Count; ++i)
            {
                FObjectBase enemy = sortedEnemyList[i];
                if (enemy.FindController<FIFFController>().IsEnumy(IFFType.LocalPlayer))
                {
                    retList.Add(enemy);

                    if (retList.Count == InCount)
                        break;
                }
            }
        }

        return retList;
    }

    public void DamageToTarget(FObjectBase InAttacker, FObjectBase InTarget, int InDamage)
    {
        if (InAttacker.IsOwnLocalPlayer() == false)
            return;

        FStatController statController = InAttacker.FindController<FStatController>();
        if (statController == null)
            return;

        DamageToTarget(InTarget, InDamage, statController.GetStat(StatType.CriticalChance), statController.GetStat(StatType.CriticalDamage));
    }

    public void DamageToTarget(FObjectBase InTarget, int InDamage, float InCriticalChance, float InCriticalDamage)
    {
        FStatController targetStatController = InTarget.FindController<FStatController>();
        if (targetStatController == null)
            return;

        bool critical = Random.value <= InCriticalChance;
        int damage = (int)(critical ? InDamage * InCriticalDamage : InDamage);

        FCombatTextManager.Instance.AddText(critical ? CombatTextType.Critical : CombatTextType.Normal, damage, InTarget);
        targetStatController.OnDamage(InDamage);

        P2P_DAMAGE pkt = new P2P_DAMAGE();
        pkt.objectId = InTarget.ObjectID;
        pkt.damage = damage;
        pkt.critical = critical;

        FServerManager.Instance.SendMessage(pkt);
    }

    private void Update()
    {
        SortEnemyByRemainDistance();
        RemoveObjectAfterTick();
    }

    private void SortEnemyByRemainDistance()
    {
        bool dirty = false;
        for (int i = 0; i < sortedEnemyList.Count - 1; ++i)
        {
            FObjectBase A = sortedEnemyList[i];
            FObjectBase B = sortedEnemyList[i + 1];

            float remainDistanceA = A.FindController<FMoveController>().RemainDistance;
            float remainDistanceB = B.FindController<FMoveController>().RemainDistance;

            if (remainDistanceB < remainDistanceA)
            {
                FObjectBase temp = sortedEnemyList[i];
                sortedEnemyList[i] = sortedEnemyList[i + 1];
                sortedEnemyList[i + 1] = temp;

                B.SortingOrder = A.SortingOrder + 1;

                dirty = true;
            }
        }

        if (dirty || FrontEnemy == null)
        {
            foreach (FObjectBase obj in sortedEnemyList)
            {
                if (obj.FindController<FIFFController>().IsEnumy(IFFType.LocalPlayer))
                {
                    FrontEnemy = obj;
                    break;
                }
            }
        }
    }

    private void RemoveObjectAfterTick()
    {
        foreach (FObjectBase InObject in removeObjectList)
        {
            sortedEnemyList.Remove(InObject);

            InObject.Release();
            GameObject.Destroy(InObject.gameObject);
            objectMap.Remove(InObject.ObjectID);
        }
        removeObjectList.Clear();
    }
}
