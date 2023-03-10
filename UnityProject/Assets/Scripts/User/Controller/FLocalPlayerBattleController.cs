using FEnum;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FBattleDicePreset
{
    public readonly int index;
    public readonly int diceID;
    public int level = 1;
    public int eyeCount = 0;
    public int upgradeCost;
    public double attackRate;
    
    int upgradeDisableFlag;

    public bool IsUpgradable { get { return upgradeDisableFlag == 0; } }

    public FBattleDicePreset(int index, int diceID)
    {
        this.index = index;
        this.diceID = diceID;
    }

    public void SetUpgradeDisableFlag(DiceUpgradeDisableReason InReason, bool InEnable)
    {
        upgradeDisableFlag = InEnable ? upgradeDisableFlag &= ~(1 << (int)InReason) : upgradeDisableFlag |= 1 << (int)InReason;
    }
}

public class FLocalPlayerBattleController : FControllerBase
{
    private FBattleDicePreset[] dicePresetList = new FBattleDicePreset[FGlobal.MAX_PRESET];
    private Dictionary<int, FBattleDice> summonDiceMap = new Dictionary<int, FBattleDice>();
    private List<int> emptyDiceSlotIndexList = Enumerable.Range(0, FGlobal.MAX_SUMMON_DICE).ToList();

    int sp;
    int diceSummonCost;
    int diceSummonDisableFlag;

    public int CardIncrease { get; set; }
    public int Wave { get; set; }

    public FLocalPlayerBattleController(FLocalPlayer InOwner) : base(InOwner)
    {

    }

