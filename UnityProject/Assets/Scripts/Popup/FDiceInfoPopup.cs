using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FDiceInfoPopup : FPopupBase
{
    [SerializeField]
    FAcquiredDiceSlot acquiredDiceSlot;
    [SerializeField]
    FNotAcquiredDiceSlot notAcquiredDiceSlot;
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI grade;
    [SerializeField]
    TextMeshProUGUI description;
    [SerializeField]
    List<FDiceStatInfo> statInfoList;
    [SerializeField]
    TextMeshProUGUI critical;
    [SerializeField]
    TextMeshProUGUI upgradeCritical;
    [SerializeField]
    TextMeshProUGUI upgradeCost;
    [SerializeField]
    Button upgradeBtn;
    [SerializeField]
    Button useBtn;

    int diceID;

    public void OpenAcquiredDiceInfo(int InID)
    {
        diceID = InID;

        FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
        if (diceController == null)
            return;

        FDice dice = diceController.FindAcquiredDice(InID);
        if (dice == null)
            return;

        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InID);
        if (diceData == null)
            return;

        upgradeBtn.gameObject.SetActive(true);
        useBtn.gameObject.SetActive(true);
        notAcquiredDiceSlot.gameObject.SetActive(false);
        acquiredDiceSlot.gameObject.SetActive(true);
        acquiredDiceSlot.Init(diceData, dice);
        
        FDiceGradeData gradeData = FDiceDataManager.Instance.FindGradeData(diceData.grade);
        if (gradeData != null)
        {
            FDiceLevelData levelData = gradeData.FindDiceLevelData(dice.level);
            if(levelData != null)
            {
                upgradeCost.text = levelData.goldCost.ToString();
                SetUpgradable(levelData.diceCountCost <= dice.count);
                SetCommonDiceInfo(diceData, gradeData);
            }
        }
    }

    public void OpenNotAcquiredDiceInfo(int InID)
    {
        diceID = InID;

        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InID);
        if (diceData == null)
            return;

        upgradeBtn.gameObject.SetActive(false);
        useBtn.gameObject.SetActive(false);
        acquiredDiceSlot.gameObject.SetActive(false);
        notAcquiredDiceSlot.gameObject.SetActive(true);
        notAcquiredDiceSlot.Init(diceData);
        SetUpgradable(false);

        FDiceGradeData gradeData = FDiceDataManager.Instance.FindGradeData(diceData.grade);
        if(gradeData != null)
            SetCommonDiceInfo(diceData, gradeData);
    }

    void SetUpgradable(bool InUpgradable)
    {
        foreach(FDiceStatInfo stat in statInfoList)
        {
            stat.Upgradable = InUpgradable;
        }

        upgradeCritical.gameObject.SetActive(InUpgradable);
        upgradeBtn.enabled = InUpgradable;
        upgradeBtn.GetComponent<Animator>().SetTrigger(InUpgradable ? "Normal" : "Disabled");
    }

    void SetCommonDiceInfo(in FDiceData InDiceData, in FDiceGradeData InGradeData)
    {
        nameText.text = InDiceData.name;
        description.text = InDiceData.description;

        FLocalPlayerStatController statController = FGlobal.localPlayer.FindController<FLocalPlayerStatController>();
        if(statController != null)
        {
            critical.text = statController.Critical + "%";
        }

        grade.text = InGradeData.gradeName;
        upgradeCritical.text = InGradeData.critical + "%";
    }

    public void OnClickUpgrade()
    {
        FDiceController diceController = FGlobal.localPlayer.FindController<FDiceController>();
        if(diceController != null)
        {
            diceController.RequestUpgradeDice(diceID);
        }
    }

    public void OnClickUse()
    {
        FDiceInventory diceInventory = FUIManager.Instance.FindUI<FDiceInventory>();
        if(diceInventory != null)
        {
            diceInventory.OpenPresetRegist(diceID);
            Close();
        }
    }

    public void OnClose()
    {
        FPopupManager.Instance.ClosePopup();
    }
}
