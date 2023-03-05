using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class FBoxPurchasePopup : FPopupBase
{
    [SerializeField]
    TextMeshProUGUI title;
    [SerializeField]
    TextMeshProUGUI price;
    [SerializeField]
    Image boxImage;
    [SerializeField]
    List<FBoxGoods> goodsList;
    [SerializeField]
    List<Transform> lineList;

    private int boxID;

    public void OpenPopup(int InID)
    {
        boxID = InID;

        FStoreBoxData boxData = FStoreDataManager.Instance.FindStoreBoxData(InID);
        title.text = boxData.name;
        price.text = "x " + boxData.price;
        boxImage.sprite = Resources.Load<Sprite>(boxData.boxImagePath);

        goodsList[0].Count = "x" + boxData.gold;

        int i = 1;
        boxData.ForeachGoodsData((FBoxGoodsData InData) => {
            FBoxGoods boxGoods = goodsList[i];
            boxGoods.gameObject.SetActive(true);
            boxGoods.GoodsIcon = Resources.Load<Sprite>(FStoreDataManager.Instance.GetBoxGoodsImage(InData.grade));
            boxGoods.Count = "x " + (InData.min == InData.max ? InData.min : InData.min + " ~ " + InData.max);

            FDiceGradeData diceGradeData = FDiceDataManager.Instance.FindGradeData(InData.grade);
            if (diceGradeData != null)
            {
                boxGoods.GoodsName = diceGradeData.gradeName;
            }

            if (i % 2 == 0)
            {
                int lineIndex = i / 2 - 1;
                lineList[lineIndex].gameObject.SetActive(true);
                boxGoods.transform.parent.gameObject.SetActive(true);
            }

            ++i;
        });

        for(; i < goodsList.Count; ++i)
        {
            FBoxGoods boxGoods = goodsList[i];
            boxGoods.gameObject.SetActive(false);
            if (i % 2 == 0)
            {
                int lineIndex = i / 2 - 1;
                lineList[lineIndex].gameObject.SetActive(false);
                boxGoods.transform.parent.gameObject.SetActive(false);
            }
        }
    }

    public void OnClickPurchase()
    {
        FStoreController storeController = FLocalPlayer.Instance.FindController<FStoreController>();
        if (storeController != null)
        {
            storeController.RequestPurchaseBox(boxID);
        }
    }
}
