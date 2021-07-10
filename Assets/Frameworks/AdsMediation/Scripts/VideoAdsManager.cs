using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

namespace FPG
{
    public class VideoAdsManager : MonoBehaviour
    {
        private static VideoAdsManager Instance;
        public static VideoAdsManager GetInstance()
        {
            return Instance;
        }

        private VideoAdsType runningAdType;
        private int adSearchIndex = 0;
        private int total_int_ad_show_counter = 0;
        private string strRunningAdsUnitId;

        public static readonly string AD_NAME_RW = "Rewarded";
        public static readonly string AD_NAME_INT = "Interstitial";

        private WaitForSeconds restartAdLoadDelay = new WaitForSeconds(10f);

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
        }

        #region IDFA Prompt

//        public void PreInit()
//        {
//#if UNITY_ANDROID
//            Init();
//#elif UNITY_IOS
//            AppleIDFA.RequestAccess(PreInitResponse);
//#endif
//        }
//        public void PreInitResponse(bool success)
//        {
//            Init();
//        }

        #endregion

        #region Initializers
        public void Init()
        {
            SetRunningAdType();

            if (!applovinInitFlag)
            {
                ApplovinController.GetInstance().Init(ApplovinInitialized);
            }

            //if (!irnSrcInitFlag)
            //{
            //    IrnSrcController.GetInstance().Init(IrnSrcInitialized);
            //}
            //if (!admobInitFlag)
            //{
            //    AdmobController.GetInstance().Init(AdmobInitialized);
            //}
            //if (!fanInitFlag)
            //{
            //    FANController.GetInstance().Init(FANInitialized);
            //}
        }

        private enum AdProvider { Admob, FAN, IrnSrc, Applovin }
        private bool admobInitFlag, fanInitFlag, irnSrcInitFlag, applovinInitFlag;

        private void AdProviderInitialized(AdProvider provider)
        {
            switch (provider)
            {
                case AdProvider.Admob:
                    admobInitFlag = true;
                    break;
                case AdProvider.FAN:
                    fanInitFlag = true;
                    break;
                case AdProvider.IrnSrc:
                    irnSrcInitFlag = true;
                    break;
                case AdProvider.Applovin:
                    applovinInitFlag = true;
                    break;
                default:
                    break;
            }

            if (applovinInitFlag)  // ---> /*admobInitFlag && fanInitFlag && irnSrcInitFlag && applovinInitFlag*/
            {
                LoadAds();
                FPG.MobileAdsManager.GetInstance().Init();
            }
        }

        #endregion Initializers

        #region Callbacks

        private void AdmobInitialized(bool success)
        {
            //Debug.Log("VideoAdsManager->AdmobInitialized success: " + success);
            AdProviderInitialized(AdProvider.Admob);
        }
        private void FANInitialized(bool success)
        {
            //Debug.Log("VideoAdsManager->FANInitialized success: " + success);
            AdProviderInitialized(AdProvider.FAN);
        }
        private void IrnSrcInitialized(bool success)
        {
            //Debug.Log("VideoAdsManager->IrnSrcInitialized success: " + success);
            AdProviderInitialized(AdProvider.IrnSrc);
        }
        private void ApplovinInitialized(bool success)
        {
            //Debug.Log("VideoAdsManager->ApplovinInitialized success: " + success);
            AdProviderInitialized(AdProvider.Applovin);
        }
        
        private void OnINTAdLoadComplete(bool success)
        {
            //Debug.Log("VideoAdsManager->OnINTAdLoadComplete success: " + success);
            if (success)
            {
                AdLoadCompleted();
            }
            else
            {
                AdLoadFailed();
            }
        }
        private void OnINTAdClose()
        {
            //Debug.Log("VideoAdsManager->OnINTAdClose");
            Networking.getInstance().sendUserAdStatus(0, 0, 0, 0, 1, 0, 1, Networking.getInstance().video_ad_source);
            FPG.TenjinManager.GetInstance().IncrementConversionValueForImpression();
            UpdateAdShowCounter(AD_NAME_INT);
            GiveRewardToCaller(true);
            LoadAds();
            Time.timeScale = 1f;
        }

