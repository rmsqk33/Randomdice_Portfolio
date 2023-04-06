using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FStatController : FControllerBase
{
    Dictionary<StatType, float> statMap = new Dictionary<StatType, float>();
    List<FStatObserver> observers = new List<FStatObserver>();

    public FStatController(FObjectBase InOwner) : base(InOwner)
    {
    }

    public void SetStat(StatType InType, float InValue)
    {
        if(statMap.ContainsKey(InType))
            statMap[InType] = InValue;
        else
            statMap.Add(InType, InValue);

        foreach(FStatObserver observer in observers)
        {
            observer.OnStatChanged(InType, InValue);
        }
    }

    public void OnDamage(int InDamage)
    {
        int hp = GetIntStat(StatType.HP) - InDamage;
        SetStat(StatType.HP, hp);

        if(hp <= 0)
        {
            OnDeath();
        }
    }

    public float GetStat(StatType InType)
    {
        if(statMap.ContainsKey(InType))
            return statMap[InType];

        return 0;
    }

    public int GetIntStat(StatType InType)
    {
        if (statMap.ContainsKey(InType))
            return (int)statMap[InType];

        return 0;
    }

    public bool IsCritical()
    {
        return Random.value <= GetStat(StatType.CriticalChance);
    }

    public void AddObserver(FStatObserver InObserver)
    {
        observers.Add(InObserver);
    }

    public void RemoveObserver(FStatObserver InObserver)
    {
        observers.Remove(InObserver);
    }

    private void OnDeath()
    {
        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController != null)
        {
            battleController.SP += GetIntStat(StatType.SP);
        }

        FObjectManager.Instance.RemoveObject(ObjectID);
    }
}
