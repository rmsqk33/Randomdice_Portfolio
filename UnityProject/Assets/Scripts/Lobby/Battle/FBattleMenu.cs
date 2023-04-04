using Packet;
using TMPro;
using UnityEngine;

public class FBattleMenu : FLobbyScrollMenuBase
{
    [SerializeField]
    TextMeshProUGUI card = null;

    public int Card { set { card.text = value.ToString(); } }

    private void Start()
    {
        Initialize();
    }

    public void Initialize()
    {
        FInventoryController inventoryController = FGlobal.localPlayer.FindController<FInventoryController>();
        if (inventoryController != null)
        {
            Card = inventoryController.Card;
        }
    }

    public void OnClickOpenBox()
    {

    }

    public void OnClickCoopBattleMatching()
    {
        FMatchingMananger.Instance.RequestMatching();
    }
}
