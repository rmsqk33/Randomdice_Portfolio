using System.Collections.Generic;
using UnityEngine;

public class FBattleDiceUpgradeData
{
	public int Level { get; set; }
	public int Cost { get; set; }
	public float AttackRate { get; set; }
}

public class FBattleDataManager : FNonObjectSingleton<FBattleDataManager>
{
	Dictionary<int, FBattleDiceUpgradeData> diceUpgradeMap = new Dictionary<int, FBattleDiceUpgradeData>();

    public int InitSP { get; private set; }
	public int InitDiceSummonCost { get; private set; }
	public int DiceSummonCostIncrease { get; private set; }

    public void Initialize()
    {
		FDataNode coopData = FDataCenter.Instance.GetDataNodeWithQuery("CoopData");
        if (coopData != null)
		{
			InitSP = coopData.GetIntAttr("initSP");
            InitDiceSummonCost = coopData.GetIntAttr("initDiceSummonCost");
            DiceSummonCostIncrease = coopData.GetIntAttr("diceSummonCostIncrease");

			List<FDataNode> diceUpgradeNodes = coopData.GetDataNodesWithQuery("DiceUpgrade.Dice");
			foreach(FDataNode dataNode in diceUpgradeNodes)
			{
				FBattleDiceUpgradeData upgradeData = new FBattleDiceUpgradeData();
                upgradeData.Level = dataNode.GetIntAttr("level");
                upgradeData.Cost = dataNode.GetIntAttr("cost");
                upgradeData.AttackRate = dataNode.GetIntAttr("attackRate");

				diceUpgradeMap.Add(upgradeData.Level, upgradeData);
            }
        }
    }

	public FBattleDiceUpgradeData FindDiceUpgradeData(int InLevel)
	{
		if(diceUpgradeMap.ContainsKey(InLevel))
			return diceUpgradeMap[InLevel];

		return null;
	}
}
