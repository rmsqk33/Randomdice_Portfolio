using FEnum;
using Packet;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class FEquipBattleDice
{
    public readonly int index;
    public readonly int diceID;
    public int level = 1;
    public int eyeCount = 0;
    public int upgradeCost;
    
    int upgradeDisableFlag;

    public bool IsUpgradable { get { return upgradeDisableFlag == 0; } }

    public FEquipBattleDice(int index, int diceID)
    {
        this.index = index;
        this.diceID = diceID;
    }

    public void SetUpgradeDisableFlag(DiceUpgradeDisableReason InReason, bool InEnable)
    {
        upgradeDisableFlag = InEnable ? upgradeDisableFlag &= ~(1 << (int)InReason) : upgradeDisableFlag |= 1 << (int)InReason;
    }
}

public class FBattleDiceController : FControllerBase
{
    private FEquipBattleDice[] equipDiceList = new FEquipBattleDice[FGlobal.MAX_PRESET];
    private Dictionary<int, FBattleDice> summonDiceMap = new Dictionary<int, FBattleDice>();
    private List<int> emptyDiceSlotIndexList = Enumerable.Range(0, FGlobal.MAX_SUMMON_DICE).ToList();

    int sp;
    int diceSummonCost;
    int diceSummonDisableFlag;
    int dragDiceIndex = -1;

    public FBattleDiceController(FLocalPlayer InOwner) : base(InOwner)
    {

    }

    public int SP
    {
        get { return sp; }
        set
        {
            sp = value;

            FBattlePanelUI ui = FindBattlePanelUI();
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

            FBattlePanelUI ui = FindBattlePanelUI();
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

        FPresetController presetController = FGlobal.localPlayer.FindController<FPresetController>();
        if(presetController != null)
        {
            FBattleDiceLevelData levelData = FBattleDataManager.Instance.FindDiceLevelData(1);
            if(levelData != null)
            {
                int i = 0;
                presetController.ForeachDicePreset(presetController.SelectedPresetIndex, (int InDiceID) => {
                    FEquipBattleDice battleDice = new FEquipBattleDice(i, InDiceID);
                    battleDice.upgradeCost = levelData.cost;
                    battleDice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.NOT_ENOUGH_SP, battleDice.upgradeCost <= SP);
                    equipDiceList[i] = battleDice;
                    ++i;
                });
            }

            FBattleFieldData battlefieldData = FBattleFieldDataManager.Instance.FindBattleFieldData(presetController.GetSelectedBattlefieldID());
            if (battlefieldData != null)
            {
                GameObject.Instantiate(Resources.Load(battlefieldData.battlefieldPrefab));
            }
        }

        FBattlePanelUI battleUI = FindBattlePanelUI();
        if(battleUI != null)
        {
            battleUI.Initialize();
        }
    }

    public void DiceLevelUp(int InIndex)
    {
        if (InIndex < 0 || equipDiceList.Count() <= InIndex)
            return;

        FEquipBattleDice dice = equipDiceList[InIndex];
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
        dice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.MAX_LEVEL, dice.level < FBattleDataManager.Instance.MaxLevel);

        SP -= cost;
        
        FBattlePanelUI battleUI = FindBattlePanelUI();
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
        int summonDiceID = equipDiceList[Random.Range(0, equipDiceList.Count())].diceID;

