using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
//using UnityEditor.AI;
using DG.Tweening;
using TMPro;

public class PlayerController : MonoBehaviour
{
    [HideInInspector] public Animator anim;
    [HideInInspector] public Rigidbody rb;
    public LayerMask clickable;
    public NavMeshAgent myAgent;
    public bool running;
    public bool dead;
    public float tapDelay=0;
    bool tapped;
    public float footstepDelay = 0;
    public float footstepAudioThresold = 0.3f;
    public GameObject dirtTrailFX;
    [Header("Player Stats")]
    [HideInInspector] public string healthKey = "healthkey";
    public int health;
    public int maxHealth;
    [Header("WEAPONS")]
    public GameObject axe;
    public GameObject gun;
    public GameObject hammer;

    [Header("Shooting")]
    public GameObject bullet;
    public GameObject bulletForTrippleShoot;
    public float shootDelay;
    public float shootDelayMax;
    public Transform[] firePoints;
    public Transform shellPoint;
    public GameObject bulletShellPrefab;
    public bool canShoot;
    public GameObject[] enemies;
    public GameObject closestEnemy;
    public bool enemyContact;
    public GameObject muzzleFlash;
    private Transform target;

    public float range = 15f;

    [Header("POWER UPS!")]
    [Header("Shield")]
    public GameObject shieldPrefab;
    public GameObject shieldPickup;
    public bool hasShield;
    public GameObject playerShield;
    public int shieldLevel;
    [Header("Triple Shoot")]
    public bool hasTripleShoot;
    public float tripleShootTimer;
    public float tripleShootTimerMax;
    [Header("LumberCraft")]
    public bool canChop;
    public GameObject axeCollider;
    public GameObject slashFX;
    public GameObject axeAnim;
    public GameObject axeTrail;
    public float woodPickRange = 5f;
    public GameObject targetCircle;
    [Header("Wood Stacking")]
    public GameObject[] woodStack;
    bool building;

    [Header("Bridge")]
    public Material bridgeOpqMat1;
    public Material bridgeOpqMat2;
    public Material groundOpaqueMat;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody>();
        anim = GetComponentInChildren<Animator>();
        myAgent = GetComponent<NavMeshAgent>();

        closestEnemy = null;
        enemyContact = false;
        canShoot = false;
      
        tripleShootTimer = tripleShootTimerMax;
        GetPlayerHealth();

        InitEmptyStack();
        WoodStackGain();
        if (GameManager.instance.totalWood == 0) woodStack[0].SetActive(false);

        hammer.SetActive(false);
        gun.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (!GameManager.instance.startGame) return;
        if (dead) return;

        if (canShoot)
        {
            AutoShoot();
            axe.SetActive(false);
        }
        else if (!canShoot && !building)
        {
            axe.SetActive(true);
        }
          
        if (closestEnemy == null)
        {
            gun.SetActive(false);
            anim.SetBool("shoot", false);
            canShoot = false;
            muzzleFlash.SetActive(false);
        }

        if(playerShield != null)
            playerShield.transform.Rotate((Vector3.up * 200 * Time.deltaTime));

        if (hasTripleShoot)
        {
            tripleShootTimer -= Time.deltaTime;
            if (tripleShootTimer <= 0)
            {
                hasTripleShoot = false;
                tripleShootTimer = tripleShootTimerMax;
            }
                
        }

