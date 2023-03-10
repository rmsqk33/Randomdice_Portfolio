using System.Collections.Generic;

public class FBattleFieldData
{
    public readonly int id;
    public readonly int price;
    public readonly string name;
    public readonly string skinImagePath;
    
    public FBattleFieldData(int id, int price, string name, string skinImagePath)
    {
        this.id = id;
        this.price = price;
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
            int price = node.GetIntAttr("price");
            string name = node.GetStringAttr("name");
            string skinImagePath = node.GetStringAttr("skinImage");

            FBattleFieldData newData = new FBattleFieldData(id, price, name, skinImagePath);
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
