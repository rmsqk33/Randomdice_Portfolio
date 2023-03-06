using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FBattleFieldInfoPopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    Image battleFieldImage;

    int diceID;
    
    public void OpenAcquiredBattleFieldInfo(int InID)
    {
        diceID = InID;

        FBattleFieldData data = FBattleFieldDataManager.Instance.FindBattleFieldData(InID);
        if (data == null)
            return;

        nameText.text = data.name;
        battleFieldImage.sprite = Resources.Load<Sprite>(data.skinImagePath);
    }

    public void OpenNotAcquiredBattleFieldInfo(int InID)
    {
        diceID = InID;

        FBattleFieldData data = FBattleFieldDataManager.Instance.FindBattleFieldData(InID);
        if (data == null)
            return;

        nameText.text = data.name;
        battleFieldImage.sprite = Resources.Load<Sprite>(data.skinImagePath);
    }

    public void OnClickUse()
    {
        FPresetController presetController = FLocalPlayer.Instance.FindController<FPresetController>();
        if (presetController != null)
        {
            presetController.SetBattleFieldPreset(diceID);
        }
        Close();
    }

    public void OnClickPurchase()
    {
    }

    public void OnClose()
    {
        FPopupManager.Instance.ClosePopup();
    }
}
