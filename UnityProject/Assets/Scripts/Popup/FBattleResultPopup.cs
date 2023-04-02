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
        FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
        if (waveController == null)
            return;

        wave.text = "���̺� " + waveController.Wave;
        card.text = "+ " + waveController.TotalCard;

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
