using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FAcquiredDicePopupSlot : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI gradeText;
    [SerializeField]
    TextMeshProUGUI countText;
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    Image eye;

    public void SetSlot(int InDiceID, int InCount)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDiceID);
        if (diceData == null)
            return;

        FDiceGradeData diceGradeData = FDiceDataManager.Instance.FindGradeData(diceData.grade);
        if (diceGradeData == null)
            return;

        nameText.text = diceData.name;
        gradeText.text = diceGradeData.gradeName;
        countText.text = "x" + InCount.ToString();

        diceIcon.gameObject.SetActive(diceData.grade != FEnum.DiceGrade.DICE_GRADE_LEGEND);
        diceIconL.gameObject.SetActive(diceData.grade == FEnum.DiceGrade.DICE_GRADE_LEGEND);

        if (diceData.grade != FEnum.DiceGrade.DICE_GRADE_LEGEND)
            diceIcon.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceIconL.sprite = Resources.Load<Sprite>(diceData.iconPath);

        eye.color = diceData.color;
    }
}
