using System.Collections.Generic;
using FEnum;
using UnityEngine;

public class FCombatTextManager : FSceneLoadedSingleton<FCombatTextManager>
{
    [SerializeField]
    FDictionary<CombatTextType, FCombatText> combatTextPrefabs;

    Dictionary<int, FCombatText> combatTextList = new Dictionary<int, FCombatText>();
    Dictionary<CombatTextType, List<FCombatText>> combatTextPool = new Dictionary<CombatTextType, List<FCombatText>>();
    int instanceID;

    protected override void Awake()
    {
        base.Awake();

        for(CombatTextType i = CombatTextType.None + 1; i < CombatTextType.Max; ++i)
        {
            combatTextPool.Add(i, new List<FCombatText>());
        }
    }

    private void Update()
    {
        foreach (var pair in combatTextList)
        {
            pair.Value.Tick();
        }
    }

    public void AddText(CombatTextType InType, int InValue, FObjectBase InTarget)
    {
        FCombatText combatText;

        List<FCombatText> pool = combatTextPool[InType];
        if (pool.Count == 0)
        {
            combatText = Instantiate(combatTextPrefabs[InType], transform);
        }
        else
        {
            int index = pool.Count - 1;
            combatText = pool[index];
            pool.RemoveAt(index);

            combatText.gameObject.SetActive(true);
        }

        combatText.InstanceID = instanceID;
        combatText.Value = InValue;
        combatText.Type = InType;
        combatText.WorldPosition = InTarget.WorldPosition;
        combatText.Target = InTarget;

        combatTextList.Add(instanceID++, combatText);
    }

    public void RemoveText(int InInstanceID)
    {
        if(combatTextList.ContainsKey(InInstanceID))
        {
            FCombatText combatText = combatTextList[InInstanceID];
            combatTextPool[combatText.Type].Add(combatText);
            combatTextList.Remove(InInstanceID);

            combatText.gameObject.SetActive(false);
        }
    }
}
