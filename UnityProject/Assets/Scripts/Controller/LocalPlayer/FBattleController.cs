using FEnum;
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

public class FBattleController : FControllerBase
{
    private FBattleDicePreset[] dicePresetList = new FBattleDicePreset[FGlobal.MAX_PRESET];
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
    float summonInterval;
    float waveEndCheckElapsed;
    bool startedWave;

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
                //게임 종료 처리
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
                ui.SetCardIncrease(totalCard);
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
                    FBattleDicePreset battleDice = new FBattleDicePreset(i, InDiceID);
                    battleDice.upgradeCost = levelData.cost;
                    battleDice.attackRate = levelData.attackRate;
                    battleDice.SetUpgradeDisableFlag(DiceUpgradeDisableReason.NOT_ENOUGH_SP, battleDice.upgradeCost <= SP);
                    dicePresetList[i] = battleDice;
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
        CreateEnemyProcess(InDeltaTime);
        CheckEndWaveProcess(InDeltaTime);
    }

    void CreateEnemyProcess(float InDeltaTime)
    {
        if (startedWave == false)
            return;

        if (waveData.SummonCount <= summonCount)
            return;

        summonInterval -= InDeltaTime;
        if (summonInterval <= 0)
        {
            int enemyID = waveData.GetEnemyID(summonCount);
            FObjectManager.Instance.CreateEnemy(enemyID);

            summonInterval = battleData.summonInterval;

            ++summonCount;
        }
    }

    void CheckEndWaveProcess(float InDeltaTime)
    {
        if (startedWave == false)
            return;

        if (0 < FObjectManager.Instance.EnemyCount || summonCount < waveData.SummonCount)
        {
            waveEndCheckElapsed = 0;
            return;
        }

        waveEndCheckElapsed += InDeltaTime;
        if (FBattleDataManager.Instance.WaveEndInterval <= waveEndCheckElapsed)
        {
            NextWave();
        }
    }

    public void StartBattle(int InID)
    {
        battleData = FBattleDataManager.Instance.FindBattleData(InID);
        if (battleData == null)
            return;

        wave = 0;
        NextWave();
    }

    public void NextWave()
    {
        startedWave = false;
        
        ++Wave;
        waveData = battleData.FindWaveData(Wave);

        FBattlePanelUI battleUI = FindBattlePanelUI();
        if (battleUI != null)
        {
            battleUI.StartWaveAlarm(Wave);
        }
    }

    public void StartWave()
    {
        startedWave = true;

        waveEndCheckElapsed = 0;
        summonCount = 0;
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
        int summonDiceID = dicePresetList[Random.Range(0, dicePresetList.Count())].diceID;

        CreateSummonDice(emptyDiceSlotIndexList[summonSlotIndex], summonDiceID, InEyeCount);
    }

    public void SummonDiceRandomDice(int InSlotIndex, int InEyeCount)
    {
        int summonDiceID = dicePresetList[Random.Range(0, dicePresetList.Count())].diceID;

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

    public delegate void ForeachBattleDicePresetDelegate(FBattleDicePreset InDice);
    public void ForeachBattleDicePreset(ForeachBattleDicePresetDelegate InFunc)
    {
        foreach(FBattleDicePreset dice in dicePresetList)
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
        FBattleDicePreset preset = FindBattleDicePreset(InDiceID);
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

    private FBattlePanelUI FindBattlePanelUI()
    {
        return FUIManager.Instance.FindUI<FBattlePanelUI>();
    }
}
