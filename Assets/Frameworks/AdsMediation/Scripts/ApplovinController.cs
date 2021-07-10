using System;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;
using System.Linq;


namespace FPG
{
    public class ApplovinController : MonoBehaviour, IAds
    {
        private static ApplovinController Instance;
        public static ApplovinController GetInstance()
        {
            return Instance;
        }

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                //DontDestroyOnLoad(gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
        }
        
        public void Init(UnityAction<bool> initCallback)
        {
            //Debug.Log("AdsMediation ApplovinController->Init called");

            MaxSdkCallbacks.OnSdkInitializedEvent += (MaxSdkBase.SdkConfiguration sdkConfiguration) => {
                
                InitRewardedAd();
                InitInterstitialAd();
                InitBannerAd();

                initCallback?.Invoke(true);
            };

            MaxSdk.SetSdkKey(AdPlacement.applovin_app_key);
            MaxSdk.InitializeSdk();
        }

        #region Rewarded

        private UnityAction<bool> onRWAdLoadCallback;
        private UnityAction<bool> onRWCloseCallback;
        
        private string rewardedAdUnitId;
        private bool rewardPending;


        public void InitRewardedAd()
        {
            //Debug.Log("AdsMediation ApplovinController->InitRewardedAd");
            MaxSdkCallbacks.OnRewardedAdLoadedEvent += OnRewardedAdLoadedEvent;
            MaxSdkCallbacks.OnRewardedAdLoadFailedEvent += OnRewardedAdFailedEvent;
            MaxSdkCallbacks.OnRewardedAdFailedToDisplayEvent += OnRewardedAdFailedToDisplayEvent;
            MaxSdkCallbacks.OnRewardedAdDisplayedEvent += OnRewardedAdDisplayedEvent;
            MaxSdkCallbacks.OnRewardedAdClickedEvent += OnRewardedAdClickedEvent;
            MaxSdkCallbacks.OnRewardedAdHiddenEvent += OnRewardedAdDismissedEvent;
            MaxSdkCallbacks.OnRewardedAdReceivedRewardEvent += OnRewardedAdReceivedRewardEvent;
        }

