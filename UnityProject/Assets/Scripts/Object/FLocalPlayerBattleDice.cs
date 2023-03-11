using FEnum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class FLocalPlayerBattleDice : FObjectBase, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    static int dragIndex;

    [SerializeField]
    Image diceImage;
    [SerializeField]
    Image diceImageL;
    [SerializeField]
    RectTransform rectTransform;
    [SerializeField]
    Animator eyeAnimator;
    [SerializeField]
    List<Transform> eyePositionList;

    FAllColorChanger colorChanger;
    Transform originParent;

    public int DiceID { get; set; }
    public int EyeCount { get; set; }
    public int SlotIndex { get; set; }

    private void Awake()
    {
        originParent = transform.parent;
        colorChanger = new FAllColorChanger(gameObject);
    }

    public void Initialize(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDiceID);
        if (diceData == null)
            return;

        DiceID = InDiceID;
        EyeCount = InEyeCount;
        SlotIndex = InSlotIndex;

        diceImage.gameObject.SetActive(diceData.grade != DiceGrade.DICE_GRADE_LEGEND);
        diceImageL.gameObject.SetActive(diceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        if (diceData.grade != DiceGrade.DICE_GRADE_LEGEND)
            diceImage.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceImageL.sprite = Resources.Load<Sprite>(diceData.iconPath);

        eyeAnimator.SetInteger("EyeCount", InEyeCount);
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        transform.SetParent(FUIManager.Instance.TopSiblingCanvas.transform, true);

        dragIndex = SlotIndex;

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.ActiveDiceCombinable(SlotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        Canvas topSiblingCanvas = FUIManager.Instance.TopSiblingCanvas;
        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(topSiblingCanvas.transform as RectTransform, Input.mousePosition, topSiblingCanvas.worldCamera, out pos);
        transform.position = topSiblingCanvas.transform.TransformPoint(pos);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (SlotIndex == dragIndex)
            return;

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController == null)
            return;

        battleController.CombineDice(SlotIndex, dragIndex);
        battleController.DeactiveDiceCombinable();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.SetParent(originParent, true);
        rectTransform.anchoredPosition = Vector2.zero;

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.DeactiveDiceCombinable();
        }
    }

    public void CombineDice(FLocalPlayerBattleDice InDestDice)
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.RemoveSummonDice(InDestDice.SlotIndex);
            battleController.RemoveSummonDice(SlotIndex);
            battleController.SummonDiceRandomDice(InDestDice.SlotIndex, InDestDice.EyeCount + 1);
        }
    }

    public bool IsCombinable(FLocalPlayerBattleDice InDestDice)
    {
        if (InDestDice.DiceID != DiceID)
            return false;

        if (InDestDice.EyeCount != EyeCount)
            return false;

        if (FBattleDataManager.Instance.MaxEyeCount <= EyeCount)
            return false;

        return true;
    }

    public void SetEnable(bool InEnabled)
    {
        colorChanger.SetEnable(InEnabled);
    }

}
