using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.AI;
using Luna.Replay.Playground;
using LumberCraft;

public enum GameData
{
    LevelCount,
    HouseCount,
    HouseUpgrade,
    EnemyCount,
    BridgeCount,
    WoodCount,
}

public enum ControlType
{
    Player,
    Boat
}

public class GameManager : MonoBehaviour, ISerializationCallbackReceiver
{
    public static GameManager instance;
    public ControlType controlType;
    public bool startGame;
    public bool victory;

    public GameObject player;
    public PlayerController playerCon;
    public GameObject boat;
    public GameObject petObject;
    public BotSpawner botSpawner;
    public GameObject cameraCon;
    [SerializeField]
    [Header("Currency")]
    public static string totalcoinKey = "totalcoinKey";
    public int totalCoin;
    public GameObject coinPrefab;
    public static string totalWoodKey = "totalwoodKey";
    public int totalWood;
    public static string totalGoldKey = "totalgoldKey";
    public int totalGold;
    public static string totalSilverKey = "totalsilverKey";
    public int totalSilver;
    public static string totalDiamondKey = "totaldiamondKey";
    public int totalDiamond;

    [Header("FX")]
    public GameObject tapFX;
    public GameObject popFX;
    public GameObject hitFX;
    public GameObject hitTreeFX;
    public GameObject hitGoldFX;
    public GameObject hitSilverFX;
    public GameObject hitDiamondFX;
    public GameObject portalPrefab;
    public GameObject dmgTextPopupPrefab;
    public GameObject woodPlusTextPopupPrefab;
    public GameObject confettiFX;
    public GameObject waterSplashFX;
    public GameObject buildStageCompleteFX;
    public GameObject enemyDeathFX;

    [Header("Level Info")]
    public static string worldKey = "worldKey";
    public int worldID;
    public static string levelKey = "levelKey";
    public int levelID;
    public int botsKilled;
    public int botsToKill;
    public int buildingMade;
    public int buildingToMake;
    [Header("World Info")]
    public GameObject homeBaseObject;
    public GameObject[] environments;
    public GameObject[] ground;
    public Material[] env1Mats;
    public Material[] env2Mats;
    public Material[] groundMats;
    public NavMeshSurface surface;

    [Header("Luna Replay Test")]
    [SerializeField]
    [LunaPlaygroundField()]
    public GameObject groundObject;
    public GameObject playerObject;
    [LunaPlaygroundField()]
    public int groundMatID;
    [LunaPlaygroundField()]
    public bool lunaRecording = false;

    public static string gamelevelKey = "gamelevelKey";
    public int gameLevel;
    [Header("LumberCraft")]
    public string warKey = "warKey";
    public int warStatus;
    public string respawnKey = "respawnKey";
    public int respawnStatus;
    public GameObject treePrefab;
    public GameObject woodPickupPrefab;
    public GameObject woodPickupAdTreePrefab;
    public GameObject woodLogPrefab;
    public GameObject[] resourcePickupPrefabs;
    public GameObject[] resourcePickup5StacksPrefabs;
    public GameObject startMenuArea;
    public GameObject startCamPos;
    public GameObject gameCamPos;
    [Header("LumberCraft Building")]
    public GameObject[] buildingPrefabs;
    public GameObject[] buildingPositionsWorld1;
    public GameObject[] buildingPositionsWorld2;
    public GameObject[] buildingPositionsWorld3;
    [Header("LumberCraft Base Building")]
    public bool isInBase;
    public string homeKey = "home0Key";
    public int homeStatus;
    public string millKey = "millKey";
    public int millStatus;
    public string shopKey = "shopKey";
    public int shopStatus;

    public string home2Key = "home1Key";
    public int home2Status;
    public string home3Key = "home2Key";
    public int home3Status;

