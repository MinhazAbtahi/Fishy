using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

public class ShopPopPup : BuildingsPopPup
{
    [Header("Buttons")]
    public Button btnOffer_1;
    public Button btnOffer_2;
    public Button btnNextOffer;
    public Button btnBestOffer;
    public Button btnClose;

    [Header("TextFiedl")]
    [Space(10)]
    public TextMeshProUGUI textClienNo;


    [Header("offer field")]
    [Space(10)]
    public TextMeshProUGUI offerValueTxtWood;

    public TextMeshProUGUI offer1TxtCoin;
    public TextMeshProUGUI offer1TxtWood;

    public TextMeshProUGUI offer2TxtCoin;
    public TextMeshProUGUI offer2TxtWood;
    int offerWoodValue;
    int currentWood;



    [Header("Gameobjects")]
    [Space(10)]
    public GameObject bestOffer;
    public Sprite[] spriteClients;
    public Sprite[] spriteDeals;
    public Image imgClient;
    public Image imgDeal;
    public GameObject imgDealPerfect;
    public GameObject imgDealBad;
    public GameObject imgDealGood;
    private int randomMultipliedClient;

    //------Private Variables-------
    public int clientState = 0;

    private void Awake()
    {
        randomMultipliedClient = UnityEngine.Random.Range(1, 4);
        //Start();
     
    }

    public override void Start()
    {
        base.Start();
        btnNextOffer.onClick.AddListener(() => NextOfferCallBack());
        btnClose.onClick.AddListener(() => ClosePopUpCallBack());
        btnBestOffer.onClick.AddListener(() => ClosePopUpCallBack());
        btnOffer_1.onClick.AddListener(() => buyOffer(1));
        btnOffer_2.onClick.AddListener(() => buyOffer(2));

        bestOffer.SetActive(false);

        //randomMultipliedClient = UnityEngine.Random.Range(0, 3);
        SetClient(clientState);

        currentWood = GameManager.instance.totalWood;
        if (currentWood < 11)
            ClosePopUpCallBack();
    }

    private void NextOfferCallBack()
    {
        clientState++;
        SetClient(clientState);
    }
    private void CheckDeals(int clientState)
    {
        imgDealPerfect.gameObject.SetActive(false);
        imgDealGood.gameObject.SetActive(false);
        imgDealBad.gameObject.SetActive(false);
        if (clientState == 0)
        {
            imgDealPerfect.gameObject.SetActive(true);

        }
        else if (clientState == 1)
        {
            imgDealGood.gameObject.SetActive(true);

        }
        else
        {
            imgDealBad.gameObject.SetActive(true);


        }
    }

    public void SetClient(int clientNo)
    {
        imgClient.sprite = spriteClients[clientState/*UnityEngine.Random.Range(0,spriteClients.Length)*/];
        //imgDeal.sprite = spriteDeals[clientState/*UnityEngine.Random.Range(0,spriteClients.Length)*/];
        CheckDeals(clientState);
        var CLIENT = LocalizationManager.getInstance().getLocalizedString("CLIENT %d/3") ;
        textClienNo.text = CLIENT.Replace("%d", (clientState + 1).ToString()) ;

        offerWoodValue= ((clientState * randomMultipliedClient) + 10)/*UnityEngine.Random.Range(10,16)*/;
        offerValueTxtWood.text = offerWoodValue.ToString();

        currentWood = GameManager.instance.totalWood;
        offer1TxtWood.text= currentWood.ToString();
        offer2TxtWood.text = (currentWood/2).ToString();
        offer1TxtCoin.text = (currentWood / offerWoodValue).ToString();
        offer2TxtCoin.text = ((currentWood / 2)/offerWoodValue).ToString();
        //Color btnColor;
        //if (clientNo == 1 || clientNo == 3)
        //{
        //    btnColor = Color.cyan;
        //}
        //else btnColor = Color.yellow;

        //btnOffer_1.image.color = btnColor;
        //btnOffer_2.image.color = btnColor;




        if (clientState > 1)
        {
            clientState = -1;
            //btnNextOffer.gameObject.SetActive(false);
            //bestOffer.gameObject.SetActive(true);
        }
    }

    public void buyOffer(int offerNo)
    {
        if (offerNo == 1)
        {
            if ((currentWood / offerWoodValue) > 0)
            {
                GameManager.instance.GiveWood(-currentWood);
                //GameManager.instance.GiveCoin(currentWood / offerWoodValue);
                UIManager.instance.PlayResourceClaimAnimationModified(1, 10, offer1TxtCoin.rectTransform, currentWood / offerWoodValue, null);
            }
        }
        else
        {
            if (((currentWood / offerWoodValue)/2) > 0)
            {
                GameManager.instance.GiveWood(-(currentWood / 2));
                //GameManager.instance.GiveCoin((currentWood / 2) / offerWoodValue);
                UIManager.instance.PlayResourceClaimAnimationModified(1, 10, offer2TxtCoin.rectTransform, (currentWood / 2) / offerWoodValue, null);
            }
        }

        UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
        //UIManager.instance.txtCoinCountInGame.text = GameManager.instance.totalCoin.ToString();

        ClosePopUpCallBack();
    }
}
