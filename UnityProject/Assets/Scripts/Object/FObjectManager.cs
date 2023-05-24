using Packet;
using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FObjectManager : FSingleton<FObjectManager>
{
    Dictionary<int, FObjectBase> objectMap = new Dictionary<int, FObjectBase>();
    List<FObjectBase> enemyList = new List<FObjectBase>();
    List<FObjectBase> userList = new List<FObjectBase>();
    List<FObjectBase> removeObjectList = new List<FObjectBase>();

    int enemySpawnCount = 0;
    int instanceID = 10;

    public FObjectBase FrontEnemy { get; private set; }
    public int EnemyCount { get { return enemyList.Count; } }

    public void Clear()
    {
        foreach(var pair in objectMap)
        {
            if(pair.Value != FGlobal.localPlayer && pair.Value != null)
            {
                GameObject.Destroy(pair.Value.gameObject);
            }
        }
        
        objectMap.Clear();
        enemyList.Clear();
        userList.Clear();

        enemySpawnCount = 0;
        instanceID = 10;
    }

    public void AddLocalPlayer(int InInstanceID, FObjectBase InLocalPlayer)
    {
        AddObject(InInstanceID, InLocalPlayer);
     
        userList.Add(InLocalPlayer);
    }

    public void CreateRemotePlayer(int InInstanceID)
    {
        GameObject gameObject = new GameObject("remotePlayer");
        FGlobal.remotePlayer = gameObject.AddComponent<FRemotePlayer>();
        FGlobal.remotePlayer.ObjectID = InInstanceID;
        FGlobal.remotePlayer.Initialize();

        AddObject(InInstanceID, FGlobal.remotePlayer);
     
        userList.Add(FGlobal.remotePlayer);
    }

    public int CreateLocalPlayerBattleDice(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        InDiceID = 12;

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

    public void CreateEnemy(int InID, FObjectBase InOwner)
    {
        FObjectBase enemy = CreateEnemy(instanceID++, InID, InOwner);
        if (enemy != null)
        {
            P2P_SPAWN_ENEMY pkt = new P2P_SPAWN_ENEMY();
            pkt.instanceId = enemy.ObjectID;
            pkt.enemyId = InID;
            pkt.ownerId = InOwner.ObjectID;
            FServerManager.Instance.SendMessage(pkt);
        }
    }

    public FObjectBase CreateEnemy(int InInstanceID, int InID, FObjectBase InOwner)
    {
        FEnemyData enemyData = FObjectDataManager.Instance.FindEnemyData(InID);
        if (enemyData == null)
            return null;

        FEnemy newEnemy = Instantiate<FEnemy>(Resources.Load<FEnemy>(enemyData.prefabPath), transform);
        newEnemy.SummonOwner = InOwner;
        newEnemy.Initialize(enemyData, enemySpawnCount++ / 2, FPathManager.Instance.FindStartPoint(InOwner.UserIndex));
        newEnemy.SortingOrder = -InInstanceID;

        enemyList.Add(newEnemy);
        AddObject(InInstanceID, newEnemy);

        return newEnemy;
    }

    public void CreateCollisionObject(int InID, FObjectBase InOwner, float InPathRate)
    {
        FObjectBase objectBase = CreateCollisionObject(instanceID++, InID, InOwner, InPathRate);
        if (objectBase == null)
            return;

        P2P_SPAWN_COLLISION_OBJECT pkt = new P2P_SPAWN_COLLISION_OBJECT();
        pkt.collisionObjectId = InID;
        pkt.objectId = objectBase.ObjectID;
        pkt.ownerObjectId = InOwner.ObjectID;
        pkt.pathRate = InPathRate;

        FServerManager.Instance.SendMessage(pkt);
    }

    public FObjectBase CreateCollisionObject(int InInstanceID, int InID, FObjectBase InOwner, float InPathRate)
    {
        FCollisionData collisionData = FObjectDataManager.Instance.FindCollisionData(InID);
        if (collisionData == null)
            return null;

        FCollisionObject collision = Instantiate<FCollisionObject>(Resources.Load<FCollisionObject>(collisionData.prefab), transform);
        collision.Initialize(collisionData, InOwner);
        collision.WorldPosition = FPathManager.Instance.GetPointInPathByRate(InOwner.SummonOwner.UserIndex, InPathRate);
        collision.transform.Rotate(new Vector3(0, 0, 1), FPathManager.Instance.GetAngleByRate(InOwner.SummonOwner.UserIndex, InPathRate));

        AddObject(InInstanceID, collision);

        return collision;
    }

    private void AddObject(int InObjectID, FObjectBase InObject)
    {
        if (objectMap.ContainsKey(InObjectID))
            return;

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

    public void ForeachUser(ForeachObjectDelegate InFunc)
    {
        foreach (FObjectBase user in userList)
        {
            InFunc(user);
        }
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
        targetStatController.OnDamage(damage);

        P2P_DAMAGE pkt = new P2P_DAMAGE();
        pkt.objectId = InTarget.ObjectID;
        pkt.damage = damage;
        pkt.critical = critical;

        FServerManager.Instance.SendMessage(pkt);
    }

    private void Update()
    {
        RemoveObjectAfterTick();
    }

    private void RemoveObjectAfterTick()
    {
        foreach (FObjectBase InObject in removeObjectList)
        {
            enemyList.Remove(InObject);

            InObject.Release();
            GameObject.Destroy(InObject.gameObject);
            objectMap.Remove(InObject.ObjectID);
        }
        removeObjectList.Clear();
    }
}