    [Header("Counter")]
    public static int woodCounter = 0 ;
    public static int enemyCounter = 0;
    public static string bridgeCounterKey = "BridgeCounterKey";
    public static string levelupCounterKey = "LevelupCounterKey";
    public static string houseCounterKey = "HouseCounterKey";
    public static string houseUpgradeKey = "HouseUpgradeKey";
    public static string enemyCounterKey = "EnemyCounterKey";
    public static string woodCounterKey = "WoodCounterKey";
    public static int botType = 0;



    private void Awake()
    {
        instance = this;
        surface.RemoveData();

    }
    // Start is called before the first frame update
    void Start()
    {
        //core stuff
        if (player != null)
            playerCon = player.GetComponent<PlayerController>();

        levelID = PlayerPrefs.GetInt(levelKey,0)%9;
        gameLevel = PlayerPrefs.GetInt(gamelevelKey, 0);
        worldID = PlayerPrefs.GetInt(worldKey,0);
        totalCoin = PlayerPrefs.GetInt(totalcoinKey, 0);
        totalWood = PlayerPrefs.GetInt(totalWoodKey, 0);
        UIManager.instance.UpdateWorldUI(worldID);       
        if (!PlayerPrefs.HasKey("levelCountWorld0"))
        {
            for (int i = 0; i < 10; i++)
            {
                PlayerPrefs.SetInt("levelCountWorld" + i, 0);
            }
        }
        for (int i = 0; i < environments.Length; i++)
        {
            environments[i].SetActive(false);
        }

        //warStatus = PlayerPrefs.GetInt(warKey);
        warStatus = 1;
        if (warStatus == 1)
        {
            SetCurrentWorld();
            homeBaseObject.SetActive(false);
            isInBase = false;
            botSpawner.enabled = true;
        }
        else
        {
            homeBaseObject.SetActive(true);
            isInBase = true;
            botSpawner.enabled = false;
        }

        respawnStatus = PlayerPrefs.GetInt(respawnKey);
        if (respawnStatus == 1)
        {
            //take half wood
            TakeWood(totalWood / 2);
            //respawnStatus = 0;
            //PlayerPrefs.SetInt(respawnKey, respawnStatus);

        }

        //SetCurrentWorld();

        cameraCon.GetComponent<CameraController>().enabled = false;
        player.GetComponent<LumberCraft.PlayerInputController>().enabled = false;
        //startMenuArea.SetActive(true);

        CheckBaseStatus();

        if (TagManager.IsPetActivated())
        {
            petObject.SetActive(true);
        }
        else
        {
            petObject.SetActive(false);
        }
    }
    private void FixedUpdate()
    {

        if (lunaRecording)
        {
            groundObject.GetComponent<MeshRenderer>().material = groundMats[groundMatID];
            //luna replay tests
            //if (groundMatID == 0)
            //    groundObject.GetComponent<MeshRenderer>().material = groundMats[groundMatID];
            //else if (groundMatID == 1)
            //    groundObject.GetComponent<MeshRenderer>().material = groundMats[groundMatID];
            //else if (groundMatID == 2)
            //    groundObject.GetComponent<MeshRenderer>().material = groundMats[groundMatID];
        }

    }

    
    public void SwitchControl(ControlType controlType)
    {
        switch (controlType)
        {
            case ControlType.Player:
                boat.GetComponent<BoatInputController>().enabled = false;
                boat.GetComponent<BoatController>().storeIndicator.SetActive(false);
                player.GetComponent<PlayerInputController>().enabled = true;
                player.transform.position = new Vector3(-8f, player.transform.position.y, -75f);
                player.SetActive(true);
                cameraCon.GetComponent<CameraController>().target = player.transform;
                break;
            case ControlType.Boat:
                player.GetComponent<PlayerInputController>().enabled = false;
                player.SetActive(false);
                boat.GetComponent<BoatInputController>().enabled = true;
                boat.GetComponent<BoatController>().playerVisual.SetActive(true);
                boat.GetComponent<BoatController>().storeIndicator.SetActive(true);
                boat.SetActive(true);
                cameraCon.GetComponent<CameraController>().target = boat.transform;
                break;
            default:
                break;
        }
        
    }

