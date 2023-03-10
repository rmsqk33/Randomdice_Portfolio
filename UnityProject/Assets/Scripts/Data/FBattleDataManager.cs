using System.Collections.Generic;
using UnityEngine;

public class FBattleDiceLevelData
{
	public readonly int level;
	public readonly int cost;
	public readonly double attackRate;

    public FBattleDiceLevelData(int level, int cost, double attackRate)
	{
		this.level = level;
		this.cost = cost;
		this.attackRate = attackRate;
    }
}

public class FBattleDataManager : FNonObjectSingleton<FBattleDataManager>
{
	Dictionary<int, FBattleDiceLevelData> diceLevelMap = new Dictionary<int, FBattleDiceLevelData>();

    public int InitSP { get; private set; }
	public int InitDiceSummonCost { get; private set; }
	public int DiceSummonCostIncrease { get; private set; }
    public int MaxLevel { get; private set; }
    public int MaxEyeCount { get; private set; }
    
    public void Initialize()
    {
		FDataNode battleData = FDataCenter.Instance.GetDataNodeWithQuery("BattleData");
        if (battleData != null)
		{
			InitSP = battleData.GetIntAttr("initSP");
            InitDiceSummonCost = battleData.GetIntAttr("initDiceSummonCost");
            DiceSummonCostIncrease = battleData.GetIntAttr("diceSummonCostIncrease");
            MaxEyeCount = battleData.GetIntAttr("maxEyeCount");

            List<FDataNode> diceUpgradeNodes = battleData.GetDataNodesWithQuery("DiceUpgrade.Dice");
			foreach(FDataNode dataNode in diceUpgradeNodes)
			{
                int level = dataNode.GetIntAttr("level");
                int cost = dataNode.GetIntAttr("cost");
                double attackRate = dataNode.GetDoubleAttr("attackRate");

                FBattleDiceLevelData levelData = new FBattleDiceLevelData(level, cost, attackRate);
				diceLevelMap.Add(levelData.level, levelData);
            }

			MaxLevel = diceLevelMap.Count;
        }
    }

	public FBattleDiceLevelData FindDiceLevelData(int InLevel)
	{
		if(diceLevelMap.ContainsKey(InLevel))
			return diceLevelMap[InLevel];

		return null;
	}
}
