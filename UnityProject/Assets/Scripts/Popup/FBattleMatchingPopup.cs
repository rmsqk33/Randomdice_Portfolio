using UnityEngine;
using Packet;
using TMPro;

public class FBattleMatchingPopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI elapsedTimeText;

    int prevTime;
    FTimer timer = new FTimer();

    public void OpenPopup()
    {
        timer.Start();
     
        UpdateElapsedTimeText();
    }

    private void Update()
    {
        timer.Tick(Time.deltaTime);

        if(prevTime != timer.TotalSeconds)
        {
            UpdateElapsedTimeText();
        }
    }

    private void UpdateElapsedTimeText()
    {
        string format = "����ð� : ";
        if (0 < timer.Hours)
            format += "h�� ";

        if (0 < timer.Minutes)
            format += "m�� ";

        format += "s��";

        elapsedTimeText.text = timer.ToString(format);
    }

    public void OnClickCancel()
    {
        FServerManager.Instance.StopP2PServer();

        C_BATTLE_MATCHING_CANCEL packet = new C_BATTLE_MATCHING_CANCEL();
        FServerManager.Instance.SendMessage(packet);

        Close();
    }
}
