using System.Collections.Generic;
using System.Linq;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class FDiceInventory : FUIBase
{
    [SerializeField]
    TextMeshProUGUI criticalText;
    [SerializeField]
    Transform acquiredDiceListUI;
    [SerializeField]
    Transform notAcquiredDiceListUI;
    [SerializeField]
    FAcquiredDiceSlot acquiredDiceSlotPrefab;
    [SerializeField]
    FNotAcquiredDiceSlot notAcquiredDiceSlotPrefab;
    [SerializeField]
    FDicePresetRegist presetRegistUI;
    [SerializeField]
    ScrollRect diceScrollRect;
    [SerializeField]
    List<Button> presetTabList;
    [SerializeField]
    List<FDicePresetSlot> presetSlotList;

    int selectedDiceID = 0;
    int selectedPresetIndex = 0;
    Vector2 initScrollPos = Vector2.zero;

    Dictionary<int, FAcquiredDiceSlot> acquiredDiceMap = new Dictionary<int, FAcquiredDiceSlot>();
    Dictionary<int, FNotAcquiredDiceSlot> notAcquiredDiceMap = new Dictionary<int, FNotAcquiredDiceSlot>();

    public int Critical { set { criticalText.text = value.ToString() + "%"; } }

    private void Start()
    {
        initScrollPos = diceScrollRect.content.anchoredPosition;
        InitInventory();        
    }

    public void InitInventory()
    {
        FStatController statController = FLocalPlayer.Instance.FindController<FStatController>();
        if (statController != null)
        {
            Critical = statController.Critical;
        }

        FDiceController diceController = FLocalPlayer.Instance.FindController<FDiceController>();
        if (diceController != null)
        {
            ClearInventory();
            FDiceDataManager.Instance.ForeachDiceData((in FDiceData InData) =>
            {
                FDice acquiredDiceData = diceController.FindAcquiredDice(InData.id);
                if (acquiredDiceData != null)
                    AddAcquiredDice(acquiredDiceData);
                else
                    AddNotAcquiredDice(InData);
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
        if (presetController != null)
        {
            SetPresetTab(presetController.SelectedPresetIndex);
        }
    }

    public void OnDeactive()
    {
        SetPresetRegistActive(false);

        diceScrollRect.velocity = Vector2.zero;
        diceScrollRect.content.anchoredPosition = initScrollPos;
    }

    public void OnClickPresetRegistCancel()
    {
        SetPresetRegistActive(false);
    }

    public void OnChangeDiceInPreset(int InIndex)
    {
        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            presetController.SetDicePreset(selectedDiceID, InIndex);
        }
        SetPresetRegistActive(false);
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

    public void AcquireDice(in FDice InAcquiredDiceData)
    {
        AddAcquiredDice(InAcquiredDiceData);
        RemoveNotAcquiredDice(InAcquiredDiceData.id);
    }

    public void SetDiceCount(int InID, int InCount)
    {
        if (!acquiredDiceMap.ContainsKey(InID))
            return;

        acquiredDiceMap[InID].CurrentCount = InCount;
    }
    
    public void SetDiceMaxExp(int InID, int InMaxExp)
    {
        if (!acquiredDiceMap.ContainsKey(InID))
            return;

        acquiredDiceMap[InID].MaxCount = InMaxExp;
    }

    public void SetDiceLevel(int InID, int InLevel)
    {
        if (!acquiredDiceMap.ContainsKey(InID))
            return;

        acquiredDiceMap[InID].Level = InLevel;
    }

    public void SetPresetRegistActive(bool InActive)
    {
        diceScrollRect.gameObject.SetActive(!InActive);
        
        foreach (FDicePresetSlot slot in presetSlotList)
        {
            slot.SetPresetRegistActive(InActive);
        }

        if (InActive)
        {
            presetRegistUI.SetDice(selectedDiceID);
        }
        presetRegistUI.gameObject.SetActive(InActive);
    }

    public void SetPresetTab(int InTabIndex)
    {
        UnselectPresetTab(selectedPresetIndex);
        SelectPresetTab(InTabIndex);

        selectedPresetIndex = InTabIndex;

        int i = 0;
        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            presetController.ForeachDicePreset(InTabIndex, (int InID) =>
            {
                presetSlotList[i].SetSlot(InID);
                ++i;
            });
        }
    }

    public void SetDicePreset(int InID, int InIndex)
    {
        if (0 <= InIndex && InIndex < presetSlotList.Count)
        {
            presetSlotList[InIndex].SetSlot(InID);
        }
    }

    void SelectPresetTab(int InIndex)
    {
        if (0 <= InIndex && InIndex < presetTabList.Count)
        {
            presetTabList[InIndex].GetComponent<Animator>().SetTrigger("Selected");
        }
    }

    void UnselectPresetTab(int InIndex)
    {
        if(0 <= InIndex && InIndex < presetTabList.Count)
        {
            presetTabList[InIndex].GetComponent<Animator>().SetTrigger("Normal");
        }
    }

    void AddAcquiredDice(in FDice InAcquiredDiceData)
    {
        if (acquiredDiceMap.ContainsKey(InAcquiredDiceData.id))
            return;

        FDiceData diceData = FDiceDataManager.Instance.FindDiceData(InAcquiredDiceData.id);
        if (diceData == null)
            return;

        FAcquiredDiceSlot slot = Instantiate(acquiredDiceSlotPrefab, acquiredDiceListUI);
        slot.Init(diceData, InAcquiredDiceData);
        slot.ClickHandler = OnClickAcquiredDiceSlot;

        acquiredDiceMap.Add(slot.ID, slot);

        List<int> sortList = acquiredDiceMap.Keys.ToList();
        sortList.Sort();
        int index = sortList.IndexOf(slot.ID);
        slot.transform.SetSiblingIndex(index);
    }

    void AddNotAcquiredDice(in FDiceData InData)
    {
        if (notAcquiredDiceMap.ContainsKey(InData.id))
            return;

        FNotAcquiredDiceSlot slot = Instantiate(notAcquiredDiceSlotPrefab, notAcquiredDiceListUI);
        slot.Init(InData);
        slot.OnClickHandler = OnClickNotAcquiredDiceSlot;

        notAcquiredDiceMap.Add(InData.id, slot);
    }

    void RemoveNotAcquiredDice(in int InID)
    {
        if (notAcquiredDiceMap.ContainsKey(InID))
        {
            Destroy(notAcquiredDiceMap[InID].gameObject);
            notAcquiredDiceMap.Remove(InID);
        }
    }

    void OnClickAcquiredDiceSlot(int InID)
    {
        if (acquiredDiceMap.ContainsKey(InID))
        {
            selectedDiceID = InID;
            FPopupManager.Instance.OpenAcquiredDiceInfoPopup(InID);
        }
    }

    void OnClickNotAcquiredDiceSlot(int InID)
    {
        if (notAcquiredDiceMap.ContainsKey(InID))
        {
            FPopupManager.Instance.OpenNotAcquiredDiceInfoPopup(InID);
        }
    }

    void ClearInventory()
    {
        foreach (var iter in acquiredDiceMap)
        {
            Destroy(iter.Value.gameObject);
        }
        acquiredDiceMap.Clear();

        foreach (var iter in notAcquiredDiceMap)
        {
            Destroy(iter.Value.gameObject);
        }
        notAcquiredDiceMap.Clear();
    }
}