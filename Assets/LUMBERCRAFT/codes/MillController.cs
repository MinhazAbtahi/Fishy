using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MillController : MonoBehaviour
{
    public int millLevel;
    public int millWoodCount;
    public int millWoodcapacity;
    public int productionRate;
    public GameObject millCanvas;
    public Button btnCross;
    public TextMeshPro txtMillWoodCount;
    public TextMeshProUGUI txtMillLevelCoin;
    public TextMeshProUGUI txtMillLevelAd;
    public TextMeshProUGUI txtCapacity;
    public TextMeshProUGUI txtProductionRate;
    public TextMeshProUGUI txtUpgradeCost;
    public float millWoodProduceDelay;
    public GameObject millWoodStack;
    public static MillController instance;

    private void Awake()
    {
        instance = this;

    }

    private void Start()
    {
        int milllvl;
        btnCross.onClick.AddListener(() => BtnCrossCallback());
       
        milllvl = PlayerPrefs.GetInt("millLevel", 0);

        CheckMillStatus(milllvl);
    }
    // Update is called once per frame
    void Update()
    {
        if(gameObject.GetComponent<BuildingController>().isComplete)
            ProduceWood();
    }
    public void CheckMillStatus(int millLvl)
    {
        millLevel = millLvl;

        millWoodcapacity = 40+ (millLevel* 10*40 / 100);
        productionRate = 60 + (millLevel * 10 * 60 / 100);
        var capacity = LocalizationManager.getInstance().getLocalizedString("CAPACITY") + ": ";
        txtCapacity.text = capacity + millWoodcapacity.ToString();
        var PERMIN = LocalizationManager.getInstance().getLocalizedString("PER MIN") + ": ";

        txtProductionRate.text = PERMIN + productionRate.ToString();

        //txtMillLevelCoin.text = "LEVEL " + (millLevel + 1).ToString();
        //txtMillLevelAd.text = "LEVEL " + (millLevel + 1).ToString();
        txtMillWoodCount.text = (millWoodCount + "/" + millWoodcapacity).ToString();
    }
    public void ProduceWood()
    {
        if(millWoodCount < millWoodcapacity)
        {
            millWoodProduceDelay += Time.deltaTime;
            if (millWoodProduceDelay >= 1)
            {
                millWoodStack.SetActive(true);
                millWoodCount++;
                txtMillWoodCount.text = (millWoodCount + "/" + millWoodcapacity).ToString();
                millWoodProduceDelay = 0;
               
            }
        }
    }
    public void GiveWood()
    {
        GameManager.instance.totalWood += millWoodCount;
        PlayerPrefs.SetInt(GameManager.totalWoodKey, GameManager.instance.totalWood);

        millWoodCount = 0;
        millWoodStack.SetActive(false);
    }
    void BtnCrossCallback()
    {
        GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = true;
        UIManager.instance.lumbermillPopupPanel.SetActive(false);
    }
}
