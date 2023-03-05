using FEnum;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using UnityEditor.Experimental.GraphView;
using UnityEngine;

public class FDiceData
{
    public readonly int id;
    public readonly DiceGrade grade;
    public readonly string name;
    public readonly string description;
    public readonly string iconPath;
    public readonly string notAcquiredIconPath;
    public readonly Color color;

    public FDiceData(int id, DiceGrade grade, string name, string description, string iconPath, string notAcquiredIconPath, Color color)
    {
        this.id = id;
        this.grade = grade;
        this.name = name;
        this.description = description;
        this.iconPath = iconPath;
        this.notAcquiredIconPath = notAcquiredIconPath;
        this.color = color;
    }
}

public class FDiceLevelData
{
    public readonly int level;
    public readonly int diceCountCost;
    public readonly int goldCost;

    public FDiceLevelData(int level, int diceCountCost, int goldCost)
    {
        this.level = level;
        this.diceCountCost = diceCountCost;
        this.goldCost = goldCost;
    }
}

public class FDiceGradeData
{
    public readonly DiceGrade grade;
    public readonly string gradeName;
    public readonly string backgroundPath;
    public readonly int initialLevel;
    public readonly int maxLevel;
    public readonly int critical;

    private Dictionary<int, FDiceLevelData> levelDataMap = new Dictionary<int, FDiceLevelData>();

    public FDiceGradeData(DiceGrade grade, string gradeName, string backgroundPath, int initialLevel, int critical, FDataNode InNode)
    {
        this.grade = grade;
        this.gradeName = gradeName;
        this.backgroundPath = backgroundPath;
        this.initialLevel = initialLevel;
        this.critical = critical;

        InNode.ForeachChildNodes("Level", (in FDataNode InNode) => {
            int level = InNode.GetIntAttr("level");
            int diceCountCost = InNode.GetIntAttr("diceCountCost");
            int goldCost = InNode.GetIntAttr("goldCost");

            FDiceLevelData levelData = new FDiceLevelData(level, diceCountCost, goldCost);
            levelDataMap.Add(levelData.level, levelData);
        });

        this.maxLevel = levelDataMap.Keys.Max();
    }

    public FDiceLevelData FindDiceLevelData(int InLevel)
    {
        if (levelDataMap.ContainsKey(InLevel))
            return levelDataMap[InLevel];

        return null;
    }
}

public class FDiceDataManager : FNonObjectSingleton<FDiceDataManager>
{
    Dictionary<int, FDiceData> diceDataMap = new Dictionary<int, FDiceData>();
    Dictionary<DiceGrade, FDiceGradeData> diceGradeDataMap = new Dictionary<DiceGrade, FDiceGradeData>();

    public void Initialize()
    {
        List<FDataNode> diceDataNodes = FDataCenter.Instance.GetDataNodesWithQuery("DiceList.Dice");
        foreach(FDataNode node in diceDataNodes)
        {
            int id = node.GetIntAttr("id");
            DiceGrade grade = (DiceGrade)node.GetIntAttr("grade");
            string name = node.GetStringAttr("name");
            string description = node.GetStringAttr("description");
            string iconPath = node.GetStringAttr("icon");
            string notAcquiredIconPath = node.GetStringAttr("notAcquiredIcon");
            Color color = node.GetColorAttr("color");

            FDiceData data = new FDiceData(id, grade, name, description, iconPath, notAcquiredIconPath, color);
            diceDataMap.Add(data.id, data);
        }

        List<FDataNode> diceGradeDataNodes = FDataCenter.Instance.GetDataNodesWithQuery("DiceGradeList.DiceGrade");
        foreach (FDataNode node in diceGradeDataNodes)
        {
            DiceGrade grade = (DiceGrade)node.GetIntAttr("grade");
            string gradeName = node.GetStringAttr("name");
            string backgroundPath = node.GetStringAttr("invenSlotImage");
            int initialLevel = node.GetIntAttr("initialLevel");
            int critical = node.GetIntAttr("critical");

            FDiceGradeData gradeData = new FDiceGradeData(grade, gradeName, backgroundPath, initialLevel, critical, node);
            diceGradeDataMap.Add(gradeData.grade, gradeData);
        }
    }

    public delegate void ForeachDiceDataFunc(in FDiceData InDiceData);
    public void ForeachDiceData(in ForeachDiceDataFunc InFunc)
    {
        foreach (FDiceData data in diceDataMap.Values)
        {
            InFunc(data);
        }
    }

    public FDiceData FindDiceData(int InID)
    {
        if (diceDataMap.ContainsKey(InID))
            return diceDataMap[InID];

        return null;
    }

    public FDiceGradeData FindGradeDataByID(int InID)
    {
        FDiceData diceData = FindDiceData(InID);
        if (diceData != null)
            return FindGradeData(diceData.grade);

        return null;
    }

    public FDiceGradeData FindGradeData(DiceGrade InGrade)
    {
        if(diceGradeDataMap.ContainsKey(InGrade))
            return diceGradeDataMap[InGrade];

        return null;
    }

    public FDiceLevelData FindDiceLevelData(int InID, int InLevel)
    {
        FDiceGradeData diceGradeData = FindGradeDataByID(InID);
        if (diceGradeData == null)
            return null;

        return diceGradeData.FindDiceLevelData(InLevel);
    }
}
