using System;
using UnityEngine;
using UnityEngine.Events;

namespace FPG
{
    public class MobileAdsManager
    {
        public static MobileAdsManager Instance;

        private MobileAdsManager() { }

        public static MobileAdsManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new MobileAdsManager();
            }
            return Instance;
        }

        public void Init()
        {
            //Debug.Log("MobileAdsManager -> InitializeAds.");
            //if (!IsAdBlocked()) Advertisements.Instance.Initialize();

            LoadInterstitialAd();
            //LoadBanner();
            //ShowBanner();
        }

        //private bool IsAdBlocked()
        //{
        //    return PlayerPrefs.GetInt(Constants.NoAdsFlag, 0) == 1;
        //}

        //#region interstitial ad

        //public bool IsInterstitialAdAvialable()
        //{
        //    if (IsAdBlocked()) return false;

        //    return Advertisements.Instance.IsInterstitialAvailable();

        //}

        //public void ShowInterstitial()
        //{
        //    ShowInterstitial(InterstitialAdClosed);
        //}

        //public void ShowInterstitial(UnityAction interstitialAdClosedCallback)
        //{
        //    if (IsInterstitialAdAvialable())
        //    {
        //        Advertisements.Instance.ShowInterstitial(interstitialAdClosedCallback);
        //    }
        //}

        //private void InterstitialAdClosed()
        //{
        //    Debug.Log("MobileAdsManager -> InterstitialAdClosed.");
        //}

        //#endregion interstitial ad

        #region Interstitial

        public Action<bool> onInterstitialAdStatusChanged;

        private void LoadInterstitialAd()
        {
            ApplovinController.GetInstance().LoadInterstitialAd(AdPlacement.applovin_default_interstitial_key, InterstitialAdLoaded);
            //IrnSrcController.GetInstance().LoadInterstitialAd(AdPlacement.ironsrc_default_interstitialKey, InterstitialAdLoaded);
            FPG.Networking.getInstance().sendUserAdStatus(1, 0, 0, 0, 0, 0, 0, FPG.Networking.getInstance().video_ad_source, false);
        }


        public bool IsInterstitialAdAvialable()
        {
            //if (IsAdBlocked()) return false;

            return ApplovinController.GetInstance().IsInterstitialAdAvailable();
            //return IrnSrcController.GetInstance().IsInterstitialAdAvailable();
        }

        public void ShowInterstitial()
        {
            if (IsInterstitialAdAvialable())
            {
                ApplovinController.GetInstance().ShowIterstitialAd(InterstitialAdClosed);             
                FPG.FacebookManager.GetInstance().InterstitialAdShownEvent();
                //IrnSrcController.GetInstance().ShowIterstitialAd(InterstitialAdClosed);
            }
        }
        UnityAction onInterstitialAdClosed;
        public void ShowInterstitial(UnityAction interstitialAdClosedCallback)
        {
            onInterstitialAdClosed = interstitialAdClosedCallback;
            if (IsInterstitialAdAvialable())
            {
                onInterstitialAdStatusChanged?.Invoke(false);
                ApplovinController.GetInstance().ShowIterstitialAd(InterstitialAdClosed);
                //IrnSrcController.GetInstance().ShowIterstitialAd(InterstitialAdClosed);
                FPG.FirebaseManager.GetInstance().LogAnalyticsEvent("gae_InterstitialShow");
                UpdateIntAdShowCounter();
                Time.timeScale = 0f;
            }
        }

        private void InterstitialAdClosed()
        {
            //Debug.Log("MobileAdsManager -> InterstitialAdClosed.");
            onInterstitialAdClosed?.Invoke();
            onInterstitialAdClosed = null;
            FPG.FirebaseManager.GetInstance().LogAnalyticsEvent("gae_InterstitialCompleted");
            FPG.TenjinManager.GetInstance().IncrementConversionValueForImpression();

            Time.timeScale = 1f;

            LoadInterstitialAd();
        }

        private void InterstitialAdLoaded(bool loaded)
        {
            //Debug.Log("MobileAdsManager -> InterstitialAdLoaded. loaded: " + loaded);
            onInterstitialAdStatusChanged.Invoke(true);
            FPG.FirebaseManager.GetInstance().LogAnalyticsEvent("gae_InterstitialLoad");
        }

        public static readonly string IntAdCounterKey = "IntAdCounter";
        public static void UpdateIntAdShowCounter(string adType = "int")
        {
            //Debug.Log("VideoAdsManager->UpdateAdShowCounter adType: "+ adType);
            int savedAdCounter = PlayerPrefs.GetInt(IntAdCounterKey, 100000);
            PlayerPrefs.SetInt(IntAdCounterKey, savedAdCounter + 1);
            Networking.getInstance().total_int_ad_show_counter += 1;
        }

        #endregion Interstitial


        #region Banner

        public void LoadBanner()
        {
            if (TagManager.IsBannerActivated()) ApplovinController.GetInstance().LoadBanner(AdPlacement.applovin_default_banner_key);
        }
        public void ShowBanner()
        {
            if (TagManager.IsBannerActivated()) ApplovinController.GetInstance().ShowBanner();
        }
        public void HideBanner()
        {
            if (TagManager.IsBannerActivated()) ApplovinController.GetInstance().HideBanner();
        }

        #endregion Banner
    }
}