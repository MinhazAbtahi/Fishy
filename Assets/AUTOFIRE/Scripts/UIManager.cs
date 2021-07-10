using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System;
using UnityEngine.Events;

public class UIManager : MonoBehaviour
{
    public static UIManager instance;
  
    public GameObject gamePanel;
    public GameObject gameOverPanel;
    public GameObject levelCompletePanel;
    public GameObject tutorial;
    public Button retryBtn;
    public Button nextBtn;
    public Button homeBtn;
    public Button retryHomeBtn;
   

    public Image healthBar;
    public Image levelProgBar;
    [Header("Start Panel")]
    public GameObject startPanel;
    public Button playBtn;
    public TextMeshProUGUI txtLevel;
    public TextMeshProUGUI txtWorld;
    public TextMeshProUGUI txtCoin;
    public TextMeshProUGUI txtWood;
    public TextMeshProUGUI txtHowtoPlay;
    [HideInInspector] public bool howtoPlayTapped;
    [Header("Game Panel")]
    public TextMeshProUGUI txtLevelGamePanel;
    public TextMeshProUGUI txtWorldGamePanel;
    public TextMeshProUGUI txtWoodCountInGame;
    public TextMeshProUGUI txtCoinCountInGame;
    public TextMeshProUGUI txtBuildingCountInGame;
    [Header("Lumbercraft")]
    public GameObject shopTradingPopupPanel;
    public GameObject lumbermillPopupPanel;
    public GameObject housePopupPanel;
    public GameObject smallIslandPopupPanel;

    //ui coin anim
    [Header("Coin Anim")]
    private WaitForSeconds resourceClaimWaitTime;
    private WaitForSeconds textIncrementWaitTime;
    private float transitionTime = 0.45f;
    private int totalRemove;
    private Coroutine jumpTextCoroutine;
    bool isTextJumpingStarted = false;
    public RectTransform uiCoinPos;
    public RectTransform uiWoodPos;
    public GameObject resourceSprite;
    public Sprite coinSprite;

    //sharkWorld
    [Header("World")]
    public GameObject sharkWorldPanel;
    public Button sharkWorldBtn;
    public GameObject UICanvas;
    public List<Sprite> worldImages;
    public Image imgProdImage;
    public TextMeshProUGUI txtWorldName;
    string[] WorldName = new string[]
    {
    "Enchanted Forest",
    "Forgotten Land",
    "Death Valley",
    "Waste Land",
    "Middgard",
    "Silent Forest",
    "Spirit Hill",
    "Westerworld",
    "Badlands",
    "Hell Forest"
    };


  

