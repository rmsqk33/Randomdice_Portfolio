using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FLocalPlayerBattlePanelUI : FUIBase
{
    [SerializeField]
    TextMeshProUGUI sp;
    [SerializeField]
    TextMeshProUGUI diceSummonCost;
    [SerializeField]
    TextMeshProUGUI nickname;
    [SerializeField]
    TextMeshProUGUI wave;
    [SerializeField]
    TextMeshProUGUI cardIncrease;
    [SerializeField]
    TextMeshProUGUI cardTotal;
    [SerializeField]
    Image classIcon;
    [SerializeField]
    FButtonEx summonDiceBtn;
    [SerializeField]
    List<FLocalPlayerBattleDicePresetSlot> diceSlotList;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        FLocalPlayerBattleController battleController = FLocalPlayer.Instance.FindController<FLocalPlayerBattleController>();
        if (battleController != null)
        {
            SetSP(battleController.SP);
            SetDiceSummonCost(battleController.DiceSummonCost);
            SetDiceSummonBtnEnable(battleController.IsDiceSummonable);

            int i = 0;
            battleController.ForeachBattleDicePreset((FBattleDicePreset InDice) =>
            {
                if (diceSlotList.Count <= i)
                    return;

                FLocalPlayerBattleDicePresetSlot diceSlot = diceSlotList[i];
                diceSlot.SetDiceImage(InDice.diceID);
                diceSlot.SetLevel(InDice.level);
                diceSlot.SetEyeCount(InDice.eyeCount);
                diceSlot.SetUpgradeCost(InDice.upgradeCost);
                diceSlot.SetUpgradable(InDice.IsUpgradable);
                ++i;
            });

            SetWave(1);
            SetCardTotal(0);
            SetCardIncrease(battleController.CardIncrease);
        }

        FStatController statController = FLocalPlayer.Instance.FindController<FStatController>();
        if(statController != null)
        {
            nickname.text = statController.Name;
            classIcon.sprite = Resources.Load<Sprite>(FDataCenter.Instance.GetStringAttribute("UserClass.Class[@class=" + statController.Level + "]@icon"));
        }
    }

    public void OnClickSummonDice()
    {
        FLocalPlayerBattleController battleController = FLocalPlayer.Instance.FindController<FLocalPlayerBattleController>();
        if (battleController != null)
        {
            battleController.SummonDiceRandomSlot();
        }
    }

    public void OnClickUpgradeDice(int InIndex)
    {
        FLocalPlayerBattleController battleController = FLocalPlayer.Instance.FindController<FLocalPlayerBattleController>();
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
        if (diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetLevel(InLevel);
    }

    public void SetDiceUpgradeCost(int InIndex, int InUpgradeCost)
    {
        if (diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetUpgradeCost(InUpgradeCost);
    }

    public void SetDiceEyeCount(int InIndex, int InCount)
    {
        if (diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetEyeCount(InCount);
    }

    public void SetDiceUpgradable(int InIndex, bool InUpgradable)
    {
        if (diceSlotList.Count <= InIndex)
            return;

        diceSlotList[InIndex].SetUpgradable(InUpgradable);
    }

    public void SetCardTotal(int InCard)
    {
        cardTotal.text = InCard.ToString();
    }

    public void SetCardIncrease(int InCard)
    {
        cardIncrease.text = "+" + InCard;
    }

    public void SetWave(int InWave)
    {
        wave.text = "¿þÀÌºê " + InWave;
    }
}
