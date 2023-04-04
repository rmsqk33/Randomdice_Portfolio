using FEnum;
using System.Collections.Generic;
using UnityEngine;

public class FRemotePlayerBattleDice : FObjectBase
{
    [SerializeField]
    SpriteRenderer diceImage;
    [SerializeField]
    SpriteRenderer diceImageL;
    [SerializeField]
    Animator eyeAnimator;
    [SerializeField]
    List<Transform> eyeList;

    public void Initialize(int InDiceID, int InEyeCount)
    {
        ContentID = InDiceID;

        InitUI(InEyeCount);

        AddController<FIFFController>();
        FindController<FIFFController>().IFFType = IFFType.RemotePlayer;

        AddController<FDiceStatController>();
        FindController<FDiceStatController>().Initialize(InEyeCount);

        AddController<FSkillController>();
    }

    private void InitUI(int InEyeCount)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(ContentID);
        if (diceData == null)
            return;

        diceImage.gameObject.SetActive(diceData.grade != DiceGrade.DICE_GRADE_LEGEND);
        diceImageL.gameObject.SetActive(diceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        if (diceData.grade != DiceGrade.DICE_GRADE_LEGEND)
            diceImage.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceImageL.sprite = Resources.Load<Sprite>(diceData.iconPath);

        eyeAnimator.SetInteger("EyeCount", InEyeCount);
        for (int i = 0; i < InEyeCount; ++i)
        {
            SpriteRenderer sprite = eyeList[i].GetComponentInChildren<SpriteRenderer>(true);
            sprite.color = diceData.color;
        }
    }
}
