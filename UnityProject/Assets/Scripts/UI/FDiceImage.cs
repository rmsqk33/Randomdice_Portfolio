using FEnum;
using UnityEngine;
using UnityEngine.UI;

public class FDiceImage : MonoBehaviour
{
    [SerializeField]
    Image diceImage;
    [SerializeField]
    Image diceImageL;
    [SerializeField]
    Image diceEye;

    public void SetImage(int InDiceID)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDiceID);
        if (diceData == null)
            return;

        SetImage(diceData);
    }

    public void SetImage(in FDiceData InData)
    {
        SetImage(InData.grade, InData.iconPath, InData.color);
    }

    public void SetNotAcquiredDice(in FDiceData InData)
    {
        SetImage(InData.grade, InData.notAcquiredIconPath, InData.color);
    }

    private void SetImage(DiceGrade InGrade, string InPath, Color InColor)
    {
        diceImage.gameObject.SetActive(InGrade != DiceGrade.DICE_GRADE_LEGEND);
        diceImageL.gameObject.SetActive(InGrade == DiceGrade.DICE_GRADE_LEGEND);

        if (InGrade != FEnum.DiceGrade.DICE_GRADE_LEGEND)
            diceImage.sprite = Resources.Load<Sprite>(InPath);
        else
            diceImageL.sprite = Resources.Load<Sprite>(InPath);

        diceEye.color = InColor;
    }
}
