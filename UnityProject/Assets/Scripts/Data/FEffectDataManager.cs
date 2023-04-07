using FEnum;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class FProjectileData
{
    public readonly int id;
    public readonly int effectID;
    public readonly int abnormalityID;
    public readonly int speed;
    public readonly string prefab;

    public FProjectileData(FDataNode InNode)
    {
        id = InNode.GetIntAttr("id");
        effectID = InNode.GetIntAttr("effectID");
        abnormalityID = InNode.GetIntAttr("abnormalityID");
        speed = InNode.GetIntAttr("speed");
        prefab = InNode.GetStringAttr("prefab");
    }
}

public class FEffectData
{
    public readonly int id;
    public readonly SkillEffectType type;
    public readonly string prefab;

    public readonly int damage;
    public readonly int damagePerLevel;
    public readonly int damagePerBattleLevel;
    public readonly int radius;
    public readonly int duration;

    public FEffectData(FDataNode InNode)
    {
        id = InNode.GetIntAttr("id");
        type = (SkillEffectType)InNode.GetIntAttr("type");
        prefab = InNode.GetStringAttr("prefab");

        damage = InNode.GetIntAttr("damage");
        damagePerLevel = InNode.GetIntAttr("damagePerLevel");
        damagePerBattleLevel = InNode.GetIntAttr("damagePerBattleLevel");
        radius = InNode.GetIntAttr("radius");
        duration = InNode.GetIntAttr("duration");
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
