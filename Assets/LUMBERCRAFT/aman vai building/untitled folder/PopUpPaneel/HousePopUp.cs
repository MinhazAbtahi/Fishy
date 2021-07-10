using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class HousePopUp : BuildingsPopPup
{
    [Header("Buttons")]
    public Button upgradeCoin;
    public Button upgradeAdd;
    public Button btnClose;

    public TextMeshProUGUI txtHouseLevelCoin;
    public TextMeshProUGUI txtHouseLevelAd;

    public int homeId;
    public GameObject buildingObj;
    public GameObject[] home1UpgradeObj;
    public GameObject[] home2UpgradeObj;
    public GameObject[] home3UpgradeObj;

    [Header("sprites")]
    public Sprite disableCoinBtnImg;
    public Sprite enableCoinBtnImg;
    public Sprite disableAdBtnImg;
    public Sprite enableAdBtnImg;

    public bool notECPressed;
    public TextMeshProUGUI txtNotEnoughCoin;
    public GameObject coinEffects;
    public GameObject adEffects;
    public GameObject coinParent;
    public GameObject adParent;
    private void OnEnable()
    {
        upgradeableCheck();
        UpgradableAdCheck();
        txtNotEnoughCoin.gameObject.SetActive(false);
        notECPressed = false;
    }

    public override void Start()
    {
        //base.Start();
        btnClose.onClick.AddListener(() => BtnCrossCallback());
        upgradeCoin.onClick.AddListener(() => coinUpgrade());
        upgradeAdd.onClick.AddListener(() => adUpgrade());

        //homeId=this.gameObject.GetComponent<>

        var leveltext = LocalizationManager.getInstance().getLocalizedString("LEVEL") + " ";

        int houselvl = PlayerPrefs.GetInt("home" + homeId + "Key");
        txtHouseLevelCoin.text = leveltext + (houselvl + 1).ToString();
        txtHouseLevelAd.text = leveltext + (houselvl + 1).ToString();
       
    }

    public void UpdateUI()
    {
        var leveltext = LocalizationManager.getInstance().getLocalizedString("LEVEL") + " ";

        int houselvl = PlayerPrefs.GetInt("home" + homeId + "Key");
        txtHouseLevelCoin.text = leveltext + (houselvl + 1).ToString();
        txtHouseLevelAd.text = leveltext + (houselvl + 1).ToString();
    }

    public void upgrade()
    {

        int houselvl = PlayerPrefs.GetInt("home" + homeId + "Key");
       
        PlayerPrefs.SetInt("home" + homeId + "Key", houselvl + 1);


        buildingObj.GetComponentInParent<BuildingController>().shakeBuilding();

        var leveltext = LocalizationManager.getInstance().getLocalizedString("LEVEL") + " " ;
        txtHouseLevelCoin.text = leveltext + (houselvl+2).ToString();
        txtHouseLevelAd.text = leveltext + (houselvl+ 2).ToString();

        GameManager.UpdateGameData(GameData.HouseUpgrade);

        for (int i = 0; i < home1UpgradeObj.Length; i++)
        {
            home1UpgradeObj[i].SetActive(false);
        }
        home1UpgradeObj[houselvl].SetActive(true);

    }

    public void coinUpgrade()
    {
        int currentCoin = GameManager.instance.totalCoin;
        if (currentCoin >= 10)
        {
            upgrade();
            GameManager.instance.totalCoin -= 10;
            PlayerPrefs.SetInt("totalcoinKey", GameManager.instance.totalCoin);
            UIManager.instance.txtCoinCountInGame.text = GameManager.instance.totalCoin.ToString();
        }
        else
        {
            if (!notECPressed)
            {
                StartCoroutine(NotEnoughCoinRoutine());
            }
        }
        upgradeableCheck();
    }

    public void adUpgrade()
    {
        if (FPG.VideoAdsManager.GetInstance().IsVideoAdsAvailable())
        {
            //FPG.VideoAdsManager.GetInstance().ShowVideoAds(OnVideoAdClosed);
            FPG.AdViewer.Instance.ShowVideoAdOrCrossPromo(OnVideoAdClosed, "HousePopUp");

        }
        else
        {
            Time.timeScale = 1f;
            OnVideoAdClosed(false);
        }
        UpgradableAdCheck();
    }

    public void OnVideoAdClosed(bool adWatched)
    {
        if (adWatched)
        {
            Time.timeScale = 1f;
            upgrade();
        }

    }

    void BtnCrossCallback()
    {
        GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = true;
        UIManager.instance.housePopupPanel.SetActive(false);

        txtNotEnoughCoin.gameObject.SetActive(false);
        notECPressed = false;
    }

    public void upgradeableCheck()
    {
        var localization = upgradeCoin.GetComponent<LocalizationReplacer>();

        //check coin and disable button
        int currentCoin = GameManager.instance.totalCoin;
        if (currentCoin < 10)
        {
            //upgradeCoin.interactable = false;
            upgradeCoin.GetComponent<Image>().sprite = disableCoinBtnImg;
            coinEffects.SetActive(false);
            coinParent.GetComponent<Animator>().enabled = false;
            coinParent.GetComponent<RectTransform>().rotation= Quaternion.Euler(Vector3.zero);
            localization.SetNewData(alternate: true);

        }
        else
        {
            upgradeCoin.interactable = true;
            upgradeCoin.GetComponent<Image>().sprite = enableCoinBtnImg;
            coinEffects.SetActive(true);
            coinParent.GetComponent<Animator>().enabled = true;
            localization.SetNewData(alternate: false);

        }

    }
    public void UpgradableAdCheck()
    {
        var localization = upgradeAdd.GetComponent<LocalizationReplacer>();

        //check coin and disable button
        int currentCoin = GameManager.instance.totalCoin;
        if (FPG.VideoAdsManager.GetInstance().IsVideoAdsAvailable())
        {
            upgradeAdd.interactable = true;
            upgradeAdd.GetComponent<Image>().sprite = enableAdBtnImg;
            adEffects.SetActive(true);
            adParent.GetComponent<Animator>().enabled = true;
            localization.SetNewData(alternate: false);

        }
        else
        {
            upgradeAdd.interactable = false;
            upgradeAdd.GetComponent<Image>().sprite = disableAdBtnImg;
            adEffects.SetActive(false);
            adParent.GetComponent<Animator>().enabled = false;
            adParent.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
            localization.SetNewData(alternate: true);

        }

    }
    IEnumerator NotEnoughCoinRoutine()
    {
        notECPressed = true;
        txtNotEnoughCoin.gameObject.SetActive(true);
        txtNotEnoughCoin.rectTransform.DOShakeScale(0.3f,0.2f);
        yield return new WaitForSeconds(1.5f);
        txtNotEnoughCoin.gameObject.SetActive(false);
        notECPressed = false;
    }
}
