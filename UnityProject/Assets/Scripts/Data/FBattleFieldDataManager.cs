using System.Collections.Generic;

public class FBattleFieldData
{
    public readonly int id;
    public readonly string name;
    public readonly string skinImagePath;

    public FBattleFieldData(int id, string name, string skinImagePath)
    {
        this.id = id;
        this.name = name;
        this.skinImagePath = skinImagePath;
    }
}

public class FBattleFieldDataManager : FNonObjectSingleton<FBattleFieldDataManager>
{
    Dictionary<int, FBattleFieldData> battleFieldDataMap = new Dictionary<int, FBattleFieldData>();

    public void Initialize()
    {
        List<FDataNode> dataNodeList = FDataCenter.Instance.GetDataNodesWithQuery("BattleFieldList.BattleField");
        foreach (FDataNode node in dataNodeList)
        {
            int id = node.GetIntAttr("id");
            string name = node.GetStringAttr("name");
            string skinImagePath = node.GetStringAttr("skinImage");

            FBattleFieldData newData = new FBattleFieldData(id, name, skinImagePath);
            battleFieldDataMap.Add(newData.id, newData);
        }
    }

    public delegate void ForeachBattleFieldDataFunc(FBattleFieldData InData);
    public void ForeachBattleFieldData(ForeachBattleFieldDataFunc InFunc)
    {
        foreach(FBattleFieldData data in battleFieldDataMap.Values)
        {
            InFunc(data);
        }
    }

    public FBattleFieldData FindBattleFieldData(int InID)
    {
        if (battleFieldDataMap.ContainsKey(InID))
            return battleFieldDataMap[InID];

        return null;
    }
}