    public void CheckLevelProgression()
    {
        if (!victory)
        {

            //if (botsKilled >= botsToKill && buildingMade >= buildingToMake)
            //{
            //    Debug.Log("level complete");
            //    victory = true;
            //    GameObject portalIns = Instantiate(portalPrefab, new Vector3(player.transform.position.x, player.transform.position.y + 3, player.transform.position.z - 5) , portalPrefab.transform.rotation);
            //    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.portalSFX);
            //}
        }
    }
    public void LevelCompleteUponEnteringPortal()
    {
        ShowInterstitialAd();
        FPG.FirebaseManager.GetInstance().LogAnalyticsEvent("LevelComplete_" + gameLevel);

       

        gameLevel++;
        PlayerPrefs.SetInt(gamelevelKey, gameLevel);

        levelID = (levelID + 1);

        //int levelCountWorld = PlayerPrefs.GetInt("levelCountWorld" + worldID);
        PlayerPrefs.SetInt("levelCountWorld" + worldID, levelID);

      
        //health aktu barbe
        playerCon.health += 10;
        if (playerCon.health > 100)
        {
            playerCon.health = 100;
        }
        PlayerPrefs.SetInt(playerCon.healthKey, playerCon.health);
        Debug.Log("levelID "+ levelID);
        if (levelID >= 9)//world progression
        {
            PlayerPrefs.SetInt("levelCountWorld" + worldID, 0);
            worldID++;
            PlayerPrefs.SetInt(worldKey, worldID);
            levelID = 0;
            playerCon.health = playerCon.maxHealth;
            PlayerPrefs.SetInt(playerCon.healthKey, playerCon.health);
            UIManager.instance.UpdateWorldUI(worldID);

        }

        

        int maxLevelCompleted = PlayerPrefs.HasKey("maxLevelCompleted") ? PlayerPrefs.GetInt("maxLevelCompleted") : 0;

        if (maxLevelCompleted <= gameLevel)
        {
            maxLevelCompleted = gameLevel;
            PlayerPrefs.SetInt("maxLevelCompleted", maxLevelCompleted);
        }


        //coin add
        if (maxLevelCompleted % 3 == 0)
        {

            if ((maxLevelCompleted / 3) % 3 == 0)
            {
                GiveCoin(((50 + (2 * 25)) + (worldID * 50)));
               
            }
            else if ((maxLevelCompleted / 3) % 2 == 0)
            {
                GiveCoin(((50 + (1 * 25)) + (worldID * 50)));
               
            }
            else
            {
                GiveCoin((50 + (0 * 25)) + (worldID * 50));              
            }

        }



        //levelID = levelID % 9;
        PlayerPrefs.SetInt(levelKey, levelID);


        //completePanel
        //if (levelID % 3 == 0)
        //{
        //    player.GetComponent<PlayerController>().CompleteLevel();
        //}
        //else
        //{
        //    UIManager.instance.NextBtnCallback();
        //}

        UIManager.instance.NextBtnCallback();


        //end of world check
        if (worldID > 9)
        {
            worldID = 0;
            PlayerPrefs.SetInt(worldKey, worldID);
        }
        

    }

    public void WarPortal()
    {
        warStatus = 1;
        PlayerPrefs.SetInt(warKey, warStatus);
        SceneManager.LoadScene("GameScene");
    }


