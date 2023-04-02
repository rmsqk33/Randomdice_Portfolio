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
    TextMeshProUGUI localPlayerNickname;
    [SerializeField]
    Image localPlayerClassIcon;
    [SerializeField]
    List<FLocalPlayerBattleDicePresetSlot> localPlayerDiceSlotList;

    [SerializeField]
    TextMeshProUGUI remotePlayerNickname;
    [SerializeField]
    Image remotePlayerClassIcon;
    [SerializeField]
    List<FRemotePlayerBattleDicePresetSlot> remotePlayerDiceSlotList;

    private void Start()
    {
        Init();
    }

    public void Init()
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            SetSP(battleController.SP);
            SetDiceSummonCost(battleController.DiceSummonCost);
            SetDiceSummonBtnEnable(battleController.IsDiceSummonable);

            int i = 0;
            battleController.ForeachBattleDicePreset((FEquipBattleDice InDice) =>
            {
                if (localPlayerDiceSlotList.Count <= i)
                    return;

                FLocalPlayerBattleDicePresetSlot diceSlot = localPlayerDiceSlotList[i];
                diceSlot.SetDiceImage(InDice.diceID);
                diceSlot.SetLevel(InDice.level);
                diceSlot.SetEyeCount(InDice.eyeCount);
                diceSlot.SetUpgradeCost(InDice.upgradeCost);
                diceSlot.SetUpgradable(InDice.IsUpgradable);
                ++i;
            });

            SetTotalCard(0);
            SetCardIncrease(battleController.CardIncrease);
        }

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if(statController != null)
        {
            localPlayerNickname.text = statController.Name;
            localPlayerClassIcon.sprite = Resources.Load<Sprite>(FDataCenter.Instance.GetStringAttribute("UserClass.Class[@class=" + statController.Level + "]@icon"));
        }
    }

    public void OnClickSummonDice()
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.SummonDiceRandomSlot();
        }
    }

    public void OnClickUpgradeDice(int InIndex)
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
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
        if (localPlayerDiceSlotList.Count <= InIndex)
            return;

        localPlayerDiceSlotList[InIndex].SetLevel(InLevel);
    }

    public void SetDiceUpgradeCost(int InIndex, int InUpgradeCost)
    {
        if (localPlayerDiceSlotList.Count <= InIndex)
            return;

        localPlayerDiceSlotList[InIndex].SetUpgradeCost(InUpgradeCost);
    }

    public void SetDiceEyeCount(int InIndex, int InCount)
    {
        if (localPlayerDiceSlotList.Count <= InIndex)
            return;

        localPlayerDiceSlotList[InIndex].SetEyeCount(InCount);
    }

    public void SetDiceUpgradable(int InIndex, bool InUpgradable)
    {
        if (localPlayerDiceSlotList.Count <= InIndex)
            return;

        localPlayerDiceSlotList[InIndex].SetUpgradable(InUpgradable);
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
