using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FRemotePlayerBattlePanelUI : FUIBase
{
    [SerializeField]
    TextMeshProUGUI nickname;
    [SerializeField]
    Image classIcon;
    [SerializeField]
    List<FRemotePlayerBattleDicePresetSlot> diceSlotList;

    public void SetClassIcon(string InPath)
    {
        classIcon.sprite = Resources.Load<Sprite>(InPath);
    }

    public void SetNickName(string InName)
    {
        nickname.text = InName;
    }
}
