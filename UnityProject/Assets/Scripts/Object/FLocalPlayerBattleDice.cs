using FEnum;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Rendering;

public class FLocalPlayerBattleDice : FObjectBase, IBeginDragHandler, IDragHandler, IDropHandler, IEndDragHandler
{
    static FLocalPlayerBattleDice dragObject;

    [SerializeField]
    SpriteRenderer diceImage;
    [SerializeField]
    SpriteRenderer diceImageL;
    [SerializeField]
    Animator eyeAnimator;
    [SerializeField]
    List<Transform> eyeList;

    FAllColorChanger colorChanger;

    public int SlotIndex { get; set; }
    
    public void Initialize(int InDiceID, int InEyeCount, int InSlotIndex)
    {
        ContentID = InDiceID;
        SlotIndex = InSlotIndex;

        InitUI(InEyeCount);

        AddController<FIFFController>();
        FindController<FIFFController>().IFFType = IFFType.LocalPlayer;

        AddController<FBattleDiceController>();
        FindController<FBattleDiceController>().Initialize(InSlotIndex, eyeList.GetRange(0, InEyeCount));

        AddController<FSkillController>();
    }

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragObject = this;
        SetEnableCollider(false);
        SetSortingOrder(9999);

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.ActiveDiceCombinable(SlotIndex);
        }
    }

    public void OnDrag(PointerEventData eventData)
    {
        WorldPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (SlotIndex == dragObject.SlotIndex)
            return;

        FBattleDiceController diceController = dragObject.FindController<FBattleDiceController>();
        if(diceController != null)
        {
            if(diceController.IsCombinable(this))
            {
                diceController.CombineDice(this);
            }
        }

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.DeactiveDiceCombinable();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        LocalPosition = Vector2.zero;
        SetEnableCollider(true);
        SetSortingOrder(0);

        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.DeactiveDiceCombinable();
        }
    }

    public void SetEnable(bool InEnabled)
    {
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
