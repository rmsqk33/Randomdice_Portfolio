using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FEnum;

public class FNotAcquiredDiceSlot : MonoBehaviour
{
    [SerializeField]
    Image background;
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    TextMeshProUGUI gradeText;

    public int ID { get; set; }

    public delegate void ClickHandler(int InID);
    ClickHandler clickHandler;
    public ClickHandler OnClickHandler { set { clickHandler = value; } }

    public void Init(in FDiceData InData)
    {
        ID = InData.id;
        diceIconL.gameObject.SetActive(InData.grade == DiceGrade.DICE_GRADE_LEGEND);
        diceIcon.gameObject.SetActive(InData.grade != DiceGrade.DICE_GRADE_LEGEND);

        if (diceIconL.IsActive())
            diceIconL.sprite = Resources.Load<Sprite>(InData.notAcquiredIconPath);
        else
            diceIcon.sprite = Resources.Load<Sprite>(InData.notAcquiredIconPath);

        FDiceGradeData gradeData = FDiceDataManager.Instance.FindGradeData(InData.grade);
        if (gradeData != null)
        {
            background.sprite = Resources.Load<Sprite>(gradeData.backgroundPath);
            gradeText.text = gradeData.gradeName;
        }
    }

    public void OnClickSlot()
    {
        clickHandler(ID);
    }
}
