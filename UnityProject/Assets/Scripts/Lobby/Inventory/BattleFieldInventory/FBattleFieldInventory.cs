using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class FBattleFieldInventory : FInventoryBase
{
    [SerializeField]
    private FBattleFieldPreset battleFieldPreset;
    [SerializeField]
    ScrollRect battleFieldScrollRect;
    [SerializeField]
    Transform acquiredBattleFieldListUI;
    [SerializeField]
    Transform notAcquiredBattleFieldListUI;
    [SerializeField]
    FBattleFieldSlot battleFieldPrefab;

    Vector2 initScrollPos;

    Dictionary<int, FBattleFieldSlot> acquiredBattleFieldMap = new Dictionary<int, FBattleFieldSlot>();
    Dictionary<int, FBattleFieldSlot> notAcquiredBattleFieldMap = new Dictionary<int, FBattleFieldSlot>();

    private void Start()
    {
        initScrollPos = battleFieldScrollRect.content.anchoredPosition;
        InitBattleFieldList();
    }

    public override void OnActive()
    {
        base.OnActive();

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if(presetController != null)
        {
            battleFieldPreset.SetPreset(presetController.SelectedPresetIndex);
        }
    }

    public override void OnDeactive()
    {
        base.OnDeactive();

        battleFieldScrollRect.velocity = Vector2.zero;
        battleFieldScrollRect.content.anchoredPosition = initScrollPos;
    }

    public void OnClickAcquiredBattleFieldSlot(int InID)
    {
        if(acquiredBattleFieldMap.ContainsKey(InID))
        {
            FPopupManager.Instance.OpenAcquiredBattleFieldInfoPopup(InID);
        }
    }

    public void OnClickNotAcquiredBattleFieldSlot(int InID)
    {
        if(notAcquiredBattleFieldMap.ContainsKey(InID))
        {
            FPopupManager.Instance.OpenNotAcquiredBattleFieldInfoPopup(InID);
        }
    }

    public void OnPurchaseBattleField()
    {

    }

    public void InitBattleFieldList()
    {
        FBattlefieldController battlefieldController = FLocalPlayer.Instance.FindController<FBattlefieldController>();
        if (battlefieldController != null)
        {
            FBattleFieldDataManager.Instance.ForeachBattleFieldData((FBattleFieldData InData) =>
            {
                if (battlefieldController.IsAcquiredBattleField(InData.id))
                    AddAcquiredBattleField(InData);
                else
                    AddNotAcquiredBattleField(InData);
            });
        }
    }

    private void AddAcquiredBattleField(FBattleFieldData InData)
    {
        if (acquiredBattleFieldMap.ContainsKey(InData.id))
            return;

        FBattleFieldSlot slot = Instantiate(battleFieldPrefab, acquiredBattleFieldListUI);
        slot.Init(InData);
        slot.GetComponent<Button>().onClick.AddListener(() => { OnClickAcquiredBattleFieldSlot(InData.id); });

        acquiredBattleFieldMap.Add(InData.id, slot);

        List<int> sortList = acquiredBattleFieldMap.Keys.ToList();
        int index = sortList.IndexOf(InData.id);
        slot.transform.SetSiblingIndex(index);
    }

    private void AddNotAcquiredBattleField(FBattleFieldData InData)
    {
        if (notAcquiredBattleFieldMap.ContainsKey(InData.id))
            return;

        FBattleFieldSlot slot = Instantiate(battleFieldPrefab, notAcquiredBattleFieldListUI);
        slot.Init(InData);
        slot.GetComponent<Button>().onClick.AddListener(() => { OnClickNotAcquiredBattleFieldSlot(InData.id); });

        notAcquiredBattleFieldMap.Add(InData.id, slot);
    }

    private void RemoveNotAcquiredBattleField(in int InID)
    {
        if (notAcquiredBattleFieldMap.ContainsKey(InID))
        {
            Destroy(notAcquiredBattleFieldMap[InID]);
            notAcquiredBattleFieldMap.Remove(InID);
        }
    }
}
