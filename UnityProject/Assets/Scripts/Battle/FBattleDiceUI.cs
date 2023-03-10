using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using FEnum;
using UnityEngine.Rendering;
using Unity.VisualScripting;
using System.Linq;

public class FBattleDiceUI : MonoBehaviour, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    static FBattleDiceUI dragItem;
    static Canvas dragParent;

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

    FLocalPlayerBattleBoardUI boardUI;
    Vector2 dragPosition;
    bool useDrag = true;
    Transform originParent;

    public int SlotIndex { get; set; }

    private void Awake()
    {
        if (dragParent == null)
        {
            dragParent = FUIManager.Instance.FindTopCanvas();
        }
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        if (useDrag == false)
            return;

        transform.SetParent(dragParent.transform, true);

        dragItem = this;
        dragPosition = eventData.position;
        boardUI.OnBegieDrag(SlotIndex);
    }

    public void OnDrag(PointerEventData eventData)
    {
        if (useDrag == false)
            return;

        Vector2 pos;
        RectTransformUtility.ScreenPointToLocalPointInRectangle(dragParent.transform as RectTransform, Input.mousePosition, dragParent.worldCamera, out pos);
        transform.position = dragParent.transform.TransformPoint(pos);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (useDrag == false)
            return;

        if (SlotIndex != dragItem.SlotIndex)
        {
            FLocalPlayerBattleController battleController = FLocalPlayer.Instance.FindController<FLocalPlayerBattleController>();
            if (battleController != null)
            {
                battleController.CombineDice(SlotIndex, dragItem.SlotIndex);
            }
        }
        
        boardUI.OnEndDrag();
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (useDrag == false)
            return;

        transform.SetParent(originParent, true);

        rectTransform.anchoredPosition = Vector2.zero;
        boardUI.OnEndDrag();
    }

    public void SetDice(int InDiceID, int InEyeCount, int InSlotIndex, bool InUseDrag = true)
    {
        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InDiceID);
        if (diceData == null)
            return;

        diceImage.gameObject.SetActive(diceData.grade != DiceGrade.DICE_GRADE_LEGEND);
        diceImageL.gameObject.SetActive(diceData.grade == DiceGrade.DICE_GRADE_LEGEND);
        if (diceData.grade != DiceGrade.DICE_GRADE_LEGEND)
            diceImage.sprite = Resources.Load<Sprite>(diceData.iconPath);
        else
            diceImageL.sprite = Resources.Load<Sprite>(diceData.iconPath);

        eyeAnimator.SetInteger("EyeCount", InEyeCount);

        SlotIndex = InSlotIndex;

        useDrag = InUseDrag;
        if (InUseDrag)
        {
            boardUI = FUIManager.Instance.FindUI<FLocalPlayerBattleBoardUI>();
            colorChanger = new FAllColorChanger(gameObject);
            originParent = transform.parent;
        }
        else
        {
            Image[] imageList = GetComponentsInChildren<Image>();
            foreach(Image image in imageList)
            {
                image.raycastTarget = false;
            }
        }
    }

    public void SetEnable(bool InEnabled)
    {
        colorChanger.SetEnable(InEnabled);
    }
}
