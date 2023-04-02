using FEnum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class FBattleDice : FObjectBase, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    [SerializeField]
    SpriteRenderer diceImage;
    [SerializeField]
    SpriteRenderer diceImageL;
    [SerializeField]
    Animator eyeAnimator;
    [SerializeField]
    List<Transform> eyeList;

    FAllColorChanger colorChanger;
    int slotIndex;

    public int SlotIndex { get { return slotIndex; } }
    
    public void Initialize(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        ContentID = InDiceID;
        slotIndex = InSlotIndex;

        InitUI(InEyeCount);

        AddController<FIFFController>();
        FindController<FIFFController>().IFFType = IFFType.LocalPlayer;

        AddController<FDiceStatController>();
        FindController<FDiceStatController>().Initialize(InEyeCount);

        AddController<FSkillController>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        SetEnableCollider(false);
        SetSortingOrder(9999);

        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController != null)
        {
            battleController.OnBegieDragDice(slotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnDrop(PointerEventData eventData)
    {
        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController != null)
        {
            battleController.OnDropDice(slotIndex);
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LocalPosition = Vector2.zero;
        SetEnableCollider(true);
        SetSortingOrder(0);

        FBattleDiceController battleController = FGlobal.localPlayer.FindController<FBattleDiceController>();
        if (battleController != null)
        {
            battleController.OnEndDragDice();
        }
    }

    public void SetEnable(bool InEnabled)
    {
        SetEnableCollider(InEnabled);
        colorChanger.SetEnable(InEnabled);
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

        colorChanger = new FAllColorChanger(gameObject);
    }

    private void SetEnableCollider(bool InEnable)
    {
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = InEnable;
        }
    }

    private void SetSortingOrder(int InOrder)
    {
        SortingGroup sortingGroup = GetComponent<SortingGroup>();
        if(sortingGroup != null)
        {
            sortingGroup.sortingOrder = InOrder;
        }
    }
}