        public void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback)
        {
            //Debug.Log("AdsMediation ApplovinController->LoadRewardedAd adUnitId" + adUnitId);
            onRWAdLoadCallback = adLoadCallback;
            rewardedAdUnitId = adUnitId;

            MaxSdk.LoadRewardedAd(rewardedAdUnitId);
        }

        public bool IsRewardedAdAvailable()
        {

            bool aval = MaxSdk.IsRewardedAdReady(rewardedAdUnitId);
            //Debug.Log("AdsMediation ApplovinController->IsRewardedAdAvailable aval.:" + aval);
            return aval;
        }
        public void ShowRewardedAd(UnityAction<bool> closeCallback)
        {
            //Debug.Log("AdsMediation ApplovinController->ShowRewardedAd");
            onRWCloseCallback = closeCallback;

            if (IsRewardedAdAvailable())
            {
                MaxSdk.ShowRewardedAd(rewardedAdUnitId);
            }
        }
        #region Rewarded Delegates

        private void OnRewardedAdLoadedEvent(string adUnitId)
        {
            //Debug.Log("AdsMediation ApplovinController->OnRewardedAdLoadedEvent");
            onRWAdLoadCallback?.Invoke(true);
        }

        private void OnRewardedAdFailedEvent(string adUnitId, int errorCode)
        {
            //Debug.Log("AdsMediation ApplovinController->ShowRewardedAd");
            onRWAdLoadCallback?.Invoke(false);
        }

        private void OnRewardedAdFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            //Debug.Log("AdsMediation ApplovinController->OnRewardedAdFailedToDisplayEvent. code: "+errorCode);
            onRWCloseCallback?.Invoke(false);
        }

        private void OnRewardedAdDisplayedEvent(string adUnitId)
        {
            //Debug.Log("AdsMediation ApplovinController->OnRewardedAdDisplayedEvent");
        }

        private void OnRewardedAdClickedEvent(string adUnitId)
        {
            //Debug.Log("AdsMediation ApplovinController->OnRewardedAdClickedEvent");
        }

        private void OnRewardedAdDismissedEvent(string adUnitId)
        {
            //Debug.Log("AdsMediation ApplovinController->OnRewardedAdDismissedEvent");
            onRWCloseCallback?.Invoke(rewardPending);
            rewardPending = false;
        }

        private void OnRewardedAdReceivedRewardEvent(string adUnitId, MaxSdk.Reward reward)
        {
            //Debug.Log("AdsMediation ApplovinController->OnRewardedAdReceivedRewardEvent");
            rewardPending = true;
        }
        #endregion Rewarded Delegates
        
        #endregion Rewarded

        #region Interstitial
        private UnityAction<bool> onINTAdLoadCallback;
        private UnityAction onINTCloseCallback;
        private string interstitialAdUnitId;
        
        public void InitInterstitialAd()
        {
            //Debug.Log("AdsMediation ApplovinController->InitInterstitialAd");

            MaxSdkCallbacks.OnInterstitialLoadedEvent += OnInterstitialLoadedEvent;
            MaxSdkCallbacks.OnInterstitialLoadFailedEvent += OnInterstitialFailedEvent;
            MaxSdkCallbacks.OnInterstitialAdFailedToDisplayEvent += InterstitialFailedToDisplayEvent;
            MaxSdkCallbacks.OnInterstitialHiddenEvent += OnInterstitialDismissedEvent;
        }
        public void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback)
        {
            //Debug.Log("AdsMediation ApplovinController->LoadInterstitial adUnitId" + adUnitId);
            onINTAdLoadCallback = adLoadCallback;
            interstitialAdUnitId = adUnitId;

            MaxSdk.LoadInterstitial(interstitialAdUnitId);

        }

        public bool IsInterstitialAdAvailable()
        {
            bool aval = MaxSdk.IsInterstitialReady(interstitialAdUnitId);

            //Debug.Log("AdsMediation ApplovinController->IsInterstitialAdAvailable aval." + aval);
            return aval;
        }
        public void ShowIterstitialAd(UnityAction closeCallback)
        {
            //Debug.Log("AdsMediation ApplovinController->ShowIterstitialAd");
            onINTCloseCallback = closeCallback;

            if (IsInterstitialAdAvailable())
            {
                MaxSdk.ShowInterstitial(interstitialAdUnitId);
            }
        }

        #region Interstitial Delegates

        private void OnInterstitialLoadedEvent(string adUnitId)
        {
            //Debug.Log("AdsMediation ApplovinController->OnInterstitialLoadedEvent");
            onINTAdLoadCallback?.Invoke(true);
        }

        private void OnInterstitialFailedEvent(string adUnitId, int errorCode)
        {
            //Debug.Log("AdsMediation ApplovinController->OnInterstitialFailedEvent. code: "+errorCode);
            onINTAdLoadCallback?.Invoke(false);
        }

        private void InterstitialFailedToDisplayEvent(string adUnitId, int errorCode)
        {
            //Debug.Log("AdsMediation ApplovinController->InterstitialFailedToDisplayEvent. code: " + errorCode);
            onINTCloseCallback?.Invoke();
        }

        private void OnInterstitialDismissedEvent(string adUnitId)
        {
            //Debug.Log("AdsMediation ApplovinController->OnInterstitialDismissedEvent");
            onINTCloseCallback?.Invoke();
        }

        #endregion Interstitial Delegates

        #endregion Interstitial

        #region Banner
        private bool isBannerInitialised = false;
        private string bannerAdUnitId = "YOUR_BANNER_AD_UNIT_ID";
        private void InitBannerAd()
        {
            //Debug.Log("AdsMediation ApplovinController->InitBannerAd");
            //Nothing to do in InitBannerAd. Just call LoadBanner() when you need it.
        }

        public void LoadBanner(string adUnit)
        {
            isBannerInitialised = true;
            //Debug.Log("AdsMediation ApplovinController->LoadBanner");
            bannerAdUnitId = adUnit;
            // Banners are automatically sized to 320x50 on phones and 728x90 on tablets
            // You may use the utility method `MaxSdkUtils.isTablet()` to help with view sizing adjustments
            MaxSdk.CreateBanner(bannerAdUnitId, MaxSdkBase.BannerPosition.BottomCenter);

            // Set background or background color for banners to be fully functional
            //MaxSdk.SetBannerBackgroundColor(AdPlacement.applovin_default_banner_key, Color.cyan);
        }
        public void ShowBanner()
        {
            if (!isBannerInitialised) return;
            //Debug.Log("AdsMediation ApplovinController->ShowBanner");
            MaxSdk.ShowBanner(bannerAdUnitId);
        }
        public void HideBanner()
        {
            if (!isBannerInitialised) return;
            //Debug.Log("AdsMediation ApplovinController->HideBanner");
            MaxSdk.HideBanner(bannerAdUnitId);
        }

        #endregion Banner
        
        IEnumerator RunPostUpdate(UnityAction _method)
        {
            // If RunOnMainThread() is called in a secondary thread,
            // this coroutine will start on the secondary thread
            // then yield until the end of the frame on the main thread
            yield return null;

            _method();
        }

        IEnumerator RunPostUpdate(UnityAction<bool> _method, bool param)
        {
            // If RunOnMainThread() is called in a secondary thread,
            // this coroutine will start on the secondary thread
            // then yield until the end of the frame on the main thread
            yield return null;

            _method(param);
        }
    }
}