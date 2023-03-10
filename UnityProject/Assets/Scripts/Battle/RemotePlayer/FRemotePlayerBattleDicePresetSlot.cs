using TMPro;
using UnityEngine;

public class FRemotePlayerBattleDicePresetSlot : MonoBehaviour
{
    [SerializeField]
    FDiceImage diceImage;
    [SerializeField]
    TextMeshProUGUI level;

    public void SetDice(int InDiceID)
    {
        diceImage.SetImage(InDiceID, false);
    }

    public void SetLevel(int InLevel)
    {
        if(FBattleDataManager.Instance.MaxLevel <= InLevel)
            level.text = "Max";
        else
            level.text = "LV." + InLevel;
    }
}
