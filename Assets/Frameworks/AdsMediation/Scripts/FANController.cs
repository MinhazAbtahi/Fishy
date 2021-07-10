//using System;
//using System.Collections;
//using UnityEngine;
//using UnityEngine.Events;
//using System.Linq;

//using AudienceNetwork;

//namespace FPG
//{
//    public class FANController : MonoBehaviour, IAds
//    {
//        private static FANController Instance;
//        public static FANController GetInstance()
//        {
//            return Instance;
//        }

//        private void Awake()
//        {
//            if (Instance == null)
//            {
//                Instance = this;
//                DontDestroyOnLoad(gameObject);
//            }
//            else if (Instance != this)
//            {
//                Destroy(this.gameObject);
//                return;
//            }
//        }

//#if UNITY_EDITOR


//        public void Init(UnityAction<bool> initCallback)
//        {
//            initCallback(true);
//        }

//        public void InitInterstitialAd()
//        {
            
//        }
//        public void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback)
//        {
//            adLoadCallback(false);
//        }
//        public bool IsInterstitialAdAvailable()
//        {
//            return false;
//        }
//        public void ShowIterstitialAd(UnityAction closeCallback)
//        {
//            closeCallback();
//        }


//        public void InitRewardedAd()
//        {
            
//        }
//        public void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback)
//        {
//            adLoadCallback(false);
//        }
//        public bool IsRewardedAdAvailable()
//        {
//            return false;
//        }
//        public void ShowRewardedAd(UnityAction<bool> closeCallback)
//        {
//            closeCallback(false);
//        }

//#else
//        public void Init(UnityAction<bool> initCallback)
//        {
//            //Log.L("AdsMediation FANController->Init called");
//        }

//        #region rewarded

//        private UnityAction<bool> onRWAdLoadCallback;
//        private UnityAction<bool> onRWCloseCallback;

//        private RewardedVideoAd rewardedVideoAd;
//        private bool isRWAdLoaded;
//        private bool rewardPending;


//        public void InitRewardedAd()
//        {
//            Log.L("AdsMediation FANController->InitRewardedAd");
//        }

//        public void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback)
//        {
//            Log.L("AdsMediation FANController->LoadRewardedAd adUnitId" + adUnitId);
//            onRWAdLoadCallback = adLoadCallback;

//            if (rewardedVideoAd != null) rewardedVideoAd.Dispose();
//            rewardedVideoAd = null;

//            isRWAdLoaded = false;
//            // Create the rewarded video unit with a placement ID (generate your own on the Facebook app settings).
//            // Use different ID for each ad placement in your app.
//            this.rewardedVideoAd = new RewardedVideoAd(adUnitId);

//            this.rewardedVideoAd.Register(this.gameObject);

//            // Set delegates to get notified on changes or when the user interacts with the ad.
//            this.rewardedVideoAd.RewardedVideoAdDidLoad = (delegate ()
//            {
//                Log.L("AdsMediation FANController->RewardedVideo ad loaded.");
//                this.isRWAdLoaded = true;
//                onRWAdLoadCallback?.Invoke(true);
//            });
//            this.rewardedVideoAd.RewardedVideoAdDidFailWithError = (delegate (string error)
//            {
//                Log.L("AdsMediation FANController->RewardedVideo ad failed to load with error: " + error);
//                onRWAdLoadCallback?.Invoke(false);
//            });

//            this.rewardedVideoAd.RewardedVideoAdDidClose = (delegate ()
//            {
//                Log.L("AdsMediation FANController->Rewarded video ad did close.");
//                if (this.rewardedVideoAd != null)
//                {
//                    this.rewardedVideoAd.Dispose();
//                }
//                isRWAdLoaded = false;

//                if (rewardPending)
//                {
//                    StartCoroutine(RunPostUpdate(onRWCloseCallback, true));
//                }
//                else
//                {
//                    StartCoroutine(RunPostUpdate(onRWCloseCallback, false));
//                }
//                rewardPending = false;
//            });
//            rewardedVideoAd.RewardedVideoAdComplete = (delegate ()
//            {
//                Log.L("AdsMediation FANController->Rewarded video ad did complete.");
//                rewardPending = true;
//            });