    public void SetCurrentWorld()
    {
        //environments[0].SetActive(true);
        //level world select design
        if(worldID == 0)//world 3
        {
            int randMat = Random.Range(0, env1Mats.Length);
            environments[2].SetActive(true);

            foreach (Transform child in ground[0].transform)
            {
                child.GetComponent<MeshRenderer>().material = env1Mats[randMat];
            }
            surface.BuildNavMesh();

            //GameObject buildingIns = Instantiate(buildingPrefabs[0], buildingPositionsWorld3[Random.Range(0, buildingPositionsWorld3.Length)].transform.position, buildingPrefabs[0].transform.rotation);
            
        }
        else
        {
            if (worldID % 2 == 0)
            {
                int randMat = Random.Range(0, env1Mats.Length);
                environments[0].SetActive(true);

                foreach (Transform child in ground[0].transform)
                {
                    child.GetComponent<MeshRenderer>().material = env1Mats[randMat];
                }
                surface.BuildNavMesh();

                //GameObject buildingIns = Instantiate(buildingPrefabs[0], buildingPositionsWorld1[Random.Range(0, buildingPositionsWorld1.Length)].transform.position, buildingPrefabs[0].transform.rotation);
                

            }
            else
            {
                int randMat = Random.Range(0, env2Mats.Length);
                environments[1].SetActive(true);

                foreach (Transform child in ground[1].transform)
                {
                    child.GetComponent<MeshRenderer>().material = env2Mats[randMat];
                }
                surface.BuildNavMesh();

                //GameObject buildingIns = Instantiate(buildingPrefabs[0], buildingPositionsWorld2[Random.Range(0, buildingPositionsWorld2.Length)].transform.position, buildingPrefabs[0].transform.rotation);
                
            }
        }



        //world mat select
        //for (int g = 0; g < ground.Length; g++)
        //{
        //    //ground[g].GetComponent<MeshRenderer>().material = groundMats[worldID / 2];
        //}
        BotAmount();

        

    }
    public void BotAmount()
    {
       
        //enemy count decider
        botsToKill = 3 + worldID + levelID / 3;
        if (botsToKill > 7) botsToKill = 7;
        //BotSpawner.instance.enemyCountMax = botsToKill;
    }
    public void GiveCoin(int i)
    {
        totalCoin += i;
        PlayerPrefs.SetInt(totalcoinKey, totalCoin);
    }
    public void TakeCoin(int i)
    {
        totalCoin -= i;
        PlayerPrefs.SetInt(totalcoinKey, totalCoin);
    }
    public void GiveWood(int i)
    {
        totalWood += i;
        PlayerPrefs.SetInt(totalWoodKey, totalWood);
    }
    public void TakeWood(int i)
    {
        totalWood -= i;
        PlayerPrefs.SetInt(totalWoodKey, totalWood);
    }
    public void CheckBaseStatus()
    {
        homeStatus = PlayerPrefs.GetInt(homeKey);
        millStatus = PlayerPrefs.GetInt(millKey);
        shopStatus = PlayerPrefs.GetInt(shopKey);

        if (homeStatus == 1) TutorialManager.instance.house.SetActive(true);
        if (millStatus == 1) TutorialManager.instance.mill.SetActive(true);
        if (shopStatus == 1) TutorialManager.instance.shop.SetActive(true);
    }
    private static int adShowLevelInterval = 0;
    public void ShowInterstitialAd()
    {
        //adShowLevelInterval++;
        //adShowLevelInterval >= 3 &&
        if (FPG.MobileAdsManager.GetInstance().IsInterstitialAdAvialable())
        {
            FPG.Networking.getInstance().video_ad_source =  "GameOver";
            FPG.Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 1, 0, 1, FPG.Networking.getInstance().video_ad_source,false);
            FPG.MobileAdsManager.GetInstance().ShowInterstitial(OnIntersititialAdClosed);
            //FPG.AdViewer.Instance.ShowVideoAdOrCrossPromo(OnIntersititialAdClosed,"GameManager");
            //adShowLevelInterval = 0;

        }
        else
        {
            OnIntersititialAdClosed();
        }
    }
    public void OnIntersititialAdClosed()
    {
        SceneManager.LoadScene("GameScene");
    }
    private void OnApplicationQuit()
    {
        //warStatus = 0;
        //PlayerPrefs.SetInt(warKey, warStatus);
        PlayerPrefs.SetInt(respawnKey, 0);
    }
    public void OnBeforeSerialize()
    {
        
    }

    public void OnAfterDeserialize()
    {
        
    }

    #region GameDataValue

    public static void UpdateGameData(GameData _data)
    {
        switch(_data)
        {
            case GameData.LevelCount:
                {
                    int woodLevel = GetGameData(GameData.WoodCount);
                    PlayerPrefs.SetInt(woodCounterKey, ++woodLevel);
                    /*FPG.Networking.getInstance().sendUserActivity("BattleStart_" + woodLevel.ToString());*/

                    int level = GetGameData(GameData.LevelCount);
                    woodCounter++;
                    if(woodCounter >= 2)
                    {
                        woodCounter = 0;
                        level++;
                        PlayerPrefs.SetInt(levelupCounterKey, level);
                        FPG.Networking.getInstance().send_unpaid_user_battle_status(0,0,0);

                        if(level <=40)
                            FPG.Networking.getInstance().sendUserActivity("BattleStart_" + level.ToString());
                    }
                    
                }
                break;

            case GameData.HouseCount:
                {
                    int houseCount = GetGameData(GameData.HouseCount);
                    FPG.Networking.getInstance().sendUserActivity("HouseBuild_"+ houseCount.ToString());
                    FPG.Networking.getInstance().send_unpaid_user_battle_status(1, 1, 0, GameData.HouseCount);
                    houseCount++;
                    PlayerPrefs.SetInt(houseCounterKey, houseCount);
                    
                }
                break;

            case GameData.HouseUpgrade:
                {
                    int houseUpgrade = GetGameData(GameData.HouseUpgrade);
                    FPG.Networking.getInstance().sendUserActivity("HouseUpgrade_" + houseUpgrade.ToString());
                    FPG.Networking.getInstance().send_unpaid_user_battle_status(1, 1, 0, GameData.HouseUpgrade);

                    houseUpgrade++;
                    PlayerPrefs.SetInt(houseUpgradeKey, houseUpgrade);
                    
                }
                break;

            case GameData.EnemyCount:
                {
                    int enemyCount = GetGameData(GameData.EnemyCount);
                    //enemyCounter++;
                    //if (enemyCounter >= 3)
                    {
                        FPG.Networking.getInstance().send_unpaid_user_battle_status(1, 0, 0, GameData.EnemyCount);
                        //enemyCounter = 0;
                        enemyCount++;
                        PlayerPrefs.SetInt(enemyCounterKey, enemyCount);
                        
                    }

                }
                break;

            case GameData.BridgeCount:
                {
                    int bridgeCount = GetGameData(GameData.BridgeCount);
                    FPG.Networking.getInstance().sendUserActivity("BridgeLevel_" + bridgeCount.ToString());
                    FPG.Networking.getInstance().send_unpaid_user_battle_status(1, 1, 0, GameData.BridgeCount);

                    bridgeCount++;
                    PlayerPrefs.SetInt(bridgeCounterKey, bridgeCount);
                    
                }
                break;

                
        }
    }

    public static int GetGameData(GameData _data)
    {
        int data = 0;
        switch (_data)
        {
            case GameData.LevelCount:
                {
                    data = PlayerPrefs.GetInt(levelupCounterKey, 0);
                }
                break;

            case GameData.HouseCount:
                {
                    data = PlayerPrefs.GetInt(houseCounterKey, 1100000);
                }
                break;

            case GameData.HouseUpgrade:
                {
                    data = PlayerPrefs.GetInt(houseUpgradeKey, 11110000);
                }
                break;

            case GameData.EnemyCount:
                {
                    data = PlayerPrefs.GetInt(enemyCounterKey, 109400);
                }
                break;
            case GameData.BridgeCount:
                {
                    data = PlayerPrefs.GetInt(bridgeCounterKey, 10991000);
                }
                break;

            case GameData.WoodCount:
                {
                    data = PlayerPrefs.GetInt(woodCounterKey, 0);
                }
                break;
        }

        return data;
    }
    #endregion
}
