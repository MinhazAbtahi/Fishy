using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;


public class SmallIslandPopUp : BuildingsPopPup
{
    [Header("Buttons")]
    public Button upgradeCoin;
    public Button upgradeAdd;
    public Button btnClose;

    public TextMeshProUGUI txtHouseLevelCoin;
    public TextMeshProUGUI txtHouseLevelAd;

    public int homeId;
    public GameObject buildingObj;

    [Header("sprites")]
    public Sprite disableCoinBtnImg;
    public Sprite enableCoinBtnImg;

    private void OnEnable()
    {
        upgradeableCheck();
    }

    public override void Start()
    {
        //base.Start();
        btnClose.onClick.AddListener(() => BtnCrossCallback());
        upgradeCoin.onClick.AddListener(() => coinUpgrade());
        upgradeAdd.onClick.AddListener(() => adUpgrade());

        int houselvl = PlayerPrefs.GetInt("small_island" + homeId + "Key");
        //txtHouseLevelCoin.text = "LEVEL " + (houselvl + 1).ToString();
        //txtHouseLevelAd.text = "LEVEL " + (houselvl + 1).ToString();       
    }

    public void UpdateUI()
    {
        int houselvl = PlayerPrefs.GetInt("small_island" + homeId + "Key");
        //txtHouseLevelCoin.text = "LEVEL " + (houselvl + 1).ToString();
        //txtHouseLevelAd.text = "LEVEL " + (houselvl + 1).ToString();
    }

    public void upgrade()
    {
        int houselvl = PlayerPrefs.GetInt("small_island" + homeId + "Key");
       
        PlayerPrefs.SetInt("small_island" + homeId + "Key", houselvl + 1);

        buildingObj.GetComponentInParent<BuildingController>().radialProgress.StopFill();
        //txtHouseLevelCoin.text = "LEVEL " + (houselvl+2).ToString();
        //txtHouseLevelAd.text = "LEVEL " + (houselvl+ 2).ToString();
    }

    public void coinUpgrade()
    {
        int currentCoin = GameManager.instance.totalCoin;
        if (currentCoin >= 1)
        {
            upgrade();
            GameManager.instance.totalCoin -= 1;
            PlayerPrefs.SetInt("totalcoinKey", GameManager.instance.totalCoin);
            UIManager.instance.txtCoinCountInGame.text = GameManager.instance.totalCoin.ToString();
        }
        upgradeableCheck();
        gameObject.SetActive(false);
    }

    public void adUpgrade()
    {
        if (FPG.VideoAdsManager.GetInstance().IsVideoAdsAvailable())
        {
            FPG.VideoAdsManager.GetInstance().ShowVideoAds(OnVideoAdClosed);

        }
        else
        {
            Time.timeScale = 1f;
            OnVideoAdClosed(false);
        }

    }

    public void OnVideoAdClosed(bool adWatched)
    {
        if (adWatched)
        {
            Debug.Log("fafafafaf");
            Time.timeScale = 1f;
            upgrade();
        }
        gameObject.SetActive(false);
    }

    void BtnCrossCallback()
    {
        GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = true;
        UIManager.instance.smallIslandPopupPanel.SetActive(false);
    }

    public void upgradeableCheck()
    {
        //check coin and disable button
        int currentCoin = GameManager.instance.totalCoin;
        if (currentCoin < 1)
        {
            upgradeCoin.interactable = false;
            upgradeCoin.GetComponent<Image>().sprite = disableCoinBtnImg;
        }
        else
        {
            upgradeCoin.interactable = true;
            upgradeCoin.GetComponent<Image>().sprite = enableCoinBtnImg;
        }
        
    }
}
