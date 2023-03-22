using FEnum;
using Packet;
using UnityEngine;

public class FLocalPlayerStatController : FControllerBase
{
    public int Level { get; set; }
    public int Exp { get; set; }
    public int MaxExp { get; set; }
    public int Critical { get; private set; }
    public string Name { get; set; }

    public FLocalPlayerStatController(FLocalPlayer InOwner) : base(InOwner)
    {
    }

    public void Handle_S_USER_DATA(in S_USER_DATA InPacket)
    {
        Name = InPacket.name;
        Level = InPacket.level;
        Exp = InPacket.exp;
        MaxExp = FDataCenter.Instance.GetIntAttribute("UserClass.Class[@class=" + InPacket.level + "]@exp");
        
        CalcCritical();

        FLobbyUserInfoUI userInfoUI = FindLobbyUserInfoUI();
        if (userInfoUI != null)
        {
            userInfoUI.InitUserInfo();
        }

        if(Name.Length == 0)
        {
            FPopupManager.Instance.OpenNamePopup();
        }
    }

    public void Handle_S_CAHNGE_NAME(in S_CHANGE_NAME InPacket)
    {
        ChangeNameResult result = (ChangeNameResult)InPacket.resultType;
        if (result == ChangeNameResult.CHANGE_NAME_RESULT_SUCCESS)
        {
            Name = InPacket.name;

            FLobbyUserInfoUI userInfoUI = FindLobbyUserInfoUI();
            if (userInfoUI != null)
            {
                userInfoUI.Name = Name;
            }

            FPopupManager.Instance.ClosePopup();
        }
        else
        {
            FNamePopup popup = FUIManager.Instance.FindUI<FNamePopup>();
            if (popup != null)
            {
                string errorMessage = new string("");
                switch (result)
                {
                    case ChangeNameResult.CHANGE_NAME_RESULT_ALEADY: errorMessage = "이미 있는 닉네임입니다"; break;
                    case ChangeNameResult.CHANGE_NAME_RESULT_SPECIAL_CHARACTER: errorMessage = "특수문자는 사용할 수 없습니다"; break;
                    case ChangeNameResult.CHANGE_NAME_RESULT_BLANK: errorMessage = "이름을 입력하세요"; break;
                }

                popup.ErrorMessage = errorMessage;
            }
        }
    }

    public void AddCritical(int InID, int InIncreaseLevel = 1)
    {
        FDiceGradeData data = FDiceDataManager.Instance.FindGradeDataByID(InID);
        if (data != null)
        {
            Critical += data.critical * InIncreaseLevel;

            FDiceInventory diceInventory = FindDiceInventoryUI();
            if (diceInventory != null)
            {
                diceInventory.Critical = Critical;
            }
        }
    }

    void CalcCritical()
    {
        Critical = 0;

        FDiceController diceController = FindController<FDiceController>();
        if(diceController != null)
        {
            diceController.ForeachAcquiredDice((FDice InDice) => {
                AddCritical(InDice.id, InDice.level);
            });
        }
    }

    FDiceInventory FindDiceInventoryUI()
    {
        return FUIManager.Instance.FindUI<FDiceInventory>();
    }

    FLobbyUserInfoUI FindLobbyUserInfoUI()
    {
        return FUIManager.Instance.FindUI<FLobbyUserInfoUI>();
    }
}