//            // Initiate the request to load the ad.
//            this.rewardedVideoAd.LoadAd();
//        }

//        public bool IsRewardedAdAvailable()
//        {

//            bool aval = rewardedVideoAd != null && isRWAdLoaded;
//            Log.L("AdsMediation FANController->IsRewardedAdAvailable aval.:" + aval);
//            return aval;
//        }
//        public void ShowRewardedAd(UnityAction<bool> closeCallback)
//        {
//            Log.L("AdsMediation FANController->ShowRewardedAd");
//            onRWCloseCallback = closeCallback;

//            if (IsRewardedAdAvailable())
//            {
//                rewardedVideoAd.Show();
//            }
//        }
//        #endregion rewarded

//        #region interstitial
//        private UnityAction<bool> onINTAdLoadCallback;
//        private UnityAction onINTCloseCallback;

//        private InterstitialAd interstitialAd;
//        private bool isINTAdLoaded;

//        public void InitInterstitialAd()
//        {
//            Log.L("AdsMediation FANController->InitInterstitialAd");
//        }
//        public void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback)
//        {
//            Log.L("AdsMediation FANController->LoadInterstitial adUnitId" + adUnitId);
//            onINTAdLoadCallback = adLoadCallback;

//            if (interstitialAd != null) interstitialAd.Dispose();
//            interstitialAd = null;

//            isINTAdLoaded = false;

//            this.interstitialAd = new InterstitialAd(adUnitId);
//            this.interstitialAd.Register(this.gameObject);

//            // Set delegates to get notified on changes or when the user interacts with the ad.
//            this.interstitialAd.InterstitialAdDidLoad = (delegate ()
//            {
//                Log.L("AdsMediation FANController->Interstitial ad loaded.");
//                this.isINTAdLoaded = true;
//                onINTAdLoadCallback?.Invoke(true);
//            });
//            interstitialAd.InterstitialAdDidFailWithError = (delegate (string error)
//            {
//                Log.L("AdsMediation FANController->Interstitial ad failed to load with error: " + error);
//                onINTAdLoadCallback?.Invoke(false);
//            });

//            this.interstitialAd.interstitialAdDidClose = (delegate ()
//            {
//                Log.L("AdsMediation FANController->Interstitial ad did close.");
//                if (this.interstitialAd != null)
//                {
//                    this.interstitialAd.Dispose();
//                }

//                isINTAdLoaded = false;
//                StartCoroutine(RunPostUpdate(onINTCloseCallback));
//            });

//            // Initiate the request to load the ad.
//            this.interstitialAd.LoadAd();
//        }

//        public bool IsInterstitialAdAvailable()
//        {
//            bool aval = interstitialAd != null && isINTAdLoaded;

//            Log.L("AdsMediation FANController->IsInterstitialAdAvailable aval." + aval);
//            return aval;
//        }
//        public void ShowIterstitialAd(UnityAction closeCallback)
//        {
//            Log.L("AdsMediation FANController->ShowIterstitialAd");
//            onINTCloseCallback = closeCallback;

//            if (IsInterstitialAdAvailable())
//            {
//                interstitialAd.Show();
//            }
//        }
//        #endregion interstitial





//        IEnumerator RunPostUpdate(UnityAction _method)
//        {
//            // If RunOnMainThread() is called in a secondary thread,
//            // this coroutine will start on the secondary thread
//            // then yield until the end of the frame on the main thread
//            yield return null;

//            _method();
//        }

//        IEnumerator RunPostUpdate(UnityAction<bool> _method, bool param)
//        {
//            // If RunOnMainThread() is called in a secondary thread,
//            // this coroutine will start on the secondary thread
//            // then yield until the end of the frame on the main thread
//            yield return null;

//            _method(param);
//        }
        
//#endif
//    }
//}