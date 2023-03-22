using TMPro;
using UnityEngine;

public class FWaveAlarm : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI wave;

    public int Wave { set { wave.text = "¿þÀÌºê " + value; } }

    public void OnCompleteAnim()
    {
        FBattleController battleController = FGlobal.localPlayer.FindController<FBattleController>();
        if (battleController != null)
        {
            battleController.StartWave();
        }

        gameObject.SetActive(false);
    }
}
