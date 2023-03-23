using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class FBattleResultPopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI wave;
    [SerializeField]
    TextMeshProUGUI card;

    public void OpenPopup()
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController == null)
            return;

        wave.text = "¿þÀÌºê " + battleController.Wave;
        card.text = "+ " + battleController.TotalCard;

        Time.timeScale = 0;
    }

    public void OnClick()
    {
        Time.timeScale = 1;

        SceneManager.LoadScene("LobbyScene");
    }
}
