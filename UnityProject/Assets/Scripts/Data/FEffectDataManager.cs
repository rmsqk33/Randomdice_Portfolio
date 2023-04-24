using FEnum;
using System.Collections.Generic;

public class FProjectileData
{
    public readonly int id;
    public readonly int effectID;
    public readonly int abnormalityID;
    public readonly int speed;
    public readonly int damage;
    public readonly int damagePerLevel;
    public readonly int damagePerBattleLevel;
    public readonly string prefab;

    public FProjectileData(FDataNode InNode)
    {
        id = InNode.GetIntAttr("id");
        effectID = InNode.GetIntAttr("effectID");
        abnormalityID = InNode.GetIntAttr("abnormalityID");
        speed = InNode.GetIntAttr("speed");
        damage = InNode.GetIntAttr("damage");
        damagePerLevel = InNode.GetIntAttr("damagePerLevel");
        damagePerBattleLevel = InNode.GetIntAttr("damagePerBattleLevel");
        prefab = InNode.GetStringAttr("prefab");
    }
}

public class FEffectData
{
    public readonly int id;
    public readonly SkillEffectType type;
    public readonly string prefab;

    public readonly int value;
    public readonly int valuePerLevel;
    public readonly int valuePerBattleLevel;
    public readonly int radius;

    public readonly int chainCount;
    public readonly float chainDamageRate;
    public readonly string chainPrefab;

    public readonly AbilityType abilityType;

    public FEffectData(FDataNode InNode)
    {
        id = InNode.GetIntAttr("id");
        type = (SkillEffectType)InNode.GetIntAttr("type");
        prefab = InNode.GetStringAttr("prefab");

        value = InNode.GetIntAttr("value");
        valuePerLevel = InNode.GetIntAttr("valuePerLevel");
        valuePerBattleLevel = InNode.GetIntAttr("valuePerBattleLevel");
        radius = InNode.GetIntAttr("radius");

        chainCount = InNode.GetIntAttr("chainCount");
        chainDamageRate = InNode.GetFloatAttr("chainDamageRate");
        chainPrefab = InNode.GetStringAttr("chainPrefab");

        abilityType = (AbilityType)InNode.GetIntAttr("abilityType");
    }
}

public class FEffectDataManager : FNonObjectSingleton<FEffectDataManager>
{
    Dictionary<int, FProjectileData> projectileDataMap = new Dictionary<int, FProjectileData>();
    Dictionary<int, FEffectData> effectDataMap = new Dictionary<int, FEffectData>();

    public void Initialize()
    {
        List<FDataNode> projectileNodeList = FDataCenter.Instance.GetDataNodesWithQuery("ProjectileDataList.ProjectileData");
        foreach(FDataNode node in projectileNodeList)
        {
            FProjectileData projectileData = new FProjectileData(node);
            projectileDataMap.Add(projectileData.id, projectileData);
        }

        List<FDataNode> effectNodeList = FDataCenter.Instance.GetDataNodesWithQuery("EffectDataList.EffectData");
        foreach (FDataNode node in effectNodeList)
        {
            FEffectData effectData = new FEffectData(node);
            effectDataMap.Add(effectData.id, effectData);
        }
    }

    public FProjectileData FindProjectileData(int InID)
    {
        if (projectileDataMap.ContainsKey(InID))
            return projectileDataMap[InID];

        return null;
    }

    public FEffectData FindEffectData(int InID)
    {
        if (effectDataMap.ContainsKey(InID))
            return effectDataMap[InID];

        return null;
    }
}
