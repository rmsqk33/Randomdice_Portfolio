using UnityEngine;
using Packet;
using TMPro;

public class FBattleMatchingPopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI elapsedTimeText;

    int prevTime = 0;
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
        FMatchingMananger.Instance.CancelMatching();
    }
}
