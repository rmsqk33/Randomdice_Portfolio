using UnityEngine;

public class FInventoryMenu : FLobbyScrollMenuBase
{
    [SerializeField]
    private FInventoryTabUI tabUI;
    [SerializeField]
    int initTabIndex = 0;

    public override void OnActive()
    {
        if (tabUI.SelectedTabIndex != initTabIndex)
            tabUI.SetSelectedTab(initTabIndex);

        tabUI.GetSelectedTab().OnActive();
    }

    public override void OnDeactive()
    {
        tabUI.GetSelectedTab().OnDeactive();
    }
}
