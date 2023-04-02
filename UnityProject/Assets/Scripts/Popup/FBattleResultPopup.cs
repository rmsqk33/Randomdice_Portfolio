using FEnum;
using TMPro;
using UnityEngine;

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
        FSceneManager.Instance.ChangeSceneAfterLoading(SceneType.Lobby);
    }

    private void OnDestroy()
    {
        Time.timeScale = 1;
    }
}
