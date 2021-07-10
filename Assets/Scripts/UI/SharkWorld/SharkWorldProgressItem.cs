//using GoogleMobileAds.Api;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SharkWorldProgressItem : MonoBehaviour
{
    SharkWorldItem myWorld;
    [Header("UI Elements")]
    public TextMeshProUGUI txtClearLevel;
    public TextMeshProUGUI txtProgress;
    public TextMeshProUGUI txtRewardAmount1;
    public TextMeshProUGUI txtRewardAmount2;
    public Image           imgItem1; 
    public Image           imgItem2;
    public Image           imgFillImage;
    public Button          btnClick;
    public GameObject clamiedImg;

    [Header("VALUES")]
    int    rewardAmount1, rewardAmount2;
    int    clearedRoom;
    int    currentProgress;
    int    targetProgress;
    float  fillValue;
    Sprite sprite1;
    Sprite sprite2;
    int worldId;
    int progressId;
    int currentLevel;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public SharkWorldProgressItem init( SharkWorldItem world,int WorldId,int ProgressId,int CurrentLevel,int reward1)
    {
        myWorld = world;
        worldId = WorldId;
        progressId = ProgressId;
        currentLevel = CurrentLevel;
        btnClick.onClick.AddListener(() => btnClickCallback());

        rewardAmount1 = reward1;
        //reward set
        //AssignRewardImage(sprite1, sprite2);
        AssignRewardValue(rewardAmount1, rewardAmount2);

        //progress set
        targetProgress = (ProgressId + 1) * 3;
        currentProgress = currentLevel - (worldId * 9);
        if(currentProgress> targetProgress) { currentProgress = targetProgress;  }
        else if(currentProgress < targetProgress) { clamiedImg.SetActive(false); }
       
        if (currentProgress < 0) { currentProgress = 0; }

        SetTxtProgress(currentProgress, targetProgress);

        //progress fill set
        fillValue = (currentProgress*1f / targetProgress);
       
        SetImgFillImageFill(fillValue);

        //room no set
        clearedRoom = progressId;
        SetTxtClearRoom(clearedRoom);

        return this;

    }
    
    public void SetImgFillImageFill(float Fillvalue)
    {

        imgFillImage.fillAmount = Fillvalue;
    }
    public void AssignRewardImage(Sprite sprt1, Sprite sprt2)
    {
        imgItem1.sprite = sprt1;
        imgItem2.sprite = sprt2;

    }
    public void AssignRewardValue(int RewardAmount1,int RewardAmount2)
    {
        txtRewardAmount1.text = RewardAmount1.ToString(); 
        txtRewardAmount2.text = RewardAmount2.ToString();

    }
    public void SetTxtProgress(int current,int target )
    {
        txtProgress.text = "PROGRESS " + current + "/" + target;

    }
    public void SetTxtClearRoom(int clearedroom)
    {
        txtClearLevel.text = "CLEAR ROOM " + clearedroom;
    }
    public void btnClickCallback()
    {
        Debug.Log("clicked");
    }
}
