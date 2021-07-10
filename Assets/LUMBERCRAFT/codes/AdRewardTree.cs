using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class AdRewardTree : MonoBehaviour
{
    public GameObject rewardTreePrefab;
    [SerializeField] GameObject _collider;


    [SerializeField] float treePosY = -85;
    GameObject treeIns;
    public GameObject adReadySeed;
    public bool isGenerated = false;

    private void Awake()
    {
        FPG.MobileAdsManager.GetInstance().onInterstitialAdStatusChanged += InterstitialAdStatusChanged;

        _collider.SetActive(false);
        
    }
    private void OnDestroy()
    {
        FPG.MobileAdsManager.GetInstance().onInterstitialAdStatusChanged -= InterstitialAdStatusChanged;
    }


    public void GenerateTree()
    {
        if (!isGenerated)
        {
            isGenerated = true;
            if (FPG.MobileAdsManager.GetInstance().IsInterstitialAdAvialable())
            {
                FPG.Networking.getInstance().video_ad_source = "AdRewardTree";
                FPG.Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 1, 0, 1, FPG.Networking.getInstance().video_ad_source, false);
                FPG.MobileAdsManager.GetInstance().ShowInterstitial(AdCompleteCallBack);
            }
        }
    }
    private void AdCompleteCallBack()
    {
        SoundManager.sharedInstance.PlaySFX(SoundManager.sharedInstance.goldenTreeSpawnSFX);
        GameManager.instance.player.GetComponent<PlayerController>().axeCollider.SetActive(false);
        _collider.SetActive(false);
        Vector3 treeInsPos = new Vector3(_collider.transform.localPosition.x, treePosY, _collider.transform.localPosition.z);
        treeIns = Instantiate(rewardTreePrefab, this.transform);
        treeIns.transform.localPosition = treeInsPos;
        treeIns.transform.DORotateQuaternion(Quaternion.Euler(new Vector3(0,180,0)),1.5f);
        treeIns.transform.DOLocalMoveY(_collider.transform.localPosition.y + 5, 1.85f).OnComplete(()=>
        {
            GameManager.instance.player.GetComponent<PlayerController>().axeCollider.SetActive(true);
        });
        treeIns.transform.localScale = Vector3.one * 1;
        treeIns.GetComponent<TreeController>().OnFarmComplete += OnFarmComplete;
    }

    private void OnFarmComplete()
    {
        //
        GameManager.instance.playerCon.anim.SetBool("chop360", false);
        GameManager.instance.playerCon.anim.speed = 1.0f;
        StartCoroutine(ActivateTreeCollider());
        Debug.Log("FarmComplete Called");
        treeIns.GetComponent<TreeController>().OnFarmComplete -= OnFarmComplete;
    }

    IEnumerator ActivateTreeCollider()
    {
        adReadySeed.SetActive(false);
        yield return new WaitForSeconds(TagManager.GetAdTreeRegenerateTime());
        _collider.SetActive(true);
        isGenerated = false;
        ShowAdReadySeed();
    }

    public void DestroyRewardTree()
    {
        Destroy(treeIns);
    }

    private bool adAvailable;
    private void InterstitialAdStatusChanged(bool available)
    {
        adAvailable = available;
        ShowAdReadySeed();
    }

    private void ShowAdReadySeed()
    {
        adReadySeed.SetActive(adAvailable && !isGenerated);
        Debug.Log(" "+ adAvailable +" " + isGenerated);
    }

}
