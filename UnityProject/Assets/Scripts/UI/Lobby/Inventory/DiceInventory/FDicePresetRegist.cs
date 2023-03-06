using UnityEngine;
using UnityEngine.UI;
using FEnum;

public class FDicePresetRegist : MonoBehaviour
{
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    Image diceEye;

    public void SetDice(int InDiceID)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDiceID);
        if (diceData == null)
            return;

        diceIcon.gameObject.SetActive(diceData.grade != DiceGrade.DICE_GRADE_LEGEND);
        diceIconL.gameObject.SetActive(diceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        if (diceData.grade != DiceGrade.DICE_GRADE_LEGEND)
            diceIcon.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceIconL.sprite = Resources.Load<Sprite>(diceData.iconPath);

        diceEye.color = diceData.color;
    }
}
