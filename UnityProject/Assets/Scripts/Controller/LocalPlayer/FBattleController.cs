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
    public double attackRate;
    
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

public class FBattleController : FControllerBase
{
    private FEquipBattleDice[] equipDiceList = new FEquipBattleDice[FGlobal.MAX_PRESET];
    private Dictionary<int, FLocalPlayerBattleDice> summonDiceMap = new Dictionary<int, FLocalPlayerBattleDice>();
    private List<int> emptyDiceSlotIndexList = Enumerable.Range(0, FGlobal.MAX_SUMMON_DICE).ToList();

    int sp;
    int diceSummonCost;
    int diceSummonDisableFlag;
    int life;
    int totalCard;
    int cardIncrease;
    int wave;
    int summonCount;
    bool startedWave;

    FTimer enemySummonTimer = new FTimer();
    FTimer waveEndCheckTimer = new FTimer(FBattleDataManager.Instance.WaveEndInterval);
    
    FBattleData battleData;
    FWaveData waveData;

    public FBattleController(FLocalPlayer InOwner) : base(InOwner)
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
    public int Wave 
    { 
        get { return wave; }
        set
        {
            wave = value;

            FBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetWave(wave);
            }
        }
    }

    public int Life
    {
        get { return life; }
        set 
        {
            life = value;
            if(life <= 0)
            {
                EndBattle();
            }
        }
    }

    public int TotalCard
    {
        get { return totalCard; }
        set
        {
            totalCard = value;

            FBattlePanelUI ui = FindBattlePanelUI();
            if(ui != null)
            {
                ui.SetTotalCard(totalCard);
            }
        }
    }

    public int CardIncrease 
    {
        get { return cardIncrease; }
        set 
        {
            cardIncrease = value;

            FBattlePanelUI ui = FindBattlePanelUI();
            if (ui != null)
            {
                ui.SetCardIncrease(cardIncrease);
            }
        }
    }

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
                    battleDice.attackRate = levelData.attackRate;
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
            battleUI.Init();
        }

#if DEBUG
        StartBattle(1);
