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
    private Dictionary<int, int> summonDiceIDMap = new Dictionary<int, int>();
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

        P2P_CHANGE_DICE_LEVEL pkt = new P2P_CHANGE_DICE_LEVEL();
        pkt.index = InIndex;
        pkt.level = dice.level;
        FServerManager.Instance.SendMessage(pkt);
    }

    public void SummonDiceRandomSlot(int InEyeCount = 1)
    {
        if (SP < DiceSummonCost)
            return;

        if (emptyDiceSlotIndexList.Count == 0)
            return;

        SP -= DiceSummonCost;
        DiceSummonCost += FBattleDataManager.Instance.DiceSummonCostIncrease;

        int summonSlotIndex = emptyDiceSlotIndexList[Random.Range(0, emptyDiceSlotIndexList.Count)];
        int summonDiceID = equipDiceList[Random.Range(0, equipDiceList.Count())].diceID;

        if (FGlobal.localPlayer.IsHost)
        {
            int objectID = FObjectManager.Instance.CreateLocalPlayerBattleDice(summonDiceID, InEyeCount, summonSlotIndex);
            AddBattleDice(objectID, summonDiceID, InEyeCount, summonSlotIndex);
        }
        else
        {
            RequestCreateDice(summonDiceID, InEyeCount, summonSlotIndex);
        }
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

        if (summonDiceIDMap.ContainsKey(dragDiceIndex) == false)
            return;

        if (summonDiceIDMap.ContainsKey(InSlotIndex) == false)
            return;

        FObjectBase dragDice = FObjectManager.Instance.FindObject(summonDiceIDMap[dragDiceIndex]);
        FObjectBase dropDice = FObjectManager.Instance.FindObject(summonDiceIDMap[InSlotIndex]);
        if (IsCombinableDice(dragDice, dropDice) == false)
            return;

        FStatController statController = dropDice.FindController<FStatController>();
        if (statController == null)
            return;

        CombineDice(dragDiceIndex, InSlotIndex, statController.GetIntStat(StatType.DiceEye) + 1);
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

    public FEquipBattleDice FindEquipBattleDice(int InDiceID)
    {
        foreach (FEquipBattleDice info in equipDiceList)
        {
            if (info.diceID == InDiceID)
                return info;
        }
        return null;
    }

    public void AddBattleDice(int InObjectID, int InDiceID, int InEyeCount, int InIndex)
    {
        summonDiceIDMap.Add(InIndex, InObjectID);
        emptyDiceSlotIndexList.Remove(InIndex);

        SetDiceEyeTotalCount(InDiceID, InEyeCount);
        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    private void SummonRandomDice(int InSlotIndex, int InEyeCount)
    {
        int summonDiceID = equipDiceList[Random.Range(0, equipDiceList.Count())].diceID;

        if (FGlobal.localPlayer.IsHost)
        {
            int objectID = FObjectManager.Instance.CreateLocalPlayerBattleDice(summonDiceID, InEyeCount, InSlotIndex);
            AddBattleDice(objectID, summonDiceID, InEyeCount, InSlotIndex);
        }
        else
        {
            RequestCreateDice(InSlotIndex, summonDiceID, InEyeCount);
        }
    }

    private void RequestCreateDice(int InDiceID, int InEyeCount, int InIndex)
    {
        emptyDiceSlotIndexList.Remove(InIndex);

        P2P_C_REQUEST_SPAWN_DICE pkt = new P2P_C_REQUEST_SPAWN_DICE();
        pkt.diceId = InDiceID;
        pkt.eyeCount = InEyeCount;
        pkt.index = InIndex;

        FServerManager.Instance.SendMessage(pkt);
    }

    private void RemoveSummonDice(int InIndex)
    {
        if (summonDiceIDMap.ContainsKey(InIndex) == false)
            return;

        int objectID = summonDiceIDMap[InIndex];
        FObjectBase dice = FObjectManager.Instance.FindObject(objectID);
        if (dice == null)
            return;

        FStatController statController = dice.FindController<FStatController>();
        if (statController == null)
            return;

        SetDiceEyeTotalCount(dice.ContentID, -statController.GetIntStat(StatType.DiceEye));

        FObjectManager.Instance.RemoveObjectAndSendP2P(objectID);
        summonDiceIDMap.Remove(InIndex);
        emptyDiceSlotIndexList.Add(InIndex);

        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    private void ActiveDiceCombinable(int InSlotIndex)
    {
        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController == null)
            return;

        if (summonDiceIDMap.ContainsKey(InSlotIndex) == false)
            return;

        FObjectBase dragDice = FObjectManager.Instance.FindObject(summonDiceIDMap[InSlotIndex]);
        foreach (var pair in summonDiceIDMap)
        {
            if (InSlotIndex == pair.Key)
                continue;

            FObjectBase dropDice = FObjectManager.Instance.FindObject(pair.Value);
            if (dropDice == null)
                continue;

            if (IsCombinableDice(dragDice, dropDice) == false)
            {
                dropDice.SetEnable(false);
            }
        }
    }

    private void DeactiveDiceCombinable()
    {
        foreach(var pair in summonDiceIDMap)
        {
            FObjectBase dice = FObjectManager.Instance.FindObject(pair.Value);
            if(dice != null)
            {
                dice.SetEnable(true);
            }
        }
    }

    private bool IsCombinableDice(FObjectBase InDragDice, FObjectBase InDropDice)
    {
        FStatController dragStatController = InDragDice.FindController<FStatController>();
        if (dragStatController == null)
            return false;

        FStatController dropStatController = InDropDice.FindController<FStatController>();
        if (dropStatController == null)
            return false;

        if (InDragDice.ContentID != InDropDice.ContentID)
            return false;

        if (dragStatController.GetIntStat(StatType.DiceEye) != dropStatController.GetIntStat(StatType.DiceEye))
            return false;

        if (FBattleDataManager.Instance.MaxEyeCount == dragStatController.GetIntStat(StatType.DiceEye))
            return false;

        return true;
    }

    private void CombineDice(int InDragIndex, int InDropIndex, int InEyeCount)
    {
        RemoveSummonDice(InDragIndex);
        RemoveSummonDice(InDropIndex);
        SummonRandomDice(InDropIndex, InEyeCount);
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
