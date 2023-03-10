using TMPro;
using UnityEngine;
using Packet;
using System.Text.RegularExpressions;

public class FNamePopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI nameText;
    [SerializeField]
    TextMeshProUGUI errorMessage;

    string NameText { get { return nameText.text; } }
    public string ErrorMessage 
    {
        set 
        {
            errorMessage.text = value;

            Animator animator = errorMessage.GetComponent<Animator>();
            if (animator != null)
                animator.SetTrigger("Emphasize");
        } 
        get { return errorMessage.text; } 
    }

    public void OpenPopup()
    {
        ErrorMessage = "";
    }

    public void OnClickOK()
    {
        string inputText = NameText.Remove(NameText.Length - 1);
        if(inputText.Length == 0)
        {
            ErrorMessage = "이름을 입력하세요";
            return;
        }

        Match match = Regex.Match(inputText, "^[0-9a-zA-Z가-힣]+$");
        if (match.Success == false)
        {
            ErrorMessage = "특수문자는 사용할 수 없습니다";
            return;
        }

        C_CHANGE_NAME packet = new C_CHANGE_NAME();
        packet.name = inputText;

        FServerManager.Instance.SendMessage(packet);
    }
}