    private void Awake()
    {
        instance = this;
        playBtn.onClick.AddListener(() => PlayBtnCallback());
        retryBtn.onClick.AddListener(() => RetryBtnCallback());
        nextBtn.onClick.AddListener(() => NextBtnCallback());
        homeBtn.onClick.AddListener(() => HomeBtnCallback());
        retryHomeBtn.onClick.AddListener(() => HomeBtnCallback());
        sharkWorldBtn.onClick.AddListener(() => WorldsBtnCallback());
    }
    // Start is called before the first frame update
    void Start()
    {
        //startPanel.SetActive(true);
        PlayBtnCallback();
        txtLevel.text = "Level- " + (GameManager.instance.levelID + 1).ToString();
        txtWorld.text = "World- " + (GameManager.instance.worldID + 1).ToString();
        txtCoin.text = GameManager.instance.totalCoin.ToString();
        txtWood.text = GameManager.instance.totalWood.ToString();
        InGameProgressionText();

        levelProgBar.fillAmount = (float)GameManager.instance.botsKilled / (float)GameManager.instance.botsToKill;
        txtBuildingCountInGame.text = (GameManager.instance.buildingMade +"/"+ GameManager.instance.buildingToMake).ToString();
        PlayerPrefs.SetInt("Home", 1);
        FPG.IDFAAccessController.AskIDFAAccessAtTheBeginning();

        //SoundManager.sharedInstance.StopIngameBGAudio();
    }
    public void PlayBtnCallback()
    {
        GameManager.instance.player.transform.DORotateQuaternion(Quaternion.identity,0.5f);
        Camera.main.transform.DORotateQuaternion(GameManager.instance.gameCamPos.transform.rotation, 0.5f);
        Camera.main.transform.DOMove(GameManager.instance.gameCamPos.transform.position,0.5f).OnComplete(()=>
        {
            GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = true;
            GameManager.instance.cameraCon.GetComponent<CameraController>().enabled = true;
        });
        GameManager.instance.startMenuArea.SetActive(false);
        SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.playBtnSFX);
        //GameManager.instance.SetCurrentWorld();
        BotSpawner.instance.SpawnTimerController();
        startPanel.SetActive(false);
        gamePanel.SetActive(true);
        GameManager.instance.startGame = true;
        SoundManager.sharedInstance.StopMainMenuAudio();
        SoundManager.sharedInstance.PlayIngameBGAudio();
        InGameProgressionText();
        txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
        //GameManager.instance.portalFX.transform.DOScale();
        FPG.Networking.getInstance().SetGameStatus();
    }
    public void RetryBtnCallback()
    {
        PlayerPrefs.SetInt("Home", 0);
        SceneManager.LoadScene("GameScene");
    }

    public void NextBtnCallback()
    {
        PlayerPrefs.SetInt("Home", 0);
        SceneManager.LoadScene("GameScene");
    }

    public void HomeBtnCallback()
    {
        PlayerPrefs.SetInt("Home", 1);
        SceneManager.LoadScene("GameScene");
    }   

    public void WorldsBtnCallback()
    {

        sharkWorldPanel.SetActive(true);
        startPanel.SetActive(false);
        
    }

    public void HomeCallback()
    {
        //PlayerPrefs.SetInt("Home", 1);
        startPanel.SetActive(true);
    }

    public void UpdateWorldUI(int worldId)
    {
        var level = LocalizationManager.getInstance().getLocalizedString("Level");
        txtLevel.text = level + "- " + (GameManager.instance.levelID + 1).ToString();
        txtWorld.text = "World- " + (GameManager.instance.worldID + 1).ToString();

        txtLevelGamePanel.text = level + "- " + (GameManager.instance.levelID + 1).ToString();
        txtLevelGamePanel.text = "World- " + (GameManager.instance.worldID + 1).ToString();

        imgProdImage.sprite = worldImages[worldId];
        txtWorldName.text = WorldName[worldId];
        //Debug.Log("level "+GameManager.instance.levelID);
    }
    public void InGameProgressionText()
    {
        var level = LocalizationManager.getInstance().getLocalizedString("Level");

        txtLevelGamePanel.text = level + "- " + (GameManager.instance.levelID + 1).ToString();
        txtWorldGamePanel.text = "World- " + (GameManager.instance.worldID + 1).ToString();
    }
    public IEnumerator GameOverPanelDelayRoutine()
    {
        yield return new WaitForSeconds(1.25f);

        GameManager.instance.ShowInterstitialAd();
        //gameOverPanel.SetActive(true);
    }
    public void TradingShopOpen()
    {
        shopTradingPopupPanel.SetActive(true);
        shopTradingPopupPanel.GetComponent<ShopPopPup>().clientState = 0;
        shopTradingPopupPanel.GetComponent<ShopPopPup>().SetClient(0);
        //GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().move = false;
        //GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = false;

    }
    public void LumberMillOpen()
    {
        lumbermillPopupPanel.SetActive(true);
        //StartCoroutine(millUIOpenDelay());
        //GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().move = false;
        //GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = false;

    }
    IEnumerator millUIOpenDelay()
    {    
        yield return new WaitForSeconds(0.5f);
        lumbermillPopupPanel.SetActive(true);
    }

    public void houseOpen(int homeId,GameObject building)
    {
        StartCoroutine(houseUIOpenDelay(homeId, building));
        //GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().move = false;
        //GameManager.instance.player.GetComponent<LumberCraft.PlayerInputController>().enabled = false;

    }

    IEnumerator houseUIOpenDelay(int homeId, GameObject building)
    {
        yield return new WaitForSeconds(0f);
        housePopupPanel.GetComponent<HousePopUp>().homeId = homeId;
        housePopupPanel.GetComponent<HousePopUp>().UpdateUI();
        housePopupPanel.GetComponent<HousePopUp>().buildingObj = building;
        housePopupPanel.SetActive(true);
       
    }

    public void SmallIslandOpen(int homeId, GameObject building)
    {
        StartCoroutine(SmallIslandUIOpenDelay(homeId, building));
    }

    IEnumerator SmallIslandUIOpenDelay(int homeId, GameObject building)
    {
        yield return new WaitForSeconds(0f);
        smallIslandPopupPanel.GetComponent<SmallIslandPopUp>().homeId = homeId;
        smallIslandPopupPanel.GetComponent<SmallIslandPopUp>().UpdateUI();
        smallIslandPopupPanel.GetComponent<SmallIslandPopUp>().buildingObj = building;
        smallIslandPopupPanel.SetActive(true);
    }

    // now adds coin/bucks automatically after animation has completed //1 for coin, 0 for wood ** wood not complete yet
    public void PlayResourceClaimAnimationModified(int _type, int _total, RectTransform _startPos/*, RectTransform _endPos*//*, Transform canvas*//*, GameObject textToJumpGO*/, int resourceAmount, UnityAction funcTocall = null, bool heptic = true, bool isAnimCanvas = false)
    {
      //  Log.L("playresourceclaimanimation");
        totalRemove = _total > 10 ? 10 : _total;
        StartCoroutine(ShowResourceFlyModified(_type, _startPos,/* _endPos*/uiCoinPos, UICanvas.transform /*canvas*/, txtCoin.gameObject, resourceAmount, funcTocall, heptic, isAnimCanvas));
    }


    IEnumerator ShowResourceFlyModified(int resourceType, RectTransform _startPos, RectTransform _endPos, Transform canvas, GameObject textToJumpGO, int resourceAmount, UnityAction funcTocall = null, bool heptic = false, bool isAnimCanvas = false)
    {
        Vector3 endPosition = _endPos.position;
        Vector3 startPosition = _startPos.position;
        //Log.L("in show resources flay");
        if (_endPos == null)
        {
          //  Log.L("null");
        }
        List<GameObject> g = new List<GameObject>();
        resourceClaimWaitTime = new WaitForSeconds(.4f);
        // 0==Coin
        if (resourceType == 1)
        {
            float range = isAnimCanvas ? 15 : 120;
            //if (UIManager.sharedManager().iapPanel != null) range = 15;


            //range = 15;
            for (int i = 0; i < totalRemove; i++)
            {
                if (_startPos != null)
                {
                    //Debug.Log("in ShowResourceFlyModified function : " + resourceAmount);
                    GameObject _gameObject = Instantiate(resourceSprite, startPosition, Quaternion.identity, canvas.transform);
                    _gameObject.GetComponent<Image>().sprite = coinSprite; //coin
                    g.Add(_gameObject);

                    /*if (i != 0) */                                        //For some unknown _startpos error
                                                                            // _gameObject.transform.DOMove(_endPos.position, transitionTime);

                    //sakib anim modify
                    Vector3 pos = _gameObject.GetComponent<RectTransform>().position;

                    _gameObject.GetComponent<RectTransform>().DOMove(new Vector3(pos.x + UnityEngine.Random.Range(-range, range), pos.y + UnityEngine.Random.Range(-range, range), pos.z), .6f);

                    //sakib anim modify

                    //_gameObject.GetComponent<RectTransform>().DOMove(_endPos.position, transitionTime);
                }
                yield return new WaitForSeconds(.05f);

            }
            yield return new WaitForSeconds(.05f);

            if (heptic)
            {
#if !UNITY_EDITOR && UNITY_ANDROID

            Vibration.Vibrate(30);
#endif
#if !UNITY_EDITOR && UNITY_IPHONE && UNITY_IOS
 
            Vibration.VibratePop();
#endif
            }

            //sakib modify for akshonge coin jaoa
            foreach (GameObject coins in g)
            {

                coins.GetComponent<RectTransform>().DOMove(endPosition, .7f);
                yield return new WaitForSeconds(.05f);
                if (!isTextJumpingStarted)
                {
                    //Debug.LogError("is jumpstarted is being true");
                    isTextJumpingStarted = true;
                    jumpTextCoroutine = StartCoroutine(JumpText(textToJumpGO, resourceType, resourceAmount));
                }
            }
        }
       
       
        
        yield return new WaitForSeconds(0.5f);
        foreach (var item in g)
        {
            Destroy(item);
        }

        isTextJumpingStarted = false;
        funcTocall?.Invoke();
    }

    public IEnumerator JumpText(GameObject textToJumpGo, int resourceType, int resourceAmount)
    {
        yield return new WaitForSeconds(0.4f);
        if (textToJumpGo == null)
            yield break;
        TextMeshProUGUI textToJump = textToJumpGo.GetComponent<TextMeshProUGUI>();
        textToJump.text = resourceAmount.ToString();
        Vector3 originalScale = textToJumpGo.transform.localScale;
        float scalefactor = 0.5f;
        float duration = 0.07f;
        int amountToAddPerJump = resourceAmount / 10;
        int totalAmountAdded = 0;
        //     int currentAmount = Convert.ToInt32( textToJump.text);
        int currentAmount = GameManager.instance.totalCoin;
        GameManager.instance.GiveCoin(resourceAmount);

        while (isTextJumpingStarted)
        {
#if !UNITY_EDITOR && UNITY_ANDROID

            Vibration.Vibrate(30);
#endif
#if !UNITY_EDITOR && UNITY_IPHONE && UNITY_IOS
 
            Vibration.VibratePop();
#endif
            textToJumpGo.transform.DOScale(new Vector3(originalScale.x + scalefactor, originalScale.y + scalefactor, 1), duration);
            yield return new WaitForSeconds(duration);
            totalAmountAdded += amountToAddPerJump;
            //if (totalAmountAdded < resourceAmount) ResourcesData.sharedManager().AddResource(resourceType, amountToAddPerJump);
            //textToJump.text = StorePanel.GetStringFromValue(ResourcesData.sharedManager().AmountOfResources(resourceType));
            if (totalAmountAdded < resourceAmount) currentAmount += amountToAddPerJump;
            textToJump.text = currentAmount.ToString();
            textToJumpGo.transform.DOScale(originalScale, duration);
            yield return new WaitForSeconds(duration);
        }

        //if(totalAmountAdded<resourceAmount) ResourcesData.sharedManager().AddResource(resourceType, resourceAmount-totalAmountAdded);
        textToJump.text = GameManager.instance.totalCoin.ToString();
        textToJumpGo.transform.localScale = originalScale;
        // Debug.Log("in JumpText function : " + resourceAmount);
    }

    private void StopTextJumping()
    {
        StopCoroutine(jumpTextCoroutine);
    }
}
