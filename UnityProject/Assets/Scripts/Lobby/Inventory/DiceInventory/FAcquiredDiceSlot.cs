using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using FEnum;

public class FAcquiredDiceSlot : MonoBehaviour
{
    [SerializeField]
    Image background;
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    Image diceEye;
    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    Image expGauge;
    [SerializeField]
    TextMeshProUGUI exp;
    [SerializeField]
    Image levelUpIcon;

    int currentCount = 1;
    int maxCount = 1;

    public delegate void ClickHandlerDelicate(int InID);
    ClickHandlerDelicate clickHandler;

    public int Level { set { level.text = value.ToString(); } }
    public int ID { get; set; }
    public int CurrentCount
    {
        set
        {
            currentCount = value;
            UpdateCount();
        }
    }

    public int MaxCount
    {
        set
        {
            maxCount = value;
            UpdateCount();
        }
    }
    
    public ClickHandlerDelicate ClickHandler { set { clickHandler = value; } }

    public void Init(in FDiceData InDiceData, in FDice InDice)
    {
        ID = InDice.id;
        Level = InDice.level;
        diceEye.color = InDiceData.color;
     
        diceIconL.gameObject.SetActive(InDiceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        diceIcon.gameObject.SetActive(InDiceData.grade != DiceGrade.DICE_GRADE_LEGEND);

        if (diceIconL.IsActive())
            diceIconL.sprite = Resources.Load<Sprite>(InDiceData.iconPath);
        else
            diceIcon.sprite = Resources.Load<Sprite>(InDiceData.iconPath);

        FDiceGradeData gradeData = FDiceDataManager.Instance.FindGradeData(InDiceData.grade);
        if (gradeData != null)
        {
            background.sprite = Resources.Load<Sprite>(gradeData.backgroundPath);

            FDiceLevelData levelData = gradeData.FindDiceLevelData(InDice.level);
            if (levelData != null)
            {
                currentCount = InDice.count;
                maxCount = levelData.diceCountCost;
                UpdateCount();
            }
        }
    }

    void UpdateCount()
    {
        Vector3 scale = expGauge.transform.localScale;
        scale.x = Mathf.Min((float)currentCount / (float)maxCount, 1);
        expGauge.transform.localScale = scale;

        exp.text = currentCount.ToString() + "/" + maxCount.ToString();

        levelUpIcon.gameObject.SetActive(maxCount <= currentCount);
    }

    public void OnClickSlot()
    {
        clickHandler(ID);
    }
}
