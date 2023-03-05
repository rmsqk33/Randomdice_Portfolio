using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FLocalPlayerBattleUI : MonoBehaviour
{
    [SerializeField]
    Image diceIcon;
    [SerializeField]
    Image diceIconL;
    [SerializeField]
    TextMeshProUGUI upgradeCost;
    [SerializeField]
    TextMeshProUGUI eyeCount;
    [SerializeField]
    TextMeshProUGUI level;

    public int Level { set { level.text = "LV." + value; } }
    public int EyeCount { set { eyeCount.text = value.ToString(); } }
    public int UpgradeCost { set { upgradeCost.text = value.ToString(); } }

    public void Init(int InDiceID)
    {
        Level = 1;
        EyeCount = 0;
    }
}
