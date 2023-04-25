using Packet;
using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FObjectManager : FSingleton<FObjectManager>
{
    Dictionary<int, FObjectBase> objectMap = new Dictionary<int, FObjectBase>();
    List<FObjectBase> enemyList = new List<FObjectBase>();
    List<FObjectBase> removeObjectList = new List<FObjectBase>();

    int enemySpawnCount = 0;
    int instanceID = 0;

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

        enemySpawnCount = 0;
        instanceID = 0;
        AddObject(instanceID++, FGlobal.localPlayer);
    }

    public void CreateLocalPlayer()
    {
        GameObject gameObject = new GameObject("localPlayer");
        FGlobal.localPlayer = gameObject.AddComponent<FLocalPlayer>();
        GameObject.DontDestroyOnLoad(gameObject);

        AddObject(instanceID++, FGlobal.localPlayer);
    }

    public void CreateRemotePlayer()
    {
        GameObject gameObject = new GameObject("remotePlayer");
        FGlobal.remotePlayer = gameObject.AddComponent<FRemotePlayer>();
        FGlobal.remotePlayer.ObjectID = instanceID++;
        FGlobal.remotePlayer.Initialize();

        AddObject(FGlobal.remotePlayer.ObjectID, FGlobal.remotePlayer);
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

    public FObjectBase CreateEnemy(int InID)
    {
        return CreateEnemy(instanceID++, InID);
    }

    public FObjectBase CreateEnemy(int InInstanceID, int InID)
    {
        FEnemyData enemyData = FObjectDataManager.Instance.FindEnemyData(InID);
        if (enemyData == null)
            return null;

        FEnemy newEnemy = Instantiate<FEnemy>(Resources.Load<FEnemy>(enemyData.prefabPath), transform);
        newEnemy.Initialize(enemyData, enemySpawnCount++ / 2);
        newEnemy.SortingOrder = -InInstanceID;

        enemyList.Add(newEnemy);
        AddObject(InInstanceID, newEnemy);

        return newEnemy;
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
