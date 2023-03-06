using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class FPopupManager : FNonObjectSingleton<FPopupManager>
{
    class CanvasSiblingComparer : IComparer<Canvas>
    {
        public int Compare(Canvas x, Canvas y)
        {
            return x.transform.GetSiblingIndex() - y.transform.GetSiblingIndex();
        }
    }


    FPopupBase openedPopup;
    Transform popupParent;

    public void OpenMsgPopup(in string InTitle, in string InMsg, FMsgPopup.OKButtonFunc InFunc = null)
    {
        FMsgPopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/MsgPopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FMsgPopup>();

        popup.Title = InTitle;
        popup.Message = InMsg;
        popup.OKButtonHandler = InFunc;
    }

    public void OpenAcquiredDiceInfoPopup(int InID)
    {
        FDiceInfoPopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/DiceInfo/DiceInfoPopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FDiceInfoPopup>();

        popup.OpenAcquiredDiceInfo(InID);
    }

    public void OpenNotAcquiredDiceInfoPopup(int InID)
    {
        FDiceInfoPopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/DiceInfo/DiceInfoPopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FDiceInfoPopup>();

        popup.OpenNotAcquiredDiceInfo(InID);
    }

    public void OpenBattleFieldInfoPopup(int InID)
    {
        FBattleFieldInfoPopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/BattleFieldInfoPopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FBattleFieldInfoPopup>();

        popup.OpenPopup(InID);
    }

    public void OpenDicePurchasePopup(int InID)
    {
        FDicePurchasePopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/DicePurchasePopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FDicePurchasePopup>();

        popup.OpenPopup(InID);
    }

    public void OpenBoxPurchasePopup(int InID)
    {
        FBoxPurchasePopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/BoxPurchase/BoxPurchasePopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FBoxPurchasePopup>();

        popup.OpenPopup(InID);
    }

    public void OpenAcquiredDicePopup(List<KeyValuePair<int, int>> InDiceList)
    {
        FAcquiredDicePopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/AcquiredDice/AcquiredDicePopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FAcquiredDicePopup>();

        popup.OpenPopup(InDiceList);
    }

    public void OpenDiceUpgradeResultPopup(in FDice InDice)
    {
        FDiceUpgradeResultPopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/DiceUpgradeResultPopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FDiceUpgradeResultPopup>();

        popup.OpenPopup(InDice);
    }

    public void OpenAcquiredBattlefieldPopup(int InID)
    {
        FAcquiredBattlefieldPopup popup = null;
        GameObject gameObject = CreatePopup("Prefabs/Popup/AcquiredBattlefieldPopup");
        if (gameObject != null)
            popup = gameObject.GetComponent<FAcquiredBattlefieldPopup>();

        popup.OpenPopup(InID);
    }

    public void ClosePopup()
    {
        if(openedPopup != null)
        {
            openedPopup.Close();
            openedPopup = null;
        }
    }

    GameObject CreatePopup(in string InPath)
    {
        if (openedPopup != null)
            GameObject.Destroy(openedPopup.gameObject);

        GameObject popup = Resources.Load<GameObject>(InPath);
        if (popup == null)
            return null;

        if (popupParent == null)
        {
            Canvas[] canvasArray = GameObject.FindObjectsOfType<Canvas>();
            if (canvasArray.Length == 0)
            {
                GameObject.Destroy(popup);
                return null;
            }

            List<Canvas> canvasList = canvasArray.ToList<Canvas>();
            canvasList.Sort(new CanvasSiblingComparer());

            popupParent = canvasList[canvasList.Count - 1].transform;
        }

        popup = GameObject.Instantiate(popup, popupParent);
        openedPopup = popup.GetComponent<FPopupBase>();
        
        return popup;
    }
}