        if (!canChop)
        {
            anim.SetBool("chopB", false); 
            anim.SetBool("chop 2B", false);
            anim.SetBool("chop360", false);
        }
        //closestWood = NearbyWood();

    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, range);
        Gizmos.color = Color.red;
    }

    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Port"))
        {
            GameManager.instance.SwitchControl(ControlType.Boat);
        }

        if (other.gameObject.CompareTag("bot"))
        {
            GameManager.botType = (int)other.gameObject.GetComponent<BotController>().bc;
            GameManager.UpdateGameData(GameData.EnemyCount);
        }

        if (other.gameObject.CompareTag("water"))
        {
            PlayerDeathWater();
        }

        if (other.gameObject.CompareTag("Coin"))
        {
            GameManager.instance.GiveCoin(1);
            Destroy(other.gameObject);
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.coinPickSFX);
        }

        if (other.gameObject.CompareTag("enemyProjectile") && !hasShield)
        {
            TakeDamage(2);

            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("bomb"))
        {
            TakeDamage(20);
            GameObject pop = Instantiate(GameManager.instance.hitFX, transform.position, Quaternion.identity);
            pop.transform.localScale *= 2;
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("enemySniperProjectile") && !hasShield)
        {
            TakeDamage(10);

            Destroy(other.gameObject);

            Camera.main.transform.DOShakeRotation(0.45f, 1, 10, 90);
        }

        if (other.gameObject.CompareTag("playerShieldPickup") && !hasShield)
        {
            shieldLevel = 2;
            hasShield = true;
            GameObject shieldIns = Instantiate(shieldPrefab, transform.position, shieldPrefab.transform.rotation);
            shieldIns.transform.parent = transform;
            playerShield = shieldIns;
            Destroy(other.gameObject);
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.tripleFirePickSFX);
        }
        if (other.gameObject.CompareTag("TripleShootPickup") && !hasTripleShoot)
        {
            hasTripleShoot = true;
            Destroy(other.gameObject);
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.tripleFirePickSFX);
        }
        if (other.gameObject.CompareTag("playerHealthPickup"))
        {
            if(health < maxHealth - 100)
            {
                health += 100;
                PlayerPrefs.SetInt(healthKey, health);
                UIManager.instance.healthBar.fillAmount = (float)health / (float)maxHealth;
                Destroy(other.gameObject);
                SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.healthPickSFX);
            }
            else
            {
                health = maxHealth;
                PlayerPrefs.SetInt(healthKey, health);
                UIManager.instance.healthBar.fillAmount = (float)health / (float)maxHealth;
                Destroy(other.gameObject);
                SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.healthPickSFX);
            }
        }
        if (other.gameObject.CompareTag("coin"))
        {
            GameManager.instance.GiveCoin(1);
            UIManager.instance.txtCoinCountInGame.text = GameManager.instance.totalCoin.ToString();
            Destroy(other.gameObject);
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.coinPickSFX);
        }

        if (other.gameObject.CompareTag("levelendportal"))
        {
            GameManager.instance.LevelCompleteUponEnteringPortal();
        }
        if (other.gameObject.CompareTag("levelStartPortal"))
        {
            GameManager.instance.WarPortal();
        }
        
        if (other.gameObject.CompareTag("treeWood") && !dead && !canShoot)
        {
            //transform.DOLookAt(other.gameObject.transform.position, 0.2f);
            transform.DOLookAt(new Vector3(other.gameObject.transform.position.x, transform.position.y, other.gameObject.transform.position.z), .25f);
            //axeAnim.GetComponent<Animator>().SetTrigger("axeSlash");

            canChop = true;
            axeTrail.SetActive(true);

            if (other.gameObject.GetComponentInParent<TreeController>().treeType == TreeType.normal)
            {
                anim.speed = 1.5f;

                if (anim.GetComponent<Animator>().GetBool("run"))
                {
                    anim.GetComponent<Animator>().SetTrigger("runChop");
                }
                else if (anim.GetComponent<Animator>().GetBool("walk"))
                {
                    anim.GetComponent<Animator>().SetTrigger("walkChop");
                }
                else
                {
                    int randChopAnim = Random.Range(0, 1);

                    if (randChopAnim == 1)
                    {
                        anim.SetBool("chopB", true);
                        anim.SetTrigger("chop");
                    }

                    else if (randChopAnim == 0)
                    {
                        anim.SetBool("chop 2B", true);
                        anim.SetTrigger("chop 2");
                    }

                }
            }
            if (other.gameObject.GetComponentInParent<TreeController>().treeType == TreeType.adTree)
            {
                anim.speed = 1.5f;
                anim.SetBool("chop360", true);

            }



            StartCoroutine(AxeColliderRoutine(other.gameObject));
            
            hammer.SetActive(false);
            axe.SetActive(true);
            //Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("metalSource") && !dead)
        {
            transform.DOLookAt(other.gameObject.transform.position, 0.2f);
            //axeAnim.GetComponent<Animator>().SetTrigger("axeSlash");
            anim.GetComponent<Animator>().SetTrigger("chop");
            StartCoroutine(AxeColliderRoutine(other.gameObject));
            slashFX.GetComponent<ParticleSystem>().Play();
            hammer.SetActive(false);
            axe.SetActive(true);
            //Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("triggerZone") && !building && !dead && !canShoot)
        {
            if (other.gameObject.GetComponentInParent<BuildingController>().isComplete)
            {
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.shop)
                {
                    //trade
                    if (GameManager.instance.totalWood >= 22)
                        UIManager.instance.TradingShopOpen();
                }
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.lumberMill)
                {
                    //mill wood collection
                    UIManager.instance.LumberMillOpen();

                    Vector3 plusPos = new Vector3(gameObject.transform.position.x, gameObject.transform.position.y + 5, gameObject.transform.position.z);
                    GameObject plusIns = Instantiate(GameManager.instance.woodPlusTextPopupPrefab, plusPos, GameManager.instance.woodPlusTextPopupPrefab.transform.rotation);
                    plusIns.GetComponent<TextMeshPro>().text = "+" + other.gameObject.GetComponentInParent<MillController>().millWoodCount.ToString();
                    Destroy(plusIns, 0.5f);

                    other.gameObject.GetComponentInParent<MillController>().GiveWood();
                    WoodStackGain();
                    UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
                    
                }
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.housecm)
                {
                    //trade
                    UIManager.instance.houseOpen(other.gameObject.GetComponentInParent<BuildingController>().homeID, other.gameObject);

                }
            }
            if (other.gameObject.GetComponentInParent<BuildingController>().isPending)
            {
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.Small_Island)
                {
                    //small island 
                    UIManager.instance.SmallIslandOpen(other.gameObject.GetComponentInParent<BuildingController>().homeID, other.gameObject);
                }
            }
        }

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.CompareTag("bot"))
        {
            closestEnemy = GetClosestEnemy();
            enemyContact = true;
            canShoot = true;
            transform.DOLookAt(closestEnemy.transform.position, 0.25f);
        }
        if (other.gameObject.CompareTag("triggerZone") && !building && !dead && !canShoot)
        {
            if (!other.gameObject.GetComponentInParent<BuildingController>().isComplete)
            {

                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.house)
                    StartCoroutine(BuildHomeRoutine(other.gameObject));
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.bridge)
                    if (!other.transform.parent.GetComponent<Collider>().enabled)
                    {
                        StartCoroutine(BuildBridgeRoutine(other.gameObject));
                    }
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.shop)
                    StartCoroutine(BuildShopRoutine(other.gameObject));
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.lumberMill)
                    StartCoroutine(BuildLumberMillRoutine(other.gameObject));

            }
            if (!other.gameObject.GetComponentInParent<BuildingController>().isPending)
            {
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.Small_Island)
                    StartCoroutine(BuildSmallIslandRoutine(other.gameObject));
            }
            if (other.gameObject.GetComponentInParent<BuildingController>().isComplete)
            {
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.shop)
                {
                    //trade
                    
                    //if(GameManager.instance.totalWood >= 10)
                    //{
                    //    GameManager.instance.TakeWood(10);
                    //    GameManager.instance.GiveCoin(5);
                    //    WoodStackLoss();
                    //    UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
                    //    UIManager.instance.txtCoinCountInGame.text = GameManager.instance.totalCoin.ToString();
                    //    SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.coinPickSFX);
                    //}
                }

            }
        }
    }

    public IEnumerator BuildHomeRoutine(GameObject other)
    {
        building = true;
        yield return new WaitForSeconds(0.075f);
        
        //building home
        var buildControl = other.gameObject.GetComponentInParent<BuildingController>();
        if (GameManager.instance.totalWood > 0)
        {
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildProcessSFX);
            FPG.HapticController.PlayBuildHaptic(SoundManager.sharedInstance.buildProcessSFX.length);
            buildControl.requiredWood--;
            buildControl.txtReqWood.text = buildControl.requiredWood.ToString();
            GameManager.instance.TakeWood(1);
            UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            WoodStackLoss();

            GameObject woodLogIns = Instantiate(GameManager.instance.woodLogPrefab, transform.position, GameManager.instance.woodLogPrefab.transform.rotation);
            woodLogIns.transform.DOMove(other.gameObject.transform.parent.position, 0.35f).OnComplete(() =>
            {
                Destroy(woodLogIns);
            });

            //temp solution
            if (buildControl.requiredWood == 16)
            {
                buildControl.buildStages[0].SetActive(true);
                BuildFX(other.gameObject);
             
            }
            if (buildControl.requiredWood == 12)
            {
                buildControl.buildStages[1].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 8)
            {
                buildControl.buildStages[2].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 4)
            {
                buildControl.buildStages[3].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 0)
            {
                buildControl.buildStages[4].SetActive(true);
                BuildFX(other.gameObject);
                buildControl.isComplete = true;
            }
            if (buildControl.requiredWood <= 0)
            {
                //hammer.SetActive(false);
                //axe.SetActive(true);
                //upon build completion
                SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildCompleteSFX);
                FPG.HapticController.PlayBuildCompletedHaptic(SoundManager.sharedInstance.buildCompleteSFX.length);
                Destroy(other.gameObject);
                buildControl.buildCanvas.SetActive(false);
                GameManager.instance.buildingMade++;
                UIManager.instance.txtBuildingCountInGame.text = (GameManager.instance.buildingMade + "/" + GameManager.instance.buildingToMake).ToString();
                GameManager.instance.CheckLevelProgression();
                GameObject conFX = Instantiate(GameManager.instance.confettiFX, buildControl.transform.position, GameManager.instance.confettiFX.transform.rotation);
                Destroy(conFX, 5f);

                //TutorialManager.instance.mill.SetActive(true);

                //GameManager.instance.cameraCon.GetComponent<CameraController>().enabled = false;
                //Camera.main.transform.DOMove(TutorialManager.instance.houseCompleteCamPos.transform.position, 3.5f).OnComplete(()=>
                //{
                //    GameManager.instance.cameraCon.GetComponent<CameraController>().enabled = true;
                //    Camera.main.transform.DORotateQuaternion(GameManager.instance.gameCamPos.transform.rotation, 0.2f);
                //});
                //Camera.main.transform.DORotateQuaternion(TutorialManager.instance.houseCompleteCamPos.transform.rotation, 1.5f);

                if(buildControl.homeID==0)
                    PlayerPrefs.SetInt(GameManager.instance.homeKey, 1);
                if (buildControl.homeID ==1)
                    PlayerPrefs.SetInt(GameManager.instance.home2Key, 1);
                if (buildControl.homeID == 2)
                    PlayerPrefs.SetInt(GameManager.instance.home3Key, 1);

                TutorialManager.instance.Start();
                GameManager.UpdateGameData(GameData.HouseCount);
            }
        }
        building = false;
    }
    public IEnumerator BuildBridgeRoutine(GameObject other)
    {
        building = true;
        yield return new WaitForSeconds(0.1f);
        var buildControl = other.gameObject.GetComponentInParent<BuildingController>();
        if (GameManager.instance.totalWood > 0)
        {
            buildControl.bridgeCollider.SetActive(false);
            other.transform.parent.GetComponent<Collider>().enabled = true;
            other.GetComponent<Collider>().enabled = false;
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildProcessSFX);
            FPG.HapticController.PlayBuildHaptic(SoundManager.sharedInstance.buildProcessSFX.length);
            buildControl.requiredWood--;
            buildControl.txtReqWood.text = buildControl.requiredWood.ToString();
            GameManager.instance.TakeWood(1);
            UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            buildControl.hasWood++;
            WoodStackLoss();

            GameObject woodLogIns = Instantiate(GameManager.instance.woodLogPrefab, transform.position, GameManager.instance.woodLogPrefab.transform.rotation);
            woodLogIns.transform.DOMove(other.gameObject.transform.position, 0.5f).OnComplete(() =>
            {
                Destroy(woodLogIns);
            });

            int index = buildControl.hasWood - 1;
            MeshRenderer renderer = buildControl.buildStages[index].GetComponent<MeshRenderer>();
            renderer.enabled = true;
            renderer.material.DOFade(1, .25f).OnComplete(() =>
            {
                if (index % 2 == 0) { renderer.material = bridgeOpqMat1; }
                else { renderer.material = bridgeOpqMat2; }
            });

            //for (int i = 0; i < buildControl.buildStages.Length; i++)
            //{
            //    buildControl.buildStages[buildControl.hasWood - 1].SetActive(true);

            //}
            GameManager.instance.surface.BuildNavMesh();
            //UnityEditor.AI.NavMeshBuilder.BuildNavMesh();

            if (buildControl.requiredWood <= 0)
            {
                buildControl.bridgeCollider.SetActive(false);
                //hammer.SetActive(false);
                //axe.SetActive(true);
                //upon build completion
                SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildCompleteSFX);
                FPG.HapticController.PlayBuildCompletedHaptic(SoundManager.sharedInstance.buildCompleteSFX.length);
                Destroy(other.gameObject);
                buildControl.buildCanvas.SetActive(false);
                //GameManager.instance.buildingMade++;
                //GameManager.instance.CheckLevelProgression();
                GameObject conFX = Instantiate(GameManager.instance.confettiFX, other.gameObject.transform.position, GameManager.instance.confettiFX.transform.rotation);
                Destroy(conFX, 1.5f);

                BridgeController.instance.bridgeStatusData[buildControl.bridgeID] = 1;
                PlayerPrefs.SetInt(BridgeController.instance.bridgeDataKey + buildControl.bridgeID, BridgeController.instance.bridgeStatusData[buildControl.bridgeID]);

                GameManager.UpdateGameData(GameData.BridgeCount);
            }
        }
        else
        {
            if (buildControl.requiredWood <= 0) yield break;
            Transform build = buildControl.buildStages[buildControl.hasWood].transform;
            buildControl.bridgeCollider.transform.localPosition = new Vector3(build.localPosition.x, build.localPosition.y, build.localPosition.z + .5f);
            buildControl.bridgeCollider.SetActive(true);
        }
        building = false;
    }

    public GameObject currentSmallIsland;

    public IEnumerator BuildSmallIslandRoutine(GameObject other)
    {
        building = true;
        yield return new WaitForSeconds(0.1f);
        currentSmallIsland = other;
        var buildControl = other.gameObject.GetComponentInParent<BuildingController>();
        if (GameManager.instance.totalWood > 0)
        {
            buildControl.isPending = true;
            buildControl.bridgeCollider.SetActive(false);
            //other.transform.parent.GetComponent<Collider>().enabled = true;

            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildProcessSFX);
            FPG.HapticController.PlayBuildHaptic(SoundManager.sharedInstance.buildProcessSFX.length);
            buildControl.requiredWood--;
            buildControl.txtReqWood.text = buildControl.requiredWood.ToString();
            GameManager.instance.TakeWood(1);
            UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            buildControl.hasWood++;
            WoodStackLoss();

            GameObject woodLogIns = Instantiate(GameManager.instance.woodLogPrefab, transform.position, GameManager.instance.woodLogPrefab.transform.rotation);
            woodLogIns.transform.DOMove(other.gameObject.transform.position, 0.5f).OnComplete(() =>
            {
                Destroy(woodLogIns);
            });

            buildControl.radialProgress.gameObject.SetActive(true);
            buildControl.radialProgress.StartFill();
            UIManager.instance.SmallIslandOpen(buildControl.homeID, other.gameObject);
            //UnityEditor.AI.NavMeshBuilder.BuildNavMesh();           
        }
        else
        {
            if (buildControl.requiredWood <= 0) yield break;
            Transform build = buildControl.buildStages[buildControl.hasWood].transform;
            buildControl.bridgeCollider.transform.localPosition = new Vector3(build.localPosition.x, build.localPosition.y, build.localPosition.z + .5f);
            buildControl.bridgeCollider.SetActive(true);
        }
        building = false;
    }

    public void FinishSmallIslandBuild()
    {
        GameObject other = currentSmallIsland;
        var buildControl = other.gameObject.GetComponentInParent<BuildingController>();

        if (buildControl.requiredWood <= 0)
        {
            other.GetComponent<Collider>().enabled = false;
            buildControl.isPending = false;
            buildControl.isComplete = true;

            //int index = buildControl.hasWood - 1;
            //buildControl.buildStages[index].transform.DOScale(new Vector3(0.2f, 2f, 0.4f), .45f).SetEase(Ease.OutBack);
            //MeshRenderer renderer = buildControl.buildStages[index].GetComponent<MeshRenderer>();
            //renderer.enabled = true;
            //renderer.material.DOFade(1, .5f).OnComplete(() =>
            //{
            //    renderer.material = groundOpaqueMat;
            //});
            for (int i = 0; i < buildControl.buildStages.Length; i++)
            {
                int index = i;
                buildControl.buildStages[index].transform.DOScale(Vector3.one, .45f).SetEase(Ease.OutBack);
                MeshRenderer renderer = buildControl.buildStages[index].GetComponent<MeshRenderer>();
                renderer.enabled = true;
                renderer.material.DOFade(1, .45f).OnComplete(() =>
                {
                    //renderer.material = groundOpaqueMat;
                    buildControl.buildStages[index].gameObject.SetActive(false);
                    buildControl.tileToActivate.SetActive(true);
                });
            }

            GameManager.instance.surface.BuildNavMesh();
            buildControl.bridgeCollider.SetActive(false);
            hammer.SetActive(false);
            axe.SetActive(true);
            //upon build completion
            UIManager.instance.smallIslandPopupPanel.SetActive(false);
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildCompleteSFX);
            FPG.HapticController.PlayBuildCompletedHaptic(SoundManager.sharedInstance.buildCompleteSFX.length);
            ///Destroy(other.gameObject);
            buildControl.buildCanvas.SetActive(false);
            //GameManager.instance.buildingMade++;
            //GameManager.instance.CheckLevelProgression();
            GameObject conFX = Instantiate(GameManager.instance.confettiFX, other.gameObject.transform.position, GameManager.instance.confettiFX.transform.rotation);
            Destroy(conFX, 1.5f);

            BridgeController.instance.bridgeStatusData[buildControl.bridgeID] = 1;
            PlayerPrefs.SetInt(BridgeController.instance.bridgeDataKey + buildControl.bridgeID, BridgeController.instance.bridgeStatusData[buildControl.bridgeID]);
        }
    }

    public IEnumerator BuildShopRoutine(GameObject other)
    {
        //anim.SetBool("magicBuild", true);
        building = true;
        yield return new WaitForSeconds(0.125f);
        //building home
        var buildControl = other.gameObject.GetComponentInParent<BuildingController>();
        if (GameManager.instance.totalWood > 0)
        {
            axe.SetActive(false);
            //anim.SetBool("magicBuild",true);
            //anim.SetTrigger("magicBuildT");
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildProcessSFX);
            FPG.HapticController.PlayBuildHaptic(SoundManager.sharedInstance.buildProcessSFX.length);
            buildControl.requiredWood--;
            buildControl.txtReqWood.text = buildControl.requiredWood.ToString();
            GameManager.instance.TakeWood(1);
            UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            WoodStackLoss();
            GameObject woodLogIns = Instantiate(GameManager.instance.woodLogPrefab, transform.position, GameManager.instance.woodLogPrefab.transform.rotation);
            woodLogIns.transform.DOMove(other.gameObject.transform.parent.position, 0.5f).OnComplete(() =>
            {
                Destroy(woodLogIns);
            });

            //temp solution
            if (buildControl.requiredWood == 32)
            {
                buildControl.buildStages[0].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 24)
            {
                buildControl.buildStages[1].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 18)
            {
                buildControl.buildStages[2].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 10)
            {
                buildControl.buildStages[3].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 0)
            {
                buildControl.buildStages[4].SetActive(true);
                BuildFX(other.gameObject);
                buildControl.isComplete = true;
            }

            if (buildControl.requiredWood <= 0)
            {
                //anim.SetBool("magicBuild", false);
                if (GameManager.instance.totalWood >= 22)
                    UIManager.instance.TradingShopOpen();
                //hammer.SetActive(false);
                axe.SetActive(true);
                //upon build completion
                SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildCompleteSFX);
                FPG.HapticController.PlayBuildCompletedHaptic(SoundManager.sharedInstance.buildCompleteSFX.length);
                //Destroy(other.gameObject);
                buildControl.buildCanvas.SetActive(false);
                GameManager.instance.buildingMade++;
                UIManager.instance.txtBuildingCountInGame.text = (GameManager.instance.buildingMade + "/" + GameManager.instance.buildingToMake).ToString();
                GameManager.instance.CheckLevelProgression();
                GameObject conFX = Instantiate(GameManager.instance.confettiFX, buildControl.transform.position, GameManager.instance.confettiFX.transform.rotation);
                Destroy(conFX, 5f);

                PlayerPrefs.SetInt(GameManager.instance.shopKey, 1);
                GameManager.UpdateGameData(GameData.HouseCount);
            }
        }
        building = false;
    }
    public IEnumerator BuildLumberMillRoutine(GameObject other)
    {
        building = true;
        yield return new WaitForSeconds(0.125f);
        //building home
        var buildControl = other.gameObject.GetComponentInParent<BuildingController>();
        if (GameManager.instance.totalWood > 0)
        {
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildProcessSFX);
            FPG.HapticController.PlayBuildHaptic(SoundManager.sharedInstance.buildProcessSFX.length);
            buildControl.requiredWood--;
            buildControl.txtReqWood.text = buildControl.requiredWood.ToString();
            GameManager.instance.TakeWood(1);
            UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            WoodStackLoss();
            GameObject woodLogIns = Instantiate(GameManager.instance.woodLogPrefab, transform.position, GameManager.instance.woodLogPrefab.transform.rotation);
            woodLogIns.transform.DOMove(other.gameObject.transform.parent.position, 0.5f).OnComplete(() =>
            {
                Destroy(woodLogIns);
            });

            //temp solution
            if (buildControl.requiredWood == 25)
            {
                buildControl.buildStages[0].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 15)
            {
                buildControl.buildStages[1].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 10)
            {
                buildControl.buildStages[2].SetActive(true);
                BuildFX(other.gameObject);
            }
            if (buildControl.requiredWood == 0)
            {
                buildControl.buildStages[3].SetActive(true);
                BuildFX(other.gameObject);
                buildControl.isComplete = true;
            }
            //if (buildControl.requiredWood == 0)
            //{
            //    buildControl.buildStages[4].SetActive(true);
            //    BuildFX(other.gameObject);
            //    buildControl.isComplete = true;
            //}
            //if (buildControl.requiredWood == 5)
            //{
            //    buildControl.buildStages[5].SetActive(true);
            //    BuildFX(other.gameObject);
            //}
            //if (buildControl.requiredWood == 0)
            //{
            //    buildControl.buildStages[6].SetActive(true);
            //    BuildFX(other.gameObject);
            //    buildControl.isComplete = true;
            //}

            if (buildControl.requiredWood <= 0)
            {
                
                //hammer.SetActive(false);
                //axe.SetActive(true);
                //upon build completion
                SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.buildCompleteSFX);
                FPG.HapticController.PlayBuildCompletedHaptic(SoundManager.sharedInstance.buildCompleteSFX.length);
                //Destroy(other.gameObject);
                buildControl.buildCanvas.SetActive(false);
                GameManager.instance.buildingMade++;
                UIManager.instance.txtBuildingCountInGame.text = (GameManager.instance.buildingMade + "/" + GameManager.instance.buildingToMake).ToString();
                GameManager.instance.CheckLevelProgression();
                GameObject conFX = Instantiate(GameManager.instance.confettiFX, buildControl.transform.position, GameManager.instance.confettiFX.transform.rotation);
                Destroy(conFX, 5f);

                //TutorialManager.instance.shop.SetActive(true);
                UIManager.instance.LumberMillOpen();
                //GameManager.instance.cameraCon.GetComponent<CameraController>().enabled = false;
                //Camera.main.transform.DOMove(TutorialManager.instance.millCompleteCamPos.transform.position, 3f).OnComplete(() =>
                //{
                //    UIManager.instance.LumberMillOpen();
                //    GameManager.instance.cameraCon.GetComponent<CameraController>().enabled = true;
                //    Camera.main.transform.DORotateQuaternion(GameManager.instance.gameCamPos.transform.rotation, 0.5f);
                //});
                //Camera.main.transform.DORotateQuaternion(TutorialManager.instance.millCompleteCamPos.transform.rotation, 1.5f);

                PlayerPrefs.SetInt(GameManager.instance.millKey, 1);

                GameManager.UpdateGameData(GameData.HouseCount);
            }
        }
        building = false;
    }
    void BuildFX(GameObject other)
    {
        //hammer.SetActive(true);
        //axe.SetActive(false);
        anim.SetTrigger("chop");
        GameObject buildFX = Instantiate(GameManager.instance.buildStageCompleteFX, other.transform.parent.position, GameManager.instance.buildStageCompleteFX.transform.rotation);
        Destroy(buildFX, 1.25f);
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("bot"))
        {
            enemyContact = false;
            canShoot = false;
            muzzleFlash.SetActive(false);
            anim.SetBool("shoot", false);
        }
        if (other.gameObject.CompareTag("triggerZone") && !building && !dead)
        {
            //anim.SetBool("magicBuild", false);
            //hammer.SetActive(false);
            axe.SetActive(true);
            //anim.SetBool("magicBuild", false);
            if (other.gameObject.GetComponentInParent<BuildingController>().isComplete)
            {
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.house)
                {
                    UIManager.instance.housePopupPanel.SetActive(false);
                    UIManager.instance.housePopupPanel.GetComponent<HousePopUp>().txtNotEnoughCoin.gameObject.SetActive(false);
                    UIManager.instance.housePopupPanel.GetComponent<HousePopUp>().notECPressed = false;
                }               
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.housecm)
                    UIManager.instance.housePopupPanel.SetActive(false);
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.shop)
                    UIManager.instance.shopTradingPopupPanel.SetActive(false);
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.lumberMill)
                {
                    UIManager.instance.lumbermillPopupPanel.SetActive(false);
                    UIManager.instance.lumbermillPopupPanel.GetComponent<MillPopUp>().txtNotEnoughCoin.gameObject.SetActive(false);
                    UIManager.instance.lumbermillPopupPanel.GetComponent<MillPopUp>().notECPressed = false;
                }     
            }

            if (other.gameObject.GetComponentInParent<BuildingController>().isPending)
            {
                if (other.gameObject.GetComponentInParent<BuildingController>().buildingType == BuildingType.Small_Island)
                    UIManager.instance.smallIslandPopupPanel.SetActive(false);
            }
        }

        if (other.gameObject.CompareTag("treeWood") && !dead && !canShoot)
        {
            canChop = false;
            //anim.SetBool("chopB", false); //anim.GetComponent<Animator>().SetTrigger("chop");
            //anim.SetBool("chop 2B", false);
        }
    }
    private void OnCollisionEnter(Collision other)
    {
        if (other.gameObject.CompareTag("pickupWood"))
        {
            GameManager.instance.GiveWood(1);
            WoodStackGain();
            UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            Vector3 plusPos = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 1, other.gameObject.transform.position.z);
            GameObject plusIns = Instantiate(GameManager.instance.woodPlusTextPopupPrefab, plusPos, GameManager.instance.woodPlusTextPopupPrefab.transform.rotation);
            Destroy(plusIns, 0.5f);
            Destroy(other.gameObject);
        }
        if (other.gameObject.CompareTag("pickupMetal"))
        {
            //GameManager.instance.GiveWood(1);
            //WoodStackManagement();
            //UIManager.instance.txtWoodCountInGame.text = GameManager.instance.totalWood.ToString();
            Vector3 plusPos = new Vector3(other.gameObject.transform.position.x, other.gameObject.transform.position.y + 1, other.gameObject.transform.position.z);
            GameObject plusIns = Instantiate(GameManager.instance.woodPlusTextPopupPrefab, plusPos, GameManager.instance.woodPlusTextPopupPrefab.transform.rotation);
            if(other.gameObject.GetComponent<Farmable>().resourceType == ResourceType.gold)
                plusIns.GetComponent<TextMeshPro>().color = Color.yellow;
            if (other.gameObject.GetComponent<Farmable>().resourceType == ResourceType.diamond)
                plusIns.GetComponent<TextMeshPro>().color = Color.green;
            Destroy(plusIns, 0.5f);
            Destroy(other.gameObject);
        }
        if (other.transform.GetComponentInParent<AdRewardTree>() != null)
        {
            AdRewardTree adRewardTree = other.transform.GetComponentInParent<AdRewardTree>();
            adRewardTree.GenerateTree();
        }
    }
    void InitEmptyStack()
    {
        for (int i = 0; i < woodStack.Length; i++)
        {
            woodStack[i].SetActive(false);
        }
    }
    public void WoodStackGain()
    {
        int woodIndex = GameManager.instance.totalWood / 5;
       
        for (int i = 0; i < woodStack.Length; i++)
        {
            if(i<= woodIndex && woodIndex < woodStack.Length)
            {
                woodStack[i].SetActive(true);
            }
        }
    }
    public void WoodStackLoss()
    {
        int woodIndex = GameManager.instance.totalWood / 5;

        for (int i = 0; i < woodStack.Length; i++)
        {
            if (woodIndex < woodStack.Length)
            {
                woodStack[woodIndex].SetActive(false);
            }
        }
    }
    public IEnumerator AxeColliderRoutine(GameObject x)
    {
        if (x.GetComponentInParent<TreeController>().treeType == TreeType.normal) yield return new WaitForSeconds(0.325f);
        else if (x.GetComponentInParent<TreeController>().treeType == TreeType.adTree) yield return new WaitForSeconds(0.25f);
        
        if (x!=null && x.CompareTag("treeWood"))
        {
            //x.transform.parent.transform.DOShakePosition(0.25f,0.2f);

            slashFX.GetComponent<ParticleSystem>().Play();
            //Camera.main.transform.DOShakeRotation(0.5f, 0.5f);
            x.transform.parent.GetComponent<TreeController>().leafFX.GetComponent<ParticleSystem>().Play();
            GameObject pop = Instantiate(GameManager.instance.hitTreeFX, x.transform.position, Quaternion.identity);
            Destroy(pop, 1f);
            if (x.GetComponentInParent<TreeController>().treeType == TreeType.normal)
            {
                Instantiate(GameManager.instance.woodPickupPrefab, x.transform.position, GameManager.instance.woodPickupPrefab.transform.rotation);
            }
            if (x.GetComponentInParent<TreeController>().treeType == TreeType.adTree)
            {
                Instantiate(GameManager.instance.woodPickupAdTreePrefab, x.transform.position, GameManager.instance.woodPickupAdTreePrefab.transform.rotation);
            }
            var clip = SoundManager.sharedInstance.GetRandomTreeCutSFX();
            SoundManager.sharedInstance.PlaySFX(clip);
            FPG.HapticController.PlayTreeChopHaptic(clip.length);
            axeCollider.SetActive(false);
            var clip2 = SoundManager.sharedInstance.GetRandomTreeCutGruntSFX();
            SoundManager.sharedInstance.PlaySFX(clip2);
            //wood health
            x.GetComponent<Farmable>().sourceHealth--;

            if (x.GetComponent<Farmable>().sourceHealth <= 0)
            {
                x.GetComponentInParent<TreeController>().treeHealth--;
                GameObject treetop = x.GetComponentInParent<TreeController>().treeTop;
                if (x.GetComponentInParent<TreeController>().treeType == TreeType.normal)
                {
                    treetop.transform.DOScaleX(treetop.transform.localScale.x - 0.25f, 0.1f);
                    treetop.transform.DOScaleZ(treetop.transform.localScale.z - 0.25f, 0.1f);
                }

                if (x.GetComponentInParent<TreeController>().treeHealth <= 0)
                {
                    Vector3 spawnPosOffset = new Vector3(x.GetComponentInParent<TreeController>().treeTop.transform.position.x, x.GetComponentInParent<TreeController>().treeTop.transform.position.y - 1.5f, x.GetComponentInParent<TreeController>().treeTop.transform.position.z);
                    x.GetComponentInParent<TreeController>().spawnPos.transform.position = spawnPosOffset;
                    treetop.GetComponent<BoxCollider>().enabled = false;
                    Destroy(treetop, 2f);

                    if(x.GetComponentInParent<TreeController>().treeType == TreeType.adTree)
                    {
                        x.GetComponentInParent<TreeController>().OnFarmComplete?.Invoke();
                    }
                    else
                    {
                        x.GetComponentInParent<TreeController>().canSpawn = true;
                    }


                    anim.speed = 1f;
                    GameManager.UpdateGameData(GameData.LevelCount);
                }
                Destroy(x);
                //Tree Y axis down amount
                if (x.GetComponentInParent<TreeController>().treeType == TreeType.normal)
                    x.transform.parent.transform.DOMoveY(x.transform.parent.transform.position.y - 2.6f, 0.35f);  
                if (x.GetComponentInParent<TreeController>().treeType == TreeType.adTree)
                    x.transform.parent.transform.DOMoveY(x.transform.parent.transform.position.y - 3.2f, 0.35f);
            }

            if (x.GetComponentInParent<TreeController>().treeType == TreeType.normal) yield return new WaitForSeconds(0.325f);
            else if (x.GetComponentInParent<TreeController>().treeType == TreeType.adTree) yield return new WaitForSeconds(0.25f);

            axeTrail.SetActive(false);
            axeCollider.SetActive(true);
            canChop = false;
        }
        if (x != null && x.CompareTag("metalSource"))
        {
            //Camera.main.transform.DOShakeRotation(0.5f, 0.5f);
            x.transform.DOShakePosition(0.2f,0.2f);

            SpawnPickupable(x);
            SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.hitStoneSFX);
            if (x.GetComponent<Farmable>().resourceType == ResourceType.gold)
            {
                GameObject pop = Instantiate(GameManager.instance.hitGoldFX, x.transform.position, Quaternion.identity);
                Destroy(pop, 1f);
            }
            if (x.GetComponent<Farmable>().resourceType == ResourceType.silver)
            {
                GameObject pop = Instantiate(GameManager.instance.hitSilverFX, x.transform.position, Quaternion.identity);
                Destroy(pop, 1f);
            }
            if (x.GetComponent<Farmable>().resourceType == ResourceType.diamond)
            {
                GameObject pop = Instantiate(GameManager.instance.hitDiamondFX, x.transform.position, Quaternion.identity);
                Destroy(pop, 1f);
            }
            axeCollider.SetActive(false);
            //source health
            x.GetComponent<Farmable>().sourceHealth--;
            if (x.GetComponent<Farmable>().sourceHealth == 2)
            {
                x.transform.DOScale(new Vector3(x.transform.localScale.x - 0.05f, x.transform.localScale.y - 0.05f, x.transform.localScale.z - 0.1f) , 0.2f);
            }
            if (x.GetComponent<Farmable>().sourceHealth <= 0)
            {
                SpawnPickupable5Stack(x);
                Destroy(x);
            }

            yield return new WaitForSeconds(0.5f);
            axeCollider.SetActive(true);
        }

    }
    void SpawnPickupable(GameObject x)
    {
        if (x.GetComponent<Farmable>().resourceType == ResourceType.gold)
        {
            GameObject goldIns = Instantiate(GameManager.instance.resourcePickupPrefabs[0], x.transform.position, GameManager.instance.resourcePickupPrefabs[0].transform.rotation);
            goldIns.transform.DOMoveY(goldIns.transform.position.y + 4, 0.2f);
        }
        if (x.GetComponent<Farmable>().resourceType == ResourceType.silver)
        {   
            GameObject silverIns = Instantiate(GameManager.instance.resourcePickupPrefabs[1], x.transform.position, GameManager.instance.resourcePickupPrefabs[1].transform.rotation);
            silverIns.transform.DOMoveY(silverIns.transform.position.y + 4, 0.2f);
        }
        if (x.GetComponent<Farmable>().resourceType == ResourceType.diamond)
        {
            GameObject diamIns = Instantiate(GameManager.instance.resourcePickupPrefabs[2], x.transform.position, GameManager.instance.resourcePickupPrefabs[2].transform.rotation);
            diamIns.transform.DOMoveY(diamIns.transform.position.y + 4, 0.2f);
        }
    }
    void SpawnPickupable5Stack(GameObject x)
    {
        if (x.GetComponent<Farmable>().resourceType == ResourceType.gold)
        {
            GameObject goldIns = Instantiate(GameManager.instance.resourcePickup5StacksPrefabs[0], x.transform.position, GameManager.instance.resourcePickup5StacksPrefabs[0].transform.rotation);
            goldIns.transform.DOMoveY(goldIns.transform.position.y + 4, 0.2f);
        }
        if (x.GetComponent<Farmable>().resourceType == ResourceType.silver)
        {
            GameObject silverIns = Instantiate(GameManager.instance.resourcePickup5StacksPrefabs[1], x.transform.position, GameManager.instance.resourcePickup5StacksPrefabs[1].transform.rotation);
            silverIns.transform.DOMoveY(silverIns.transform.position.y + 4, 0.2f);
        }
        if (x.GetComponent<Farmable>().resourceType == ResourceType.diamond)
        {
            GameObject diamIns = Instantiate(GameManager.instance.resourcePickup5StacksPrefabs[2], x.transform.position, GameManager.instance.resourcePickup5StacksPrefabs[2].transform.rotation);
            diamIns.transform.DOMoveY(diamIns.transform.position.y + 4, 0.2f);
        }
    }
    public void AutoShoot()
    {

        shootDelay += Time.deltaTime;
        if (shootDelay >= shootDelayMax)
        {
            axe.SetActive(false);
            gun.SetActive(true);
            anim.SetBool("shoot",true);
            //muzzleFlash.SetActive(true);
            //GameObject bshellIns = Instantiate(bulletShellPrefab, shellPoint.position, shellPoint.rotation);
            //Destroy(bshellIns, 1.5f);
            shootDelay = 0;

            var clip = SoundManager.sharedInstance.GetRandomShootSFX();
            SoundManager.sharedInstance.PlaySFX(clip);
            FPG.HapticController.PlayShootEnemyHaptic(clip.length);

            //triple shoot power up
            if (hasTripleShoot)
            {
                GameObject bulletDRIns = Instantiate(bulletForTrippleShoot, firePoints[0].position, firePoints[0].rotation);
                Destroy(bulletDRIns, 3f);
                GameObject bulletDRInss = Instantiate(bulletForTrippleShoot, firePoints[1].position, firePoints[1].rotation);
                Destroy(bulletDRInss, 3f);
                GameObject bulletDLIns = Instantiate(bulletForTrippleShoot, firePoints[2].position, firePoints[2].rotation);
                Destroy(bulletDLIns, 3f);
            }
            else
            {
                GameObject bulletIns = Instantiate(bullet, firePoints[0].position, firePoints[0].rotation);
                Destroy(bulletIns, 3f);
                PlayerBullet b = bulletIns.GetComponent<PlayerBullet>();

                if (b != null)
                    b.GetTarget(target);
            }
    
        }
    }
    public GameObject GetClosestEnemy()
    {
        enemies = GameObject.FindGameObjectsWithTag("bot");
        float closestDistance = Mathf.Infinity;
        GameObject tran = null;

        foreach (GameObject go in enemies)
        {
            float currentDistance;
            currentDistance = Vector3.Distance(transform.position, go.transform.position);
            if (currentDistance < closestDistance)
            {
                closestDistance = currentDistance;
                tran = go;

            }
        }

        if (tran != null && closestDistance <= range)
        {
            target = tran.transform;
        }
        else
            target = null;

        return tran;
    }
    public void TakeDamage(int dmg)
    {
        health -= dmg;
        PlayerPrefs.SetInt(healthKey,health);
        UIManager.instance.healthBar.fillAmount = (float)health / (float)maxHealth;
        if (health <= 0)
        {
            PlayerDeath();
        }

        GameObject dmgPop = Instantiate(GameManager.instance.dmgTextPopupPrefab, transform.position, Quaternion.identity);
        dmgPop.transform.GetComponent<TextMeshPro>().text = "-" + dmg * 10;
        //dmgPop.transform.GetComponent<TextMeshPro>().DOFade(50, .3f);
        dmgPop.transform.DOMoveY(dmgPop.transform.position.y + 2, 0.3f);
        Destroy(dmgPop, 0.3f);
    }
    public void PlayerDeath()
    {
        anim.SetTrigger("die");
        dead = true;
        enabled = false;
        myAgent.enabled = false;
        rb.isKinematic = true;
        muzzleFlash.SetActive(false);
        UIManager.instance.gamePanel.SetActive(false);
        StartCoroutine(UIManager.instance.GameOverPanelDelayRoutine());

        var clip = SoundManager.sharedInstance.GetRandomBotkillSFX();
        SoundManager.sharedInstance.PlaySFX(clip);
        FPG.HapticController.PlayEnemyDestroyedHaptic(clip.length);

        health = maxHealth;
        PlayerPrefs.SetInt(healthKey, health);

        GameManager.instance.respawnStatus = 1;
        PlayerPrefs.SetInt(GameManager.instance.respawnKey, GameManager.instance.respawnStatus);
        FPG.Networking.getInstance().send_unpaid_user_battle_status(0, 0, 1, GameData.EnemyCount);
    }
    public void PlayerDeathWater()
    {
        GameObject waterIns = Instantiate(GameManager.instance.waterSplashFX, transform.position, GameManager.instance.waterSplashFX.transform.rotation);
        Destroy(waterIns, 2f);
        targetCircle.SetActive(false);
        InitEmptyStack();
        gameObject.transform.GetChild(0).transform.gameObject.SetActive(false);
        anim.SetTrigger("die");
        dead = true;
        enabled = false;
        myAgent.enabled = false;
        GameManager.instance.cameraCon.GetComponent<CameraController>().enabled = false;
        muzzleFlash.SetActive(false);
        UIManager.instance.gamePanel.SetActive(false);
        StartCoroutine(UIManager.instance.GameOverPanelDelayRoutine());

        var clip = SoundManager.sharedInstance.GetRandomBotkillSFX();
        SoundManager.sharedInstance.PlaySFX(clip);
        FPG.HapticController.PlayEnemyDestroyedHaptic(clip.length);

        health = maxHealth;
        PlayerPrefs.SetInt(healthKey, health);

        GameManager.instance.respawnStatus = 1;
        PlayerPrefs.SetInt(GameManager.instance.respawnKey, GameManager.instance.respawnStatus);
    }
    public void CompleteLevel()
    {
        anim.SetTrigger("die");
        dead = true;
        enabled = false;
        myAgent.enabled = false;
        rb.isKinematic = true;
        muzzleFlash.SetActive(false);
        UIManager.instance.gamePanel.SetActive(false);
        UIManager.instance.levelCompletePanel.SetActive(true);
        //SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.GetRandomBotkillSFX());
    }

    public void GetPlayerHealth()
    {
        health = PlayerPrefs.GetInt(healthKey, maxHealth);
        UIManager.instance.healthBar.fillAmount = (float)health / (float)maxHealth;
    }
}
