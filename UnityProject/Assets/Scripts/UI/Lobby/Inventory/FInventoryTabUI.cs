using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FInventoryTabUI : MonoBehaviour
{
    [SerializeField]
    List<Button> tabButtonList;
    [SerializeField]
    List<FInventoryBase> inventoryList;

    public int SelectedTabIndex { get; set; } = -1;

    private void Awake()
    {
        for (int i = 0; i < tabButtonList.Count; ++i)
        {
            int index = i;
            tabButtonList[i].onClick.AddListener( () => { OnClickTab(index); });
        }
    }

    public void OnClickTab(int InIndex)
    {
        if (SelectedTabIndex == InIndex)
            return;

        SetSelectedTab(InIndex);
    }

    public void SetSelectedTab(int InIndex)
    {
        if (InIndex < 0 || tabButtonList.Count <= InIndex)
            return;

        if (SelectedTabIndex != -1)
        {
            DeactiveTab(SelectedTabIndex);
        }

        SelectedTabIndex = InIndex;
        ActiveTab(InIndex);
    }

    public FInventoryBase GetSelectedTab()
    {
        return inventoryList[SelectedTabIndex];
    }

    void ActiveTab(int InIndex)
    {
        tabButtonList[InIndex].GetComponent<Animator>().SetTrigger("Selected");
        inventoryList[InIndex].gameObject.SetActive(true);
        inventoryList[InIndex].OnActive();
    }

    void DeactiveTab(int InIndex)
    {
        tabButtonList[InIndex].GetComponent<Animator>().SetTrigger("Normal");
        inventoryList[InIndex].OnDeactive();
        inventoryList[InIndex].gameObject.SetActive(false);
    }
}
