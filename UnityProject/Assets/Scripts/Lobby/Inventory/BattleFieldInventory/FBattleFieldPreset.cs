using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FBattleFieldPreset : MonoBehaviour
{
    [SerializeField]
    List<Button> tabList;
    [SerializeField]
    Image battleFieldImage;
    [SerializeField]
    TextMeshProUGUI battleFieldName;

    int selectedPresetIndex = 0;

    private void Start()
    {
        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if(presetController != null)
        {
            SetPreset(presetController.SelectedPresetIndex);
        }
    }

    public void SetPreset(int InPresetIndex)
    {
        UnselectTab(selectedPresetIndex);
        SelectTab(InPresetIndex);

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            int battleFieldID = presetController.GetBattleFieldPresetID(InPresetIndex);
            SetBattleFieldPreset(battleFieldID);
        }

        selectedPresetIndex = InPresetIndex;
    }

    public void SetBattleFieldPreset(int InID)
    {
        FBattleFieldData battleFieldData = FBattleFieldDataManager.Instance.FindBattleFieldData(InID);
        if (battleFieldData != null)
        {
            battleFieldName.text = battleFieldData.name;
            battleFieldImage.sprite = Resources.Load<Sprite>(battleFieldData.skinImagePath);
        }
    }

    public void OnClickTab(int InIndex)
    {
        if (selectedPresetIndex == InIndex)
            return;

        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            presetController.SetPreset(InIndex);
        }
    }

    void SelectTab(int InIndex)
    {
        tabList[InIndex].GetComponent<Animator>().SetTrigger("Selected");
    }

    void UnselectTab(int InIndex)
    {
        tabList[InIndex].GetComponent<Animator>().SetTrigger("Normal");
    }
}
