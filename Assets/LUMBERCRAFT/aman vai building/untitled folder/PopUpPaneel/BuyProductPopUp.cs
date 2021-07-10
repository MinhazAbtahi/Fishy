using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BuyProductPopUp : BuildingsPopPup
{
    public LumberCraft.ProductLand productLand;
    public Button btnBuy;
    public Button btnBuyAd;

    public override void Start()
    {
        base.Start();
        btnBuy.onClick.AddListener(() => BuyProductCallBack());
        btnBuyAd.onClick.AddListener(() => BuyProductAdCallBack());
    }

    private void BuyProductAdCallBack()
    {
        if (FPG.AdViewer.Instance != null)
        {
            FPG.AdViewer.Instance.ShowVideoAdOrCrossPromo(BuyProductCallBack);
        }
    }

    private void BuyProductCallBack(bool success = true)
    {
        if (success)
        {
            productLand.LoadProduct();
        }
        ClosePopUpCallBack();
    }
}
