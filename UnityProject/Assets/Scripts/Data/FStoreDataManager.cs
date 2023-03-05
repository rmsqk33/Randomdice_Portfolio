using FEnum;
using System.Collections.Generic;

public class FBoxGoodsData
{
    public readonly DiceGrade grade;
    public readonly int min;
    public readonly int max;

    public FBoxGoodsData(DiceGrade grade, int min, int max)
    {
        this.grade = grade;
        this.min = min;
        this.max = max;
    }
}

public class FStoreBoxData
{
    public readonly int id;
    public readonly int price;
    public readonly int gold;
    public readonly string name;
    public readonly string boxImagePath;
    private List<FBoxGoodsData> goodsList = new List<FBoxGoodsData>();

    public FStoreBoxData(int id, int price, int gold, string name, string boxImagePath, FDataNode InNode)
    {
        this.id = id;
        this.price = price;
        this.gold = gold;
        this.name = name;
        this.boxImagePath = boxImagePath;

        InNode.ForeachChildNodes("Dice", (in FDataNode InDiceNode) => {
            DiceGrade grade = (DiceGrade)InDiceNode.GetIntAttr("grade");
            int min = InDiceNode.GetIntAttr("min");
            int max = InDiceNode.GetIntAttr("max");

            FBoxGoodsData diceData = new FBoxGoodsData(grade, min, max);
            goodsList.Add(diceData);
        });
    }

    public delegate void ForeachGoodsDataDelegate(FBoxGoodsData InData);
    public void ForeachGoodsData(ForeachGoodsDataDelegate InFunc)
    {
        foreach (FBoxGoodsData data in goodsList)
        {
            InFunc(data);
        }
    }
}

public class FStoreDataManager : FNonObjectSingleton<FStoreDataManager>
{
    public string boxStoreTitle;
    Dictionary<int, FStoreBoxData> boxDataMap = new Dictionary<int, FStoreBoxData>();
    Dictionary<DiceGrade, string> boxGoodsImageMap = new Dictionary<DiceGrade, string>();

    public void Initialize()
    {
        FDataNode dataNode = FDataCenter.Instance.GetDataNodeWithQuery("StoreDataList.BoxStoreData");
        if(dataNode != null)
        {
            boxStoreTitle = dataNode.GetStringAttr("name");
            dataNode.ForeachChildNodes("Card", (in FDataNode InNode) => {
                DiceGrade grade = (DiceGrade)InNode.GetIntAttr("grade");
                string imagePath = InNode.GetStringAttr("image");
                boxGoodsImageMap.Add(grade, imagePath);
            });

            dataNode.ForeachChildNodes("Box", (in FDataNode InBoxNode) => {
                int id = InBoxNode.GetIntAttr("id");
                int price = InBoxNode.GetIntAttr("price");
                int gold = InBoxNode.GetIntAttr("gold");
                string name = InBoxNode.GetStringAttr("name");
                string boxImagePath = InBoxNode.GetStringAttr("image");

                FStoreBoxData boxData = new FStoreBoxData(id, price, gold, name, boxImagePath, InBoxNode);
                boxDataMap.Add(boxData.id, boxData);
            });
        }
    }

    public string GetBoxGoodsImage(DiceGrade InGrade)
    {
        if (boxGoodsImageMap.ContainsKey(InGrade))
            return boxGoodsImageMap[InGrade];

        return "";
    }

    public FStoreBoxData FindStoreBoxData(int InID)
    {
        if (boxDataMap.ContainsKey(InID))
            return boxDataMap[InID];

        return null;
    }

    public delegate void ForeachStoreBoxDataFunc(in FStoreBoxData InData);
    public void ForeachStoreBoxData(ForeachStoreBoxDataFunc InFunc)
    {
        foreach(var iter in boxDataMap)
        {
            InFunc(iter.Value);
        }
    }
}
