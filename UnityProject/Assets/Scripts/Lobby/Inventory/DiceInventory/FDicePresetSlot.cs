using FEnum;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FDicePresetSlot : MonoBehaviour
{
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    Image presetRegistGuideArrow;

    public void SetSlot(in FDice InSlot)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InSlot.id);
        if (diceData == null)
            return;

        diceIconL.gameObject.SetActive(diceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        diceIcon.gameObject.SetActive(diceData.grade != DiceGrade.DICE_GRADE_LEGEND);
        if (diceIconL.IsActive())
            diceIconL.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceIcon.sprite = Resources.Load<Sprite>(diceData.iconPath);

        level.text = InSlot.level.ToString();
    }


    public void SetPresetRegistActive(bool InActive)
    {
        presetRegistGuideArrow.gameObject.SetActive(InActive);
        GetComponent<Button>().interactable = InActive;
    }
}
