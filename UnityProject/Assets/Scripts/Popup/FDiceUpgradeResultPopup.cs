using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FDiceUpgradeResultPopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI diceName;
    [SerializeField]
    TextMeshProUGUI diceGrade;
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    Image diceEye;
    [SerializeField]
    TextMeshProUGUI diceClass;
    [SerializeField]
    TextMeshProUGUI currentCritical;
    [SerializeField]
    TextMeshProUGUI increaseCritical;

    public void OpenPopup(FDice InDice)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDice.id);
        if (diceData == null)
            return;

        FDiceGradeData gradeData = FDiceDataManager.Instance.FindGradeData(diceData.grade);
        if (gradeData == null)
            return;

        FStatController statController = FLocalPlayer.Instance.FindController<FStatController>();
        if (statController == null)
            return;

        diceName.text = diceData.name;
        diceGrade.text = gradeData.gradeName;

        diceIcon.gameObject.SetActive(diceData.grade != FEnum.DiceGrade.DICE_GRADE_LEGEND);
        diceIconL.gameObject.SetActive(diceData.grade == FEnum.DiceGrade.DICE_GRADE_LEGEND);

        if (diceData.grade != FEnum.DiceGrade.DICE_GRADE_LEGEND)
            diceIcon.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceIconL.sprite = Resources.Load<Sprite>(diceData.iconPath);

        diceEye.color = diceData.color;
        diceClass.text = "Å¬·¡½º " + InDice.level;

        currentCritical.text = (statController.Critical - gradeData.critical) + "%";
        increaseCritical.text = "+ " + gradeData.critical + "%";
    }
}
