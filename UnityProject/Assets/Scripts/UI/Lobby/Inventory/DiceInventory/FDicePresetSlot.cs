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

    public void SetSlot(int InDiceID)
    {
        FDiceController diceController = FLocalPlayer.Instance.FindController<FDiceController>();
        if (diceController == null)
            return;

        FDice dice = diceController.FindAcquiredDice(InDiceID);
        if (dice == null)
            return;

        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDiceID);
        if (diceData == null)
            return;

        diceIconL.gameObject.SetActive(diceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        diceIcon.gameObject.SetActive(diceData.grade != DiceGrade.DICE_GRADE_LEGEND);
        if (diceIconL.IsActive())
            diceIconL.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceIcon.sprite = Resources.Load<Sprite>(diceData.iconPath);

        level.text = dice.level.ToString();
    }


    public void SetPresetRegistActive(bool InActive)
    {
        presetRegistGuideArrow.gameObject.SetActive(InActive);
        GetComponent<Button>().interactable = InActive;
    }
}