        private void OnRWAdLoadComplete(bool success)
        {
            //Debug.Log("VideoAdsManager->OnRWAdLoadComplete success:" + success);
            if (success)
            {
                AdLoadCompleted();
            }
            else
            {
                AdLoadFailed();
            }

           
        }
        private void OnRWAdClose(bool success)
        {
            //Debug.Log("VideoAdsManager-> OnRWAdClose:" + success);

            IncrementRewardedAdImpressionCount();
            if(success)
            {
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 0, 1, 0, 1, Networking.getInstance().video_ad_source);
                FPG.TenjinManager.GetInstance().IncrementConversionValueForImpression();
            }
            else
            {
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 0, 1, 0, 0, Networking.getInstance().video_ad_source);
            }

            UpdateAdShowCounter(AD_NAME_RW);
            GiveRewardToCaller(success);
            LoadAds();
            Time.timeScale = 1f;
        }

        #endregion Callbacks

        //#region Banner
        //public void LoadBanner()
        //{
        //    if (TagManager.IsBannerActivated()) IrnSrcController.GetInstance().LoadBanner();
        //}
        //public void ShowBanner()
        //{
        //    if (TagManager.IsBannerActivated()) IrnSrcController.GetInstance().ShowBanner();
        //}
        //public void HideBanner()
        //{
        //    if (TagManager.IsBannerActivated()) IrnSrcController.GetInstance().HideBanner();
        //}
        //#endregion Banner

        #region Ad Router
        private void SetRunningAdType()
        {
            runningAdType = VideoAdsType.VA_Applovin_Rewarded;

            //if (adSearchIndex < TagManager.GetAdSequence().Length)
            //{
            //    int adPlacementKey = TagManager.GetAdSequence()[adSearchIndex];

            //    if ( adPlacementKey >= 101 && adPlacementKey <= 200)
            //    {
            //        runningAdType = VideoAdsType.VA_FaceBookAds;
            //    }
            //    else if (adPlacementKey >= 201 && adPlacementKey <= 300)
            //    {
            //        runningAdType = VideoAdsType.VA_AdMob;
            //    }
            //    else if (adPlacementKey >= 301 && adPlacementKey <= 400)
            //    {
            //        runningAdType = VideoAdsType.VA_FaceBookAds_Interstitial;
            //    }
            //    else if (adPlacementKey >= 401 && adPlacementKey <= 500)
            //    {
            //        runningAdType = VideoAdsType.VA_Admob_Interstitial;
            //    }
            //    else if (adPlacementKey >= 501 && adPlacementKey <= 600)
            //    {
            //        runningAdType = VideoAdsType.VA_IrnSrc;
            //    }
            //    else if (adPlacementKey >= 601 && adPlacementKey <= 700)
            //    {
            //        runningAdType = VideoAdsType.VA_IrnSrc_Interstitial;
            //    }
            //    else if (adPlacementKey >= 701 && adPlacementKey <= 800)
            //    {
            //        runningAdType = VideoAdsType.VA_Applovin_Rewarded;
            //    }
            //    else if (adPlacementKey >= 801 && adPlacementKey <= 900)
            //    {
            //        runningAdType = VideoAdsType.VA_Applovin_Interstitial;
            //    }
            //    else
            //    {
            //        runningAdType = VideoAdsType.VA_AdMob;
            //    }
            //}
        }

        public string GetAdsUnitId()
        {
            return AdPlacement.applovin_default_rewarded_key;

            //if (adSearchIndex < TagManager.GetAdSequence().Length)
            //{
            //    AdPlacementKey placementKey = (AdPlacementKey)TagManager.GetAdSequence()[adSearchIndex];
            //    switch (placementKey)
            //    {
            //        case AdPlacementKey.adKey_facebook_high:
            //            {
            //                return AdPlacement.fb_placement_id_high;
            //            }
            //        case AdPlacementKey.adKey_facebook_medium:
            //            {
            //                return AdPlacement.fb_placement_id_medium;
            //            }
            //        case AdPlacementKey.adKey_facebook_low:
            //            {
            //                return AdPlacement.fb_placement_id_low;
            //            }
            //        case AdPlacementKey.adKey_facebook_upperTH:
            //            {
            //                return AdPlacement.fb_placement_id_upperTH;
            //            }
            //        case AdPlacementKey.adKey_facebook_lowerTH:
            //            {
            //                return AdPlacement.fb_placement_id_lowerTH;
            //            }
            //        case AdPlacementKey.adKey_facebook_default:
            //            {
            //                return AdPlacement.fb_placement_id_default;
            //            }
            //        case AdPlacementKey.adKey_admob_25:
            //            {
            //                return AdPlacement.admob_adunit_id_25;
            //            }
            //        case AdPlacementKey.adKey_admob_20:
            //            {
            //                return AdPlacement.admob_adunit_id_20;
            //            }
            //        case AdPlacementKey.adKey_admob_15:
            //            {
            //                return AdPlacement.admob_adunit_id_15;
            //            }
            //        case AdPlacementKey.adKey_admob_10:
            //            {
            //                return AdPlacement.admob_adunit_id_10;
            //            }
            //        case AdPlacementKey.adKey_admob_07:
            //            {
            //                return AdPlacement.admob_adunit_id_07;
            //            }
            //        case AdPlacementKey.adKey_admob_05:
            //            {
            //                return AdPlacement.admob_adunit_id_05;
            //            }
            //        case AdPlacementKey.adKey_admob_03:
            //            {
            //                return AdPlacement.admob_adunit_id_03;
            //            }
            //        case AdPlacementKey.adKey_facebook_interstitial_01:
            //            {
            //                return AdPlacement.facebook_interstitial_id_01;
            //            }
            //        case AdPlacementKey.adKey_facebook_interstitial_02:
            //            {
            //                return AdPlacement.facebook_interstitial_id_02;
            //            }

            //        case AdPlacementKey.adKey_admob_interstitial_01:
            //            {
            //                return AdPlacement.admob_interstitial_id_01;
            //            }
            //        case AdPlacementKey.adKey_admob_interstitial_02:
            //            {
            //                return AdPlacement.admob_interstitial_id_02;
            //            }

            //        case AdPlacementKey.adKey_admob_interstitial_03:
            //            {
            //                return AdPlacement.admob_interstitial_id_03;
            //            }
            //        case AdPlacementKey.adKey_admob_interstitial_04:
            //            {
            //                return AdPlacement.admob_interstitial_id_04;
            //            }
            //        case AdPlacementKey.adKey_admob_default:
            //            {
            //                return AdPlacement.admob_adunit_id_default;
            //            }

            //        case AdPlacementKey.adKey_irnsrc_rw_01:
            //            {
            //                return AdPlacement.ironsrc_default_rewardedKey;
            //            }
            //        case AdPlacementKey.adKey_irnsrc_int_01:
            //            {
            //                return AdPlacement.ironsrc_default_interstitialKey;
            //            }

            //        case AdPlacementKey.adKey_applovin_rw_01:
            //            {
            //                return AdPlacement.applovin_default_rewarded_key;
            //            }
            //        case AdPlacementKey.adKey_applovin_int_01:
            //            {
            //                return AdPlacement.applovin_default_interstitial_key;
            //            }
            //    }
            //}
            //return string.Empty;
        }
        public void LoadAds()
        {
            //Debug.Log("VideoAdsManager->LoadAds");
            //if (adSearchIndex < TagManager.GetAdSequence().Length)
            //{
                SetRunningAdType();

                //Networking.getInstance().selectedAd = TagManager.GetAdSequence()[adSearchIndex];
                strRunningAdsUnitId = GetAdsUnitId();

                if (!string.IsNullOrEmpty(strRunningAdsUnitId))
                {
                    Networking.getInstance().sendUserAdStatus(1, 0, 0, 0, 0, 0, 0, Networking.getInstance().video_ad_source);

                    if (runningAdType == VideoAdsType.VA_AdMob)
                    {
                        //AdmobController.GetInstance().LoadRewardedAd(strRunningAdsUnitId, OnRWAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_FaceBookAds)
                    {
                        //FANController.GetInstance().LoadRewardedAd(strRunningAdsUnitId, OnRWAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_FaceBookAds_Interstitial)
                    {
                        //FANController.GetInstance().LoadInterstitialAd(strRunningAdsUnitId, OnINTAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_Admob_Interstitial)
                    {
                        //AdmobController.GetInstance().LoadInterstitialAd(strRunningAdsUnitId, OnINTAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_IrnSrc)
                    {
                        IrnSrcController.GetInstance().LoadRewardedAd(strRunningAdsUnitId, OnRWAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_IrnSrc_Interstitial)
                    {
                        IrnSrcController.GetInstance().LoadInterstitialAd(strRunningAdsUnitId, OnINTAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_Applovin_Rewarded)
                    {
                        ApplovinController.GetInstance().LoadRewardedAd(strRunningAdsUnitId, OnRWAdLoadComplete);
                    }
                    else if (runningAdType == VideoAdsType.VA_Applovin_Interstitial)
                    {
                        ApplovinController.GetInstance().LoadInterstitialAd(strRunningAdsUnitId, OnINTAdLoadComplete);
                    }
                }
                else
                {
                    //Debug.Log("VideoAdsManager->LoadAds placement null");
                    AdLoadFailed();
                }
            //}
            //else
            //{
            //    StartCoroutine(RestartAdLoad());
            //}
        }
        public IEnumerator RestartAdLoad()
        {
            //Debug.Log("VideoAdsManager->RestartAdLoad delay: " + delay);
            yield return restartAdLoadDelay;
            if (adSearchIndex >= TagManager.GetAdSequence().Length)
            {
                adSearchIndex = Math.Max(TagManager.GetAdSequence().Length - 2, 0);
                LoadAds();
                Networking.getInstance().sendUserAdStatus(0, 0, 1, 0, 0, 0, 0, "0");
            }
        }
        #endregion Ad Router

        #region Api
        private UnityAction<bool> rewardCallback;
        public void ShowVideoAds(UnityAction<bool> callback)
        {
            Time.timeScale = 0f;

            if (!IsVideoAdsAvailable())
            {
                return;
            }

            if (runningAdType == VideoAdsType.VA_AdMob)
            {
                //AdmobController.GetInstance().ShowRewardedAd(OnRWAdClose);
				Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
			}
            else if (runningAdType == VideoAdsType.VA_FaceBookAds)
            {
                //FANController.GetInstance().ShowRewardedAd(OnRWAdClose);
				Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
			}
            else if (runningAdType == VideoAdsType.VA_FaceBookAds_Interstitial)
            {
                //FANController.GetInstance().ShowIterstitialAd(OnINTAdClose);
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
            }
            else if (runningAdType == VideoAdsType.VA_Admob_Interstitial)
            {
                //AdmobController.GetInstance().ShowIterstitialAd(OnINTAdClose);
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
            }
            else if (runningAdType == VideoAdsType.VA_IrnSrc)
            {
                IrnSrcController.GetInstance().ShowRewardedAd(OnRWAdClose);
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
            }
            else if (runningAdType == VideoAdsType.VA_IrnSrc_Interstitial)
            {
                IrnSrcController.GetInstance().ShowIterstitialAd(OnINTAdClose);
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
            }
            else if (runningAdType == VideoAdsType.VA_Applovin_Rewarded)
            {
                ApplovinController.GetInstance().ShowRewardedAd(OnRWAdClose);
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
            }
            else if (runningAdType == VideoAdsType.VA_Applovin_Interstitial)
            {
                ApplovinController.GetInstance().ShowIterstitialAd(OnINTAdClose);
                Networking.getInstance().sendUserAdStatus(0, 0, 0, 1, 0, 0, 0, Networking.getInstance().video_ad_source);
            }
            else
            {
                Time.timeScale = 1f;
            }

            rewardCallback = callback;
        }
        public bool IsVideoAdsAvailable()
        {
            if (runningAdType == VideoAdsType.VA_AdMob)
            {
                //if (AdmobController.GetInstance().IsRewardedAdAvailable())
                    return true;
            }
            else if (runningAdType == VideoAdsType.VA_FaceBookAds)
            {
                //if (FANController.GetInstance().IsRewardedAdAvailable())
                    return true;
            }
            else if (runningAdType == VideoAdsType.VA_FaceBookAds_Interstitial)
            {
                //if (FANController.GetInstance().IsInterstitialAdAvailable())
                    return true;
            }
            else if (runningAdType == VideoAdsType.VA_Admob_Interstitial)
            {
                //if (AdmobController.GetInstance().IsInterstitialAdAvailable())
                    return true;
            }
            else if (runningAdType == VideoAdsType.VA_IrnSrc)
            {
                if (IrnSrcController.GetInstance().IsRewardedAdAvailable())
                    return true;
            }
            else if (runningAdType == VideoAdsType.VA_IrnSrc_Interstitial)
            {
                if (IrnSrcController.GetInstance().IsInterstitialAdAvailable())
                    return true;
            }

            else if (runningAdType == VideoAdsType.VA_Applovin_Rewarded)
            {
                if (applovinInitFlag && ApplovinController.GetInstance().IsRewardedAdAvailable())
                    return true;
            }
            else if (runningAdType == VideoAdsType.VA_Applovin_Interstitial)
            {
                if (applovinInitFlag && ApplovinController.GetInstance().IsInterstitialAdAvailable())
                    return true;
            }

            return false;
        }

        private void GiveRewardToCaller(bool eligibleForReward)
        {
            //Debug.Log("VideoAdsManager->GiveRewardToCaller ");
            if (rewardCallback != null)
            {
                rewardCallback(eligibleForReward);
                rewardCallback = null;
            }
        }
        #endregion Api

        #region Ad Response

        public void AdLoadCompleted()
        {
            adSearchIndex = Math.Max(adSearchIndex - 2, 0);
            Networking.getInstance().sendUserAdStatus(0, 1, 0, 0, 0, 0, 0, "0");
        }

        public void AdLoadFailed()
        {
            adSearchIndex++;

            if (adSearchIndex < TagManager.GetAdSequence().Length)
            {
                Networking.getInstance().sendUserAdStatus(0, 0, 1, 0, 0, 0, 0, "0");
                LoadAds();
            }
            else
            {
                Networking.getInstance().selectedAd = 0;

                StartCoroutine(RestartAdLoad());
            }
        }

        public static readonly string AdCounterKey = "AdCounter";
        public void UpdateAdShowCounter(string adType)
        {
            //Debug.Log("VideoAdsManager->UpdateAdShowCounter adType: "+ adType);

            int savedAdCounter = PlayerPrefs.GetInt(AdCounterKey, 0);
            PlayerPrefs.SetInt(AdCounterKey, savedAdCounter + 1);
            Networking.getInstance().total_ad_show_counter += 1;

            FPG.FacebookManager.GetInstance().RewardedAdCompleteEvent();
        }

        public static readonly string RWAdsImpressionKey = "videoAdsImpression";
        public void IncrementRewardedAdImpressionCount()
        {
            int totalVideoAdsImpression = PlayerPrefs.GetInt(RWAdsImpressionKey, 0);
            totalVideoAdsImpression++;
            PlayerPrefs.SetInt(RWAdsImpressionKey, totalVideoAdsImpression);
        }
        #endregion Ad Response
    }

    public enum VideoAdsType
    {
        VA_None = 0,
        VA_AdMob = 1,
        VA_FaceBookAds = 3,
        VA_FaceBookAds_Interstitial = 4,
        VA_Admob_Interstitial = 5,
        VA_IrnSrc = 6,
        VA_IrnSrc_Interstitial = 7,
        VA_Applovin_Rewarded = 8,
        VA_Applovin_Interstitial = 9

    }

    public enum AdPlacementKey
    {
        adKey_facebook_high = 101,
        adKey_facebook_medium = 102,
        adKey_facebook_low = 103,
        adKey_facebook_default = 104,

        adKey_facebook_upperTH = 105,
        adKey_facebook_lowerTH = 106,

        adKey_admob_25 = 201,
        adKey_admob_20 = 202,
        adKey_admob_15 = 203,
        adKey_admob_10 = 204,
        adKey_admob_07 = 205,
        adKey_admob_05 = 206,
        adKey_admob_03 = 207,

        adKey_admob_default = 208,

        adKey_facebook_interstitial_01 = 301,
        adKey_facebook_interstitial_02 = 302,

        adKey_admob_interstitial_01 = 401,
        adKey_admob_interstitial_02 = 402,
        adKey_admob_interstitial_03 = 403,
        adKey_admob_interstitial_04 = 404,

        adKey_irnsrc_rw_01 = 501,

        adKey_irnsrc_int_01 = 601,

        adKey_applovin_rw_01 = 701,

        adKey_applovin_int_01 = 801
    };


}
