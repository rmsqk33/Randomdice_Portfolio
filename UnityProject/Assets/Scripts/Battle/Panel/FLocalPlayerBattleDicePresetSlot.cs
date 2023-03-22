using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FLocalPlayerBattleDicePresetSlot : MonoBehaviour
{
    [SerializeField]
    FDiceImage diceImage;
    [SerializeField]
    TextMeshProUGUI upgradeCost;
    [SerializeField]
    TextMeshProUGUI eyeCount;
    [SerializeField]
    TextMeshProUGUI level;
    [SerializeField]
    FButtonEx btnComponent;

    public void SetDiceImage(int InDiceID)
    {
        diceImage.SetImage(InDiceID, false);
    }

    public void SetEyeCount(int InCount)
    {
        eyeCount.text = InCount.ToString();
    }

    public void SetUpgradeCost(int InCost)
    {
        upgradeCost.text = InCost.ToString();
    }

    public void SetLevel(int InLevel)
    {
        if (InLevel < FBattleDataManager.Instance.MaxLevel)
            level.text = "LV." + InLevel;
        else
            level.text = "Max";
    }

    public void SetUpgradable(bool InUpgradable)
    {
        btnComponent.SetInteractable(InUpgradable);
    }
}