#endif
    }

    public override void Tick(float InDeltaTime)
    {
        if (startedWave == false)
            return;

        CreateEnemyProcess(InDeltaTime);
        CheckEndWaveProcess(InDeltaTime);
    }

    private void CreateEnemyProcess(float InDeltaTime)
    {
        if (waveData.SummonCount <= summonCount)
            return;

        if (enemySummonTimer.IsElapsedCheckTime(InDeltaTime))
        {
            int enemyID = waveData.GetEnemyID(summonCount);
            FObjectManager.Instance.CreateEnemy(enemyID);
            ++summonCount;
        }
    }

    private void CheckEndWaveProcess(float InDeltaTime)
    {
        if (summonCount < waveData.SummonCount)
            return;

        if (0 < FObjectManager.Instance.EnemyCount)
            return;

        if (waveEndCheckTimer.IsElapsedCheckTime(InDeltaTime))
        {
            StartNextWaveAlarm();
        }
    }

    public void StartBattle(int InID)
    {
        battleData = FBattleDataManager.Instance.FindBattleData(InID);
        if (battleData == null)
            return;

        enemySummonTimer.Interval = battleData.summonInterval;

        StartNextWaveAlarm();
    }

    private void EndBattle()
    {
        startedWave = false;

        FPopupManager.Instance.OpenBattleResultPopup();

        C_BATTLE_RESULT packet = new C_BATTLE_RESULT();
        packet.battleId = battleData.id;
        packet.clearWave = wave - 1;

        FServerManager.Instance.SendMessage(packet);
    }

    public void StartNextWaveAlarm()
    {
        startedWave = false;
        ++Wave;

        TotalCard += CardIncrease;
        waveData = battleData.FindWaveData(Wave);
        CardIncrease = waveData.card;

        FBattlePanelUI battleUI = FindBattlePanelUI();
        if (battleUI != null)
        {
            battleUI.StartWaveAlarm(Wave);
        }
    }

    public void StartWave()
    {
        summonCount = 0;

        enemySummonTimer.ResetElapsedTime();
        waveEndCheckTimer.ResetElapsedTime();

        startedWave = true;
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
        dice.attackRate = levelData.attackRate;
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

    public void SummonDiceRandomDice(int InSlotIndex, int InEyeCount)
    {
        int summonDiceID = equipDiceList[Random.Range(0, equipDiceList.Count())].diceID;

        CreateSummonDice(InSlotIndex, summonDiceID, InEyeCount);
    }

    public void CreateSummonDice(int InIndex, int InDiceID, int InEyeCount)
    {
        if (summonDiceMap.ContainsKey(InIndex))
            return;

        FLocalPlayerBattleDice dice = FBattleDiceCreator.Instance.CreateLocalPlayerDice(InDiceID, InEyeCount, InIndex);
        summonDiceMap.Add(InIndex, dice);
        emptyDiceSlotIndexList.Remove(InIndex);

        SetDiceEyeTotalCount(InDiceID, InEyeCount);
        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    public void RemoveSummonDice(int InIndex)
    {
        if (summonDiceMap.ContainsKey(InIndex) == false)
            return;

        FBattleDiceController diceController = summonDiceMap[InIndex].FindController<FBattleDiceController>();
        SetDiceEyeTotalCount(diceController.DiceID, -diceController.EyeCount);

        GameObject.Destroy(summonDiceMap[InIndex].gameObject);
        summonDiceMap.Remove(InIndex);
        emptyDiceSlotIndexList.Add(InIndex);

        SetDiceSummonDisableReason(DiceSummonDisableReason.NOT_EMPTY_SLOT, 0 < emptyDiceSlotIndexList.Count);
    }

    public delegate void ForeachBattleDicePresetDelegate(FEquipBattleDice InDice);
    public void ForeachBattleDicePreset(ForeachBattleDicePresetDelegate InFunc)
    {
        foreach(FEquipBattleDice dice in equipDiceList)
        {
            InFunc(dice);
        }
    }

    public delegate void ForeachSummonDiceDelegate(FLocalPlayerBattleDice InDice);
    public void ForeachSummonDice(ForeachSummonDiceDelegate InFunc)
    {
        foreach(var pair in summonDiceMap)
        {
            InFunc(pair.Value);
        }
    }

    public void ActiveDiceCombinable(int InSlotIndex)
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController == null)
            return;

        if (summonDiceMap.ContainsKey(InSlotIndex) == false)
            return;

        ForeachSummonDice((FLocalPlayerBattleDice InDice) => {
            if (InSlotIndex == InDice.SlotIndex)
                return;

            FBattleDiceController diceController = summonDiceMap[InSlotIndex].FindController<FBattleDiceController>();
            if (diceController.IsCombinable(InDice) == false)
            {
                summonDiceMap[InDice.SlotIndex].SetEnable(false);
            }
        });
    }

    public void DeactiveDiceCombinable()
    {
        ForeachSummonDice((FLocalPlayerBattleDice InDice) => {
            InDice.SetEnable(true);
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

    private void SetDiceEyeTotalCount(int InDiceID, int InDelta)
    {
        FEquipBattleDice preset = FindBattleDicePreset(InDiceID);
        if(preset != null)
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
        ForeachBattleDicePreset((FEquipBattleDice InDice) =>
        {
            bool prevState = InDice.IsUpgradable;
            InDice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.NOT_ENOUGH_SP, InDice.upgradeCost <= sp);
            if (prevState != InDice.IsUpgradable)
            {
                ui.SetDiceUpgradable(InDice.index, InDice.IsUpgradable);
            }
        });
    }

    private FEquipBattleDice FindBattleDicePreset(int InDiceID)
    {
        foreach(FEquipBattleDice info in equipDiceList)
        {
            if (info.diceID == InDiceID)
                return info;
        }
        return null;
    }

    private FBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FBattlePanelUI>();
    }
}
