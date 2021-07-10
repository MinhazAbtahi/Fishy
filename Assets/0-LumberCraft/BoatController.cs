using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;
using LumberCraft;
using System.Collections;

public class BoatController : MonoBehaviour
{
    [Header("General:")]
    public Transform visual;
    public GameObject playerVisual;

    [Header("Fishing: ")]
    public ResourceStack resourceStack;
    private NPC closestFish;
    public int currentFishCount;
    public int maxFishCount = 5;
    public float currentTime;
    public float time = .25f;
    private bool done;
    private bool isFishing;   
    public GameObject detector;
    public Transform fishStoreTarget;
    public GameObject storeIndicator;

    [Header("Bobbing:")]
    public float bobbingMultiplier = .5f;
    public float bobbingSpeed = .5f;
    private float originalY;

    [Header("Rolling:")]
    public float rollingMultiplier = .4f;
    public float rollSpeed = 10f;

    [Header("Pitching")]
    public float pitchingMultiplier = .4f;    
    public float pitchSpeed = 2f;
    private Quaternion startRotation;

    [Header("UI")]
    public Canvas ui;
    public Image progressBar;
    private Image progressFill;
    public TextMeshProUGUI currentFishText;
    public TextMeshProUGUI maxFishText;
    public TextMeshProUGUI noSpaceText;
    public Color green;
    public Color red;
    public Window_QuestPointer questPointer;

    void Start()
    {
        originalY = this.transform.position.y;
        startRotation = transform.rotation;
        progressFill = progressBar.transform.GetChild(0).GetComponent<Image>();
        currentFishText.text = currentFishCount.ToString();
        maxFishText.text = " / " + maxFishCount.ToString();
    }

    void Update()
    {
        SimulateBoatMovement();      
        if (isFishing) CatchFish();       
        ui.transform.LookAt(Camera.main.transform);
        questPointer.Show(fishStoreTarget.position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("NPC"))
        {
            StartFishing(other.gameObject);
        }

        if (other.CompareTag("Port"))
        {
            playerVisual.SetActive(false);
            GameManager.instance.SwitchControl(ControlType.Player);
        }

        if (other.CompareTag("FishStore"))
        {
            if (currentFishCount > 0)
            {
                StartCoroutine(SellFish(other.gameObject));               
            }       
        }
    }

    #region BoatMovement
    private void SimulateBoatMovement()
    {
        BobUpAndDown();
        RollSideToSide();
        PitchFrontAndBack();
    }

    void BobUpAndDown()
    {
        visual.position = new Vector3(visual.position.x,
                                         originalY + ((float)Math.Sin(Time.time) * bobbingMultiplier * bobbingSpeed),
                                         visual.position.z);
    }

    void RollSideToSide()
    {
        float angle = Mathf.Sin(Time.time * rollingMultiplier) * rollSpeed;
        visual.localRotation = startRotation * Quaternion.AngleAxis(angle, new Vector3(visual.localRotation.x, visual.localRotation.y, 1f)); ;
    }

    void PitchFrontAndBack()
    {
        float angle = Mathf.Sin(Time.time * rollingMultiplier) * pitchSpeed;
        visual.localRotation = startRotation * Quaternion.AngleAxis(angle, new Vector3(-1f, visual.localRotation.y, visual.localRotation.z));
    }
    #endregion

    #region Fishing
    private void StartFishing(GameObject other)
    {
        if (!isFishing)
        {
            isFishing = true;
            done = false;
            
            if (currentFishCount < maxFishCount)
            {
                other.GetComponent<Collider>().enabled = false;
                progressFill.color = green;
                ++currentFishCount;               
                progressBar.gameObject.SetActive(true);
                detector.SetActive(true);
                closestFish = other.transform.root.GetComponent<NPC>();
            }
            else
            {
                //progressFill.color = red;
                noSpaceText.DOFade(1, .5f);
                noSpaceText.rectTransform.DOAnchorPosY(0f, .5f).OnComplete(() =>
                {
                    noSpaceText.DOFade(0, .25f).SetDelay(.5f).OnComplete(()=> noSpaceText.rectTransform.DOAnchorPosY(-3f, 0f));
                });
            }          
        }
    }

    private void CatchFish()
    {
        if (currentTime < time)
        {
            currentTime += Time.deltaTime;
        }
        else
        {
            if (!done)
            {
                done = true;
                isFishing = false;
                progressBar.gameObject.SetActive(false);
                detector.SetActive(false);
                if(closestFish) closestFish.HookFish(resourceStack);
                currentTime = 0f;
            }
        }

        currentFishText.text = currentFishCount.ToString();
        maxFishText.text = " / " + maxFishCount.ToString();
        progressFill.fillAmount = (float)currentFishCount / (float)maxFishCount;
    }

    private IEnumerator SellFish(GameObject other)
    {
        for (int i = 0; i < currentFishCount; i++)
        {           
            Transform res = resourceStack.stacks[resourceStack.currentIndex].transform;
            Vector3 tempPos = res.localPosition;
            Vector3 pos = fishStoreTarget.position;
            res.DOMove(pos, .45f).OnComplete(() =>
            {
                res.gameObject.SetActive(false);
                res.localPosition = tempPos;
                resourceStack.ClearStack();
            });

            yield return new WaitForSeconds(.5f);
        }
        //int coinToAdd = currentFishCount;
        UIManager.instance.PlayResourceClaimAnimationModified(1, 1, ui.GetComponent<RectTransform>(), currentFishCount, null);
        currentFishCount = 0;
    }
    #endregion
}
