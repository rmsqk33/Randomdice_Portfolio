using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FBattleFieldInventory : FUIBase
{
    [SerializeField]
    ScrollRect battleFieldScrollRect;
    [SerializeField]
    Transform acquiredBattleFieldListUI;
    [SerializeField]
    Transform notAcquiredBattleFieldListUI;
    [SerializeField]
    FBattleFieldSlot battleFieldPrefab;
    [SerializeField]
    List<Button> presetTabList;
    [SerializeField]
    Image registedBattlefieldImage;
    [SerializeField]
    TextMeshProUGUI registedBattlefieldName;

    int selectedPresetIndex = 0;

    Vector2 initScrollPos;

    Dictionary<int, FBattleFieldSlot> acquiredBattleFieldMap = new Dictionary<int, FBattleFieldSlot>();
    Dictionary<int, FBattleFieldSlot> notAcquiredBattleFieldMap = new Dictionary<int, FBattleFieldSlot>();

    private void Start()
    {
        initScrollPos = battleFieldScrollRect.content.anchoredPosition;
        InitInventory();
    }

    public void InitInventory()
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

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            SetPresetTab(presetController.SelectedPresetIndex);
        }
    }

    public void OnEnable()
    {
        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if(presetController != null)
        {
            SetPresetTab(presetController.SelectedPresetIndex);
        }
    }

    public void OnDeactive()
    {
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

    public void OnClickPresetTab(int InIndex)
    {
        if (selectedPresetIndex == InIndex)
            return;

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            presetController.SetPreset(InIndex);
        }
    }

    public void SetPresetTab(int InTabIndex)
    {
        UnselectTab(selectedPresetIndex);
        SelectTab(InTabIndex);

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            int battleFieldID = presetController.GetBattleFieldPresetID(InTabIndex);
            SetBattleFieldPreset(battleFieldID);
        }

        selectedPresetIndex = InTabIndex;
    }

    public void SetBattleFieldPreset(int InID)
    {
        FBattleFieldData battleFieldData = FBattleFieldDataManager.Instance.FindBattleFieldData(InID);
        if (battleFieldData != null)
        {
            registedBattlefieldName.text = battleFieldData.name;
            registedBattlefieldImage.sprite = Resources.Load<Sprite>(battleFieldData.skinImagePath);
        }
    }

    void SelectTab(int InIndex)
    {
        if(0 <= presetTabList.Count && InIndex < presetTabList.Count)
        {
            presetTabList[InIndex].GetComponent<Animator>().SetTrigger("Selected");
        }
    }

    void UnselectTab(int InIndex)
    {
        if (0 <= presetTabList.Count && InIndex < presetTabList.Count)
        {
            presetTabList[InIndex].GetComponent<Animator>().SetTrigger("Normal");
        }
    }

    void AddAcquiredBattleField(FBattleFieldData InData)
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

    void AddNotAcquiredBattleField(FBattleFieldData InData)
    {
        if (notAcquiredBattleFieldMap.ContainsKey(InData.id))
            return;

        FBattleFieldSlot slot = Instantiate(battleFieldPrefab, notAcquiredBattleFieldListUI);
        slot.Init(InData);
        slot.GetComponent<Button>().onClick.AddListener(() => { OnClickNotAcquiredBattleFieldSlot(InData.id); });

        notAcquiredBattleFieldMap.Add(InData.id, slot);
    }

    void RemoveNotAcquiredBattleField(in int InID)
    {
        if (notAcquiredBattleFieldMap.ContainsKey(InID))
        {
            Destroy(notAcquiredBattleFieldMap[InID]);
            notAcquiredBattleFieldMap.Remove(InID);
        }
    }
}
