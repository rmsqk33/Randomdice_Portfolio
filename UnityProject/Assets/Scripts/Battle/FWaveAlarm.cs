using TMPro;
using UnityEngine;

public class FWaveAlarm : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI wave;

    public int Wave { set { wave.text = "¿þÀÌºê " + value; } }

    public void OnCompleteAnim()
    {
        FBattleWaveController waveController = FGlobal.localPlayer.FindController<FBattleWaveController>();
        if (waveController != null)
        {
            waveController.StartWave();
        }

        gameObject.SetActive(false);
    }
}