    public int SP
    {
        get { return sp; }
        set
        {
            sp = value;

            FLocalPlayerBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetSP(sp);
            }

            UpdateDiceUpgradableBySP();
            SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_ENOUGH_SP, diceSummonCost <= sp);
        }
    }

    public int DiceSummonCost
    {
        get { return diceSummonCost; }
        set
        {
            diceSummonCost = value;

            FLocalPlayerBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetDiceSummonCost(diceSummonCost);
            }

            SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_ENOUGH_SP, diceSummonCost <= sp);
        }
    }

    public bool IsDiceSummonable { get { return diceSummonDisableFlag == 0; } }

    public override void Initialize() 
    {
        sp = FBattleDataManager.Instance.InitSP + 100000;
        diceSummonCost = FBattleDataManager.Instance.InitDiceSummonCost;

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if(presetController != null)
        {
            FBattleDiceLevelData levelData = FBattleDataManager.Instance.FindDiceLevelData(1);
            if(levelData != null)
            {
                int i = 0;
                presetController.ForeachDicePreset(presetController.SelectedPresetIndex, (int InDiceID) => {
                    FBattleDicePreset battleDice = new FBattleDicePreset(i, InDiceID);
                    battleDice.upgradeCost = levelData.cost;
                    battleDice.attackRate = levelData.attackRate;
                    battleDice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.NOT_ENOUGH_SP, battleDice.upgradeCost <= SP);
                    dicePresetList[i] = battleDice;
                    ++i;
                });
            }
        }

        FLocalPlayerBattlePanelUI battleUI = FindBattlePanelUI();
        if(battleUI != null)
        {
            battleUI.Init();
        }   
    }

    public void DiceLevelUp(int InIndex)
    {
        if (dicePresetList.Count() <= InIndex)
            return;

        FBattleDicePreset dice = dicePresetList[InIndex];
        if (FBattleDataManager.Instance.MaxLevel <= dice.level)
            return;

        int cost = dice.upgradeCost;
        if (SP < cost)
            return;

        FBattleDiceLevelData levelData = FBattleDataManager.Instance.FindDiceLevelData(dice.level + 1);
        if (levelData == null)
            return;

        ++dice.level;
        dice.upgradeCost = levelData.cost;
        dice.attackRate = levelData.attackRate;
        dice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.MAX_LEVEL, dice.level < FBattleDataManager.Instance.MaxLevel);

        SP -= cost;
        
        FLocalPlayerBattlePanelUI battleUI = FindBattlePanelUI();
        if(battleUI != null)
        {
            battleUI.SetDiceLevel(InIndex, dice.level);
            battleUI.SetDiceUpgradeCost(InIndex, dice.upgradeCost);
            battleUI.SetDiceUpgradable(InIndex, dice.IsUpgradable);
        }
    }

    public void SummonDiceRandomSlot(int InEyeCount = 1)
    {
        if (SP < DiceSummonCost)
            return;

        if (emptyDiceSlotIndexList.Count == 0)
            return;

        SP -= DiceSummonCost;
        DiceSummonCost += FBattleDataManager.Instance.DiceSummonCostIncrease;

        int summonSlotIndex = Random.Range(0, emptyDiceSlotIndexList.Count);
        int summonDiceID = dicePresetList[Random.Range(0, dicePresetList.Count())].diceID;

        CreateSummonDice(emptyDiceSlotIndexList[summonSlotIndex], summonDiceID, InEyeCount);
    }

    public void SummonDiceRandomDice(int InSlotIndex, int InEyeCount)
    {
        int summonDiceID = dicePresetList[Random.Range(0, dicePresetList.Count())].diceID;

        CreateSummonDice(InSlotIndex, summonDiceID, InEyeCount);
    }

    public void CombineDice(int InDestIndex, int InSourceIndex)
    {
        if (summonDiceMap.ContainsKey(InDestIndex) == false || summonDiceMap.ContainsKey(InSourceIndex) == false)
            return;

        if (summonDiceMap[InSourceIndex].IsCombinable(summonDiceMap[InDestIndex]) == false)
            return;

        summonDiceMap[InSourceIndex].CombineDice(summonDiceMap[InDestIndex]);
    }

    public void CreateSummonDice(int InIndex, int InDiceID, int InEyeCount)
    {
        if (summonDiceMap.ContainsKey(InIndex))
            return;

        FLocalPlayerBattleBoardUI boardUI = FindBattleBoardUI();
        if (boardUI == null)
            return;

        FBattleDiceUI diceUI = boardUI.CreateDice(InDiceID, InEyeCount, InIndex);
        summonDiceMap.Add(InIndex, new FBattleDice(InDiceID, InEyeCount, diceUI));
        emptyDiceSlotIndexList.Remove(InIndex);

        SetDiceEyeTotalCount(InDiceID, InEyeCount);
        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    public void RemoveSummonDice(int InIndex)
    {
        if (summonDiceMap.ContainsKey(InIndex) == false)
            return;

        SetDiceEyeTotalCount(summonDiceMap[InIndex].DiceID, -summonDiceMap[InIndex].EyeCount);

        summonDiceMap.Remove(InIndex);
        emptyDiceSlotIndexList.Add(InIndex);

        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);

        FLocalPlayerBattleBoardUI boardUI = FindBattleBoardUI();
        if (boardUI != null)
        {
            boardUI.RemoveDice(InIndex);
        }
    }

    public delegate void ForeachBattleDicePresetDelegate(FBattleDicePreset InDice);
    public void ForeachBattleDicePreset(ForeachBattleDicePresetDelegate InFunc)
    {
        foreach(FBattleDicePreset dice in dicePresetList)
        {
            InFunc(dice);
        }
    }

    public delegate void ForeachSummonDiceDelegate(FBattleDice InDice);
    public void ForeachSummonDice(ForeachSummonDiceDelegate InFunc)
    {
        foreach(var pair in summonDiceMap)
        {
            InFunc(pair.Value);
        }
    }

    public FBattleDice FindSummonDice(int InIndex)
    {
        if (summonDiceMap.ContainsKey(InIndex))
            return summonDiceMap[InIndex];
     
        return null;
    }

    private void SetDiceSummonDisableReason(DiceSummonDisableReason InReason, bool InEnable)
    {
        bool prevState = IsDiceSummonable;

        diceSummonDisableFlag = InEnable ? diceSummonDisableFlag &= ~(1 << (int)InReason) : diceSummonDisableFlag |= 1 << (int)InReason;
        if (prevState == IsDiceSummonable)
            return;

        FLocalPlayerBattlePanelUI ui = FindBattlePanelUI();
        if (ui != null)
        {
            ui.SetDiceSummonBtnEnable(IsDiceSummonable);
        }
    }

    private void SetDiceEyeTotalCount(int InDiceID, int InDelta)
    {
        FBattleDicePreset preset = FindBattleDicePreset(InDiceID);
        if(preset != null)
        {
            preset.eyeCount += InDelta;

            FLocalPlayerBattlePanelUI battleUI = FindBattlePanelUI();
            if (battleUI != null)
            {
                battleUI.SetDiceEyeCount(preset.index, preset.eyeCount);
            }
        }
    }

    private void UpdateDiceUpgradableBySP()
    {
        FLocalPlayerBattlePanelUI ui = FindBattlePanelUI();
        ForeachBattleDicePreset((FBattleDicePreset InDice) =>
        {
            bool prevState = InDice.IsUpgradable;
            InDice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.NOT_ENOUGH_SP, InDice.upgradeCost <= sp);
            if (prevState != InDice.IsUpgradable)
            {
                ui.SetDiceUpgradable(InDice.index, InDice.IsUpgradable);
            }
        });
    }

    private FBattleDicePreset FindBattleDicePreset(int InDiceID)
    {
        foreach(FBattleDicePreset info in dicePresetList)
        {
            if (info.diceID == InDiceID)
                return info;
        }
        return null;
    }

    private FLocalPlayerBattleBoardUI FindBattleBoardUI()
    {
        return FUIManager.Instance.FindUI<FLocalPlayerBattleBoardUI>();
    }

    private FLocalPlayerBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FLocalPlayerBattlePanelUI>();
    }
}