        CreateSummonDice(emptyDiceSlotIndexList[summonSlotIndex], summonDiceID, InEyeCount);
    }

    public void OnBegieDragDice(int InSlotIndex)
    {
        dragDiceIndex = InSlotIndex;
        ActiveDiceCombinable(InSlotIndex);
    }

    public void OnDropDice(int InSlotIndex)
    {
        if (dragDiceIndex == InSlotIndex)
            return;

        if (summonDiceMap.ContainsKey(dragDiceIndex) == false)
            return;

        if (summonDiceMap.ContainsKey(InSlotIndex) == false)
            return;

        FBattleDice dragDice = summonDiceMap[dragDiceIndex];
        FBattleDice dropDice = summonDiceMap[InSlotIndex];
        if (IsCombinableDice(dragDice, dropDice) == false)
            return;

        CombineDice(dragDice, dropDice);
        DeactiveDiceCombinable();
    }

    public void OnEndDragDice()
    {
        DeactiveDiceCombinable();
    }

    public delegate void ForeachBattleDicePresetDelegate(FEquipBattleDice InDice);
    public void ForeachEquipBattleDice(ForeachBattleDicePresetDelegate InFunc)
    {
        foreach (FEquipBattleDice dice in equipDiceList)
        {
            InFunc(dice);
        }
    }

    public delegate void ForeachSummonDiceDelegate(FBattleDice InDice);
    public void ForeachSummonDice(ForeachSummonDiceDelegate InFunc)
    {
        foreach (var pair in summonDiceMap)
        {
            InFunc(pair.Value);
        }
    }

    public FEquipBattleDice FindEquipBattleDice(int InDiceID)
    {
        foreach (FEquipBattleDice info in equipDiceList)
        {
            if (info.diceID == InDiceID)
                return info;
        }
        return null;
    }

    private void SummonDiceRandomDice(int InSlotIndex, int InEyeCount)
    {
        int summonDiceID = equipDiceList[Random.Range(0, equipDiceList.Count())].diceID;

        CreateSummonDice(InSlotIndex, summonDiceID, InEyeCount);
    }

    private void CreateSummonDice(int InIndex, int InDiceID, int InEyeCount)
    {
        if (summonDiceMap.ContainsKey(InIndex))
            return;

        FBattleDice dice = FBattleDiceCreator.Instance.CreateLocalPlayerDice(InDiceID, InEyeCount, InIndex);
        summonDiceMap.Add(InIndex, dice);
        emptyDiceSlotIndexList.Remove(InIndex);

        SetDiceEyeTotalCount(InDiceID, InEyeCount);
        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    private void RemoveSummonDice(int InIndex)
    {
        if (summonDiceMap.ContainsKey(InIndex) == false)
            return;

        FBattleDice dice = summonDiceMap[InIndex];
        FDiceStatController diceStatController = dice.FindController<FDiceStatController>();
        if (diceStatController == null)
            return;

        SetDiceEyeTotalCount(diceStatController.DiceID, -diceStatController.EyeCount);

        GameObject.Destroy(dice.gameObject);
        summonDiceMap.Remove(InIndex);
        emptyDiceSlotIndexList.Add(InIndex);

        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    private void ActiveDiceCombinable(int InSlotIndex)
    {
        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController == null)
            return;

        if (summonDiceMap.ContainsKey(InSlotIndex) == false)
            return;

        FBattleDice dragDice = summonDiceMap[InSlotIndex];
        ForeachSummonDice((FBattleDice InDice) => {
            if (InSlotIndex == InDice.SlotIndex)
                return;

            if (IsCombinableDice(dragDice, InDice) == false)
            {
                InDice.SetEnable(false);
            }
        });
    }

    private void DeactiveDiceCombinable()
    {
        ForeachSummonDice((FBattleDice InDice) => {
            InDice.SetEnable(true);
        });
    }

    private bool IsCombinableDice(FBattleDice InDragDice, FBattleDice InDropDice)
    {
        FDiceStatController dragDiceController = InDragDice.FindController<FDiceStatController>();
        if (dragDiceController == null)
            return false;

        FDiceStatController dropDiceController = InDropDice.FindController<FDiceStatController>();
        if (dropDiceController == null)
            return false;

        if (dragDiceController.DiceID != dropDiceController.DiceID)
            return false;

        if (dragDiceController.EyeCount != dropDiceController.EyeCount)
            return false;

        if (FBattleDataManager.Instance.MaxEyeCount == dragDiceController.EyeCount)
            return false;

        return true;
    }

    private void CombineDice(FBattleDice InDragDice, FBattleDice InDropDice)
    {
        FDiceStatController dropDiceController = InDropDice.FindController<FDiceStatController>();
        if (dropDiceController == null)
            return;

        RemoveSummonDice(InDragDice.SlotIndex);
        RemoveSummonDice(InDropDice.SlotIndex);
        SummonDiceRandomDice(InDropDice.SlotIndex, dropDiceController.EyeCount + 1);
    }

    private void SetDiceEyeTotalCount(int InDiceID, int InDelta)
    {
        FEquipBattleDice preset = FindEquipBattleDice(InDiceID);
        if (preset != null)
        {
            preset.eyeCount += InDelta;

            FBattlePanelUI battleUI = FindBattlePanelUI();
            if (battleUI != null)
            {
                battleUI.SetDiceEyeCount(preset.index, preset.eyeCount);
            }
        }
    }

    private void UpdateDiceUpgradableBySP()
    {
        FBattlePanelUI ui = FindBattlePanelUI();
        ForeachEquipBattleDice((FEquipBattleDice InDice) =>
        {
            bool prevState = InDice.IsUpgradable;
            InDice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.NOT_ENOUGH_SP, InDice.upgradeCost <= sp);
            if (prevState != InDice.IsUpgradable)
            {
                ui.SetDiceUpgradable(InDice.index, InDice.IsUpgradable);
            }
        });
    }

    private void SetDiceSummonDisableReason(DiceSummonDisableReason InReason, bool InEnable)
    {
        bool prevState = IsDiceSummonable;

        diceSummonDisableFlag = InEnable ? diceSummonDisableFlag &= ~(1 << (int)InReason) : diceSummonDisableFlag |= 1 << (int)InReason;
        if (prevState == IsDiceSummonable)
            return;

        FBattlePanelUI ui = FindBattlePanelUI();
        if (ui != null)
        {
            ui.SetDiceSummonBtnEnable(IsDiceSummonable);
        }
    }

    private FBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FBattlePanelUI>();
    }
}
