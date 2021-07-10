
using UnityEngine;
using UnityEngine.Events;
using System.Collections;

namespace FPG
{
    public class IrnSrcController : MonoBehaviour, IAds
    {
        private static IrnSrcController Instance;
        public static IrnSrcController GetInstance()
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

#if UNITY_EDITOR || UNITY_ANDROID || UNITY_IOS || UNITY_STANDALONE_LINUX


        public void Init(UnityAction<bool> initCallback)
        {
            initCallback(true);
        }

        public void InitInterstitialAd()
        {

        }
        public void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback)
        {
            adLoadCallback(false);
        }
        public bool IsInterstitialAdAvailable()
        {
            return false;
        }
        public void ShowIterstitialAd(UnityAction closeCallback)
        {
            closeCallback();
        }


        public void InitRewardedAd()
        {

        }
        public void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback)
        {
            adLoadCallback(false);
        }
        public bool IsRewardedAdAvailable()
        {
            return false;
        }
        public void ShowRewardedAd(UnityAction<bool> closeCallback)
        {
            closeCallback(false);
        }
        public void LoadBanner()
        {

        }
        public void ShowBanner()
        {

        }
        public void HideBanner()
        {

        }
#else

        public static string appKey = AdPlacement.ironsrc_app_key;
        public void Init(UnityAction<bool> initCallback)
        {
            IronSourceConfig.Instance.setClientSideCallbacks(true);

            string id = IronSource.Agent.getAdvertiserId();
            //Debug.Log("unity-script: IronSource.Agent.getAdvertiserId : " + id);
            IronSource.Agent.setUserId(id);

            //Debug.Log("unity-script: IronSource.Agent.validateIntegration");
            IronSource.Agent.validateIntegration();

            //Debug.Log("unity-script: unity version" + IronSource.unityVersion());
            //Debug.Log("unity-script: IronSource.Agent.init");
            IronSource.Agent.init(appKey);

            InitInterstitialAd();
            InitRewardedAd();
            //if (TagManager.IsBannerActivated()) InitBannerAd();
            initCallback?.Invoke(true);
        }
        void OnApplicationPause(bool isPaused)
        {
            //Debug.Log("unity-script: OnApplicationPause = " + isPaused);
            IronSource.Agent.onApplicationPause(isPaused);
        }

        #region Iterstitial

        private string interstitialInstanceId = string.Empty;
        private UnityAction<bool> onINTAdLoadCallback;
        private UnityAction onINTCloseCallback;

        public void InitInterstitialAd()
        {
            // Add Interstitial DemandOnly Events
            IronSourceEvents.onInterstitialAdReadyEvent += InterstitialAdReadyEvent;
            IronSourceEvents.onInterstitialAdLoadFailedEvent += InterstitialAdLoadFailedEvent;
            IronSourceEvents.onInterstitialAdShowSucceededEvent += InterstitialAdShowSucceededEvent;
            IronSourceEvents.onInterstitialAdShowFailedEvent += InterstitialAdShowFailedEvent;
            IronSourceEvents.onInterstitialAdClickedEvent += InterstitialAdClickedEvent;
            IronSourceEvents.onInterstitialAdOpenedEvent += InterstitialAdOpenedEvent;
            IronSourceEvents.onInterstitialAdClosedEvent += InterstitialAdClosedEvent;
        }
        public void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback)
        {
            //if (string.IsNullOrEmpty(interstitialInstanceId)) return;
            interstitialInstanceId = adUnitId;
            onINTAdLoadCallback = adLoadCallback;

            //IronSource.Agent.loadISDemandOnlyInterstitial(interstitialInstanceId);
            IronSource.Agent.loadInterstitial();
        }
        public bool IsInterstitialAdAvailable()
        {
            //if (string.IsNullOrEmpty(interstitialInstanceId)) return false;
            //return IronSource.Agent.isISDemandOnlyInterstitialReady(interstitialInstanceId);
            return IronSource.Agent.isInterstitialReady();
        }
        public void ShowIterstitialAd(UnityAction closeCallback)
        {
            onINTCloseCallback = closeCallback;

            if (IsInterstitialAdAvailable())
            {
                if(string.IsNullOrEmpty(interstitialInstanceId)) IronSource.Agent.showInterstitial();
                else IronSource.Agent.showInterstitial(interstitialInstanceId);

                interstitialInstanceId = string.Empty;
            }
            else
            {
                //Debug.Log("unity-script: IronSource.Agent.isInterstitialReady - False");
            }
        }
        #endregion Iterstitial

        #region Iterstitial Ad Delegates

        //Invoked when the initialization process has failed.
        //@param description - string - contains information about the failure.
        void InterstitialAdLoadFailedEvent(IronSourceError error)
        {
            //Debug.Log("unity-script: I got InterstitialAdLoadFailedEvent. error: "+error);
            onINTAdLoadCallback?.Invoke(false);
        }
        //Invoked right before the Interstitial screen is about to open.
        void InterstitialAdShowSucceededEvent()
        {
            //Debug.Log("unity-script: I got InterstitialAdShowSucceededEvent");
        }
        //Invoked when the ad fails to show.
        //@param description - string - contains information about the failure.
        void InterstitialAdShowFailedEvent(IronSourceError error)
        {
            //Debug.Log("unity-script: I got InterstitialAdShowFailedEvent error: "+error);
        }
        // Invoked when end user clicked on the interstitial ad
        void InterstitialAdClickedEvent()
        {
            //Debug.Log("unity-script: I got InterstitialAdClickedEvent");
        }
        //Invoked when the interstitial ad closed and the user goes back to the application screen.
        void InterstitialAdClosedEvent()
        {
            //Debug.Log("unity-script: I got InterstitialAdClosedEvent");
            onINTCloseCallback.Invoke();
        }
        //Invoked when the Interstitial is Ready to shown after load function is called
        void InterstitialAdReadyEvent()
        {
            //Debug.Log("unity-script: I got InterstitialAdReadyEvent");
            onINTAdLoadCallback?.Invoke(true);
        }
        //Invoked when the Interstitial Ad Unit has opened
        void InterstitialAdOpenedEvent()
        {
            //Debug.Log("unity-script: I got InterstitialAdOpenedEvent");
        }

        #endregion Iterstitial Ad Delegates

        #region Rewarded
        private bool rewardPending;
        private string rewardedInstanceId = string.Empty;
        private UnityAction<bool> onRWAdLoadCallback;
        private UnityAction<bool> onRWCloseCallback;

        public void InitRewardedAd()
        {
            //Add Rewarded Video DemandOnly Events
            //IronSourceEvents.onRewardedVideoAdOpenedDemandOnlyEvent += RewardedVideoAdOpenedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdClosedDemandOnlyEvent += RewardedVideoAdClosedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdLoadedDemandOnlyEvent += this.RewardedVideoAdLoadedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdRewardedDemandOnlyEvent += RewardedVideoAdRewardedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdShowFailedDemandOnlyEvent += RewardedVideoAdShowFailedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdClickedDemandOnlyEvent += RewardedVideoAdClickedDemandOnlyEvent;
            //IronSourceEvents.onRewardedVideoAdLoadFailedDemandOnlyEvent += this.RewardedVideoAdLoadFailedDemandOnlyEvent;

            //Add Rewarded Video Events
            IronSourceEvents.onRewardedVideoAdOpenedEvent += RewardedVideoAdOpenedEvent;
            IronSourceEvents.onRewardedVideoAdClosedEvent += RewardedVideoAdClosedEvent;
            IronSourceEvents.onRewardedVideoAvailabilityChangedEvent += RewardedVideoAvailabilityChangedEvent;
            IronSourceEvents.onRewardedVideoAdStartedEvent += RewardedVideoAdStartedEvent;
            IronSourceEvents.onRewardedVideoAdEndedEvent += RewardedVideoAdEndedEvent;
            IronSourceEvents.onRewardedVideoAdRewardedEvent += RewardedVideoAdRewardedEvent;
            IronSourceEvents.onRewardedVideoAdShowFailedEvent += RewardedVideoAdShowFailedEvent;
            IronSourceEvents.onRewardedVideoAdClickedEvent += RewardedVideoAdClickedEvent;
        }
        public void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback)
        {
            //if (string.IsNullOrEmpty(adUnitId)) return;
            rewardedInstanceId = adUnitId;
            onRWAdLoadCallback = adLoadCallback;
            if (IsRewardedAdAvailable())
            {
                onRWAdLoadCallback?.Invoke(true);
                onRWAdLoadCallback = null;
            }
            //IronSource.Agent.loadISDemandOnlyRewardedVideo(rewardedInstanceId);
        }
        public bool IsRewardedAdAvailable()
        {
            //if (string.IsNullOrEmpty(rewardedInstanceId)) return false;
            return IronSource.Agent.isRewardedVideoAvailable();
        }
        public void ShowRewardedAd(UnityAction<bool> closeCallback)
        {
            //if (string.IsNullOrEmpty(rewardedInstanceId)) return;
            onRWCloseCallback = closeCallback;
            if (IsRewardedAdAvailable())
            {
                IronSource.Agent.showRewardedVideo();
            }
            else
            {
                //Debug.Log("unity-script: IronSource.Agent.isRewardedVideoAvailable - False");
            }
        }
        #endregion Rewarded

        #region Rewarded Ad Delegates

        /************* RewardedVideo Delegates *************/
        void RewardedVideoAvailabilityChangedEvent(bool canShowAd)
        {
            //Debug.Log("unity-script: I got RewardedVideoAvailabilityChangedEvent, value = " + canShowAd);
            onRWAdLoadCallback?.Invoke(canShowAd);
            onRWAdLoadCallback = null;
        }

        void RewardedVideoAdOpenedEvent()
        {
            //Debug.Log("unity-script: I got RewardedVideoAdOpenedEvent");
        }

        bool rewardable = false;
        void RewardedVideoAdRewardedEvent(IronSourcePlacement ssp)
        {
            //Debug.Log("unity-script: I got RewardedVideoAdRewardedEvent, amount = " + ssp.getRewardAmount() + " name = " + ssp.getRewardName());
            rewardable = true;
        }

        void RewardedVideoAdClosedEvent()
        {
            //Debug.Log("unity-script: I got RewardedVideoAdClosedEvent");
            onRWCloseCallback?.Invoke(rewardable);
            onRWCloseCallback = null;
            rewardable = false;
        }

        void RewardedVideoAdStartedEvent()
        {
            //Debug.Log("unity-script: I got RewardedVideoAdStartedEvent");
        }

        void RewardedVideoAdEndedEvent()
        {
            //Debug.Log("unity-script: I got RewardedVideoAdEndedEvent");
        }

        void RewardedVideoAdShowFailedEvent(IronSourceError error)
        {
            //Debug.Log("unity-script: I got RewardedVideoAdShowFailedEvent, code :  " + error.getCode() + ", description : " + error.getDescription());
            onRWCloseCallback?.Invoke(false);
            onRWCloseCallback = null;
        }

        void RewardedVideoAdClickedEvent(IronSourcePlacement ssp)
        {
            //Debug.Log("unity-script: I got RewardedVideoAdClickedEvent, name = " + ssp.getRewardName());
        }
        #endregion Rewarded Ad Delegates

        #region Demand Only Rewarded Ad Delegates
        /************* RewardedVideo DemandOnly Delegates *************/

        void RewardedVideoAdLoadedDemandOnlyEvent(string instanceId)
        {
            onRWAdLoadCallback?.Invoke(true);
            //Debug.Log("unity-script: I got RewardedVideoAdLoadedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdLoadFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            onRWAdLoadCallback?.Invoke(false);
            //Debug.Log("unity-script: I got RewardedVideoAdLoadFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void RewardedVideoAdOpenedDemandOnlyEvent(string instanceId)
        {
            //Debug.Log("unity-script: I got RewardedVideoAdOpenedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdRewardedDemandOnlyEvent(string instanceId)
        {
            rewardPending = true;
            //Debug.Log("unity-script: I got RewardedVideoAdRewardedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdClosedDemandOnlyEvent(string instanceId)
        {
            onRWCloseCallback?.Invoke(rewardPending);
            rewardPending = false;
            //Debug.Log("unity-script: I got RewardedVideoAdClosedDemandOnlyEvent for instance: " + instanceId);
        }

        void RewardedVideoAdShowFailedDemandOnlyEvent(string instanceId, IronSourceError error)
        {
            onRWCloseCallback?.Invoke(false);
            //Debug.Log("unity-script: I got RewardedVideoAdShowFailedDemandOnlyEvent for instance: " + instanceId + ", code :  " + error.getCode() + ", description : " + error.getDescription());
        }

        void RewardedVideoAdClickedDemandOnlyEvent(string instanceId)
        {
            //Debug.Log("unity-script: I got RewardedVideoAdClickedDemandOnlyEvent for instance: " + instanceId);
        }
        /************* RewardedVideo DemandOnly Delegates End *************/
        #endregion Demand Only Rewarded Ad Delegates

        #region Banner Ad
        private bool isBannerInitialised = false;
        private bool isBannerLoaded = false;
        private bool isBannerVisible = false;

        private void InitBannerAd()
        {
            IronSourceEvents.onBannerAdLoadedEvent += BannerAdLoadedEvent;
            IronSourceEvents.onBannerAdLoadFailedEvent += BannerAdLoadFailedEvent;
            IronSourceEvents.onBannerAdClickedEvent += BannerAdClickedEvent;
            IronSourceEvents.onBannerAdScreenPresentedEvent += BannerAdScreenPresentedEvent;
            IronSourceEvents.onBannerAdScreenDismissedEvent += BannerAdScreenDismissedEvent;
            IronSourceEvents.onBannerAdLeftApplicationEvent += BannerAdLeftApplicationEvent;

            isBannerInitialised = true;
        }

        public void LoadBanner()
        {
            //Debug.Log("unity-script: LoadBanner");
            if (!isBannerInitialised) return;

            //IronSource.Agent.loadBanner(new IronSourceBannerSize(320, 50), IronSourceBannerPosition.BOTTOM);
            //IronSource.Agent.loadBanner(IronSourceBannerSize.BANNER, IronSourceBannerPosition.BOTTOM, (string)"YOUR_PLACEMENT_NAME");
            IronSource.Agent.loadBanner(IronSourceBannerSize.SMART, IronSourceBannerPosition.BOTTOM);
        }
        public void HideBanner()
        {
            //Debug.Log("unity-script: HideBanner");
            if (!isBannerInitialised) return;

            if (isBannerVisible)
            {
                isBannerVisible = false;
                IronSource.Agent.hideBanner();
            }
        }
        public void ShowBanner()
        {
            //Debug.Log("unity-script: ShowBanner");
            if (!isBannerInitialised) return;
            if (!isBannerVisible)
            {
                isBannerVisible = true;
                IronSource.Agent.displayBanner();
            }
        }
        public void DestroyBanner()
        {
            //Debug.Log("unity-script: DestroyBanner");
            if (!isBannerInitialised) return;
            IronSource.Agent.destroyBanner();
        }

        #region Banner Delegates
        //Invoked once the banner has loaded
        void BannerAdLoadedEvent()
        {
            Debug.Log("unity-script: BannerAdLoadedEvent");
            isBannerLoaded = true;
            isBannerVisible = true;
        }
        //Invoked when the banner loading process has failed.
        //@param description - string - contains information about the failure.
        void BannerAdLoadFailedEvent(IronSourceError error)
        {
            Debug.Log("unity-script: BannerAdLoadFailedEvent. error: "+error.getDescription());
            isBannerLoaded = false;
        }
        // Invoked when end user clicks on the banner ad
        void BannerAdClickedEvent()
        {
            Debug.Log("unity-script: BannerAdClickedEvent");
        }
        //Notifies the presentation of a full screen content following user click
        void BannerAdScreenPresentedEvent()
        {
            Debug.Log("unity-script: BannerAdScreenPresentedEvent");
        }
        //Notifies the presented screen has been dismissed
        void BannerAdScreenDismissedEvent()
        {
            Debug.Log("unity-script: BannerAdScreenDismissedEvent");
            isBannerVisible = false;
        }
        //Invoked when the user leaves the app
        void BannerAdLeftApplicationEvent()
        {
            Debug.Log("unity-script: BannerAdLeftApplicationEvent");
        }

        #endregion Banner Delegates


        #endregion Banner Ad

        public void TestInterstitial()
        {
            //Debug.Log("IronSource test TestShowInterstitial called");
            if (IsInterstitialAdAvailable())
            {
                //Debug.Log("IronSource test TestShowInterstitial available");
                ShowIterstitialAd(
                    () =>
                    {
                        //Debug.Log("IronSource test TestShowInterstitial completed");
                    }
                );
            }
            else
            {
                //Debug.Log("IronSource test TestLoadInterstitial not available. calling load");
                LoadInterstitialAd(AdPlacement.ironsrc_default_interstitialKey,
                    (success) => {
                        //Debug.Log("IronSource test TestInterstitialLoaded success: " + success);
                    }
                );
            }
        }
        
        public void TestRewarded()
        {
            //Debug.Log("IronSource test TestShowRewarded called");
            if (IsRewardedAdAvailable())
            {
                //Debug.Log("IronSource test TestShowRewarded available");
                ShowRewardedAd(
                    (success) =>
                    {
                        //Debug.Log("IronSource test TestShowRewarded completed. success: "+success);
                    }
                );
            }
            else
            {
                //Debug.Log("IronSource test TestLoadRewarded not available. calling laod");
                LoadRewardedAd(AdPlacement.ironsrc_default_rewardedKey,
                    (success) => {
                        //Debug.Log("IronSource test TestRewardedLoaded success: " + success);
                        if (success)
                        {

                        }
                    }
                );
            }
        }


        public void TestBanner()
        {
            //Debug.Log("unity-script: TestBanner");
            if (!isBannerLoaded) LoadBanner();
            else
            {
                if (!isBannerVisible) ShowBanner();
                else
                {
                    HideBanner();
                }
            }
        }

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
#endif
    }
}
