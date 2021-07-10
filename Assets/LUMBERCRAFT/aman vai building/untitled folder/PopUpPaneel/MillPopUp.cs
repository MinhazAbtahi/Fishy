using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using DG.Tweening;

public class MillPopUp : BuildingsPopPup
{
    [Header("Buttons")]
    public Button upgradeCoin;
    public Button upgradeAdd;
    public Button btnClose;

    public TextMeshProUGUI txtMillLevelCoin;
    public TextMeshProUGUI txtMillLevelAd;

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
        base.Start();
        btnClose.onClick.AddListener(() => ClosePopUpCallBack());
        upgradeCoin.onClick.AddListener(() => coinUpgrade());
        upgradeAdd.onClick.AddListener(() => adUpgrade());


        int milllvl = PlayerPrefs.GetInt("millLevel", 0);
        var leveltext = LocalizationManager.getInstance().getLocalizedString("LEVEL") +" ";

        txtMillLevelCoin.text = leveltext + (milllvl + 1).ToString();
        txtMillLevelAd.text = leveltext + (milllvl + 1).ToString();

    }

    public void upgrade()
    {

        int milllvl = PlayerPrefs.GetInt("millLevel", 0);
        MillController.instance.millLevel = milllvl + 1;
        PlayerPrefs.SetInt("millLevel", milllvl + 1);
        MillController.instance.CheckMillStatus(milllvl + 1);
        var leveltext = LocalizationManager.getInstance().getLocalizedString("LEVEL") + " ";

        txtMillLevelCoin.text = leveltext + (milllvl + 2).ToString();
        txtMillLevelAd.text = leveltext + (milllvl + 2).ToString();

        GameManager.UpdateGameData(GameData.HouseUpgrade);

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
            FPG.AdViewer.Instance.ShowVideoAdOrCrossPromo(OnVideoAdClosed, "MillPopUp");

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
            coinParent.GetComponent<RectTransform>().rotation = Quaternion.Euler(Vector3.zero);
            localization.SetNewData(alternate: true);
            Debug.Log("upgrade should be disabled");
        }
        else
        {
            upgradeCoin.interactable = true;
            upgradeCoin.GetComponent<Image>().sprite = enableCoinBtnImg;
            coinEffects.SetActive(true);
            coinParent.GetComponent<Animator>().enabled = true;
            localization.SetNewData(alternate: false);

        }
       // localization.ReloadSprite();

    }
    public void UpgradableAdCheck()
    {
        //check coin and disable button
        var localization = upgradeAdd.GetComponent<LocalizationReplacer>();

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
        upgradeAdd.GetComponent<LocalizationReplacer>().ReloadSprite();

    }
    IEnumerator NotEnoughCoinRoutine()
    {
        notECPressed = true;
        txtNotEnoughCoin.gameObject.SetActive(true);
        txtNotEnoughCoin.rectTransform.DOShakeScale(0.3f, 0.2f);
        yield return new WaitForSeconds(1.5f);
        txtNotEnoughCoin.gameObject.SetActive(false);
        notECPressed = false;
    }
}
