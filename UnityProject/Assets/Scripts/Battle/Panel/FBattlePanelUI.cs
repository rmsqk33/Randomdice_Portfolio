using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FBattlePanelUI : FUIBase
{
    [SerializeField]
    TextMeshProUGUI sp;
    [SerializeField]
    TextMeshProUGUI diceSummonCost;
    [SerializeField]
    TextMeshProUGUI wave;
    [SerializeField]
    TextMeshProUGUI cardIncrease;
    [SerializeField]
    TextMeshProUGUI totalCard;
    [SerializeField]
    FButtonEx summonDiceBtn;
    [SerializeField]
    FWaveAlarm waveAlarm;

    [SerializeField]
    TextMeshProUGUI nickname;
    [SerializeField]
    Image classIcon;
    [SerializeField]
    List<FEquipBattleDiceSlot> diceSlotList;

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        FBattleDiceController diceController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (diceController != null)
        {
            SetSP(diceController.SP);
            SetDiceSummonCost(diceController.DiceSummonCost);
            SetDiceSummonBtnEnable(diceController.IsDiceSummonable);

            int i = 0;
            diceController.ForeachEquipBattleDice((FEquipBattleDice InDice) =>
            {
                FEquipBattleDiceSlot diceSlot = diceSlotList[i];
                diceSlot.SetDiceImage(InDice.diceID);
                diceSlot.SetLevel(InDice.level);
                diceSlot.SetEyeCount(InDice.eyeCount);
                diceSlot.SetUpgradeCost(InDice.upgradeCost);
                diceSlot.SetUpgradable(InDice.IsUpgradable);
                ++i;
            });
        }
        
        FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
        if (waveController != null)
        {
            SetTotalCard(waveController.TotalCard);
            SetCardIncrease(waveController.CardIncrease);
        }

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if(statController != null)
        {
            nickname.text = statController.Name;
            classIcon.sprite = Resources.Load<Sprite>(FDataCenter.Instance.GetStringAttribute("UserClass.Class[@class=" + statController.Level + "]@icon"));
        }
    }

    public void OnClickSummonDice()
    {
        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController != null)
        {
            battleController.SummonDiceRandomSlot();
        }
    }

    public void OnClickUpgradeDice(int InIndex)
    {
        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController != null)
        {
            battleController.DiceLevelUp(InIndex);
        }
    }

    public void SetSP(int InSP)
    {
        sp.text = InSP.ToString();
    }

    public void SetDiceSummonCost(int InCost)
    {
        diceSummonCost.text = InCost.ToString();
    }

    public void SetDiceSummonBtnEnable(bool InEnable)
    {
        summonDiceBtn.SetInteractable(InEnable);
    }

    public void SetDiceLevel(int InIndex, int InLevel)
    {
        if (InIndex < 0 || diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetLevel(InLevel);
    }

    public void SetDiceUpgradeCost(int InIndex, int InUpgradeCost)
    {
        if (InIndex < 0 || diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetUpgradeCost(InUpgradeCost);
    }

    public void SetDiceEyeCount(int InIndex, int InCount)
    {
        if (InIndex < 0 || diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetEyeCount(InCount);
    }

    public void SetDiceUpgradable(int InIndex, bool InUpgradable)
    {
        if (InIndex < 0 || diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetUpgradable(InUpgradable);
    }

    public void SetTotalCard(int InCard)
    {
        totalCard.text = InCard.ToString();
    }

    public void SetCardIncrease(int InCard)
    {
        cardIncrease.text = "+" + InCard;
    }

    public void SetWave(int InWave)
    {
        wave.text = InWave.ToString();
    }

    public void StartWaveAlarm(int InWave)
    {
        waveAlarm.Wave = InWave;
        waveAlarm.gameObject.SetActive(true);
    }

}
