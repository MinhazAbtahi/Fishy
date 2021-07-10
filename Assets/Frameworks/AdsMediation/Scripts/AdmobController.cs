//using System;
//using UnityEngine;
//using GoogleMobileAds.Api;
//using UnityEngine.Events;
//using System.Collections;

//namespace FPG {
//    public class AdmobController : MonoBehaviour, IAds
//    {
//        private static AdmobController Instance;
//        public static AdmobController GetInstance()
//        {
//            //if (Instance is null) Instance = new AdmobController();
//            return Instance;
//        }
//        //private AdmobController(){}

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
//            //Log.L("AdsMediation AdmobController->Init called");
//            MobileAds.Initialize(initStatus => {
//                Log.L("AdsMediation AdmobController->Init initstatus:"+initStatus);
//                initCallback?.Invoke(true);
//                //StartCoroutine(RunPostUpdate(initCallback, true));
//            });

//            int savedAdCounter = PlayerPrefs.GetInt("AdCounter", 0);
//            PlayerPrefs.GetInt("AdCounter", savedAdCounter + 1);
//            Networking.getInstance().total_ad_show_counter += 1;
//            Networking.getInstance().sendUserAdStatus(1, 0, 0, 0, 0, 0, 0, "init");
//        }

//        private AdRequest BuildAdRequest()
//        {
//            //AdRequest request = new AdRequest.Builder().AddExtra("npa", consent).AddExtra("is_designed_for_families", designedForFamilies).TagForChildDirectedTreatment(directedForChildren).Build();
//            //interstitialAd.LoadAd(request);

//            var b = new AdRequest.Builder();

//            b.TestDevices.Add("88819173BF9FD18B93DA3631D1EE1D3B");//Xiaomi 6A

//            b.TestDevices.Add("C65D61B5E26334F78BAD0011C173A8D7");//Photon Vai

//            b.TestDevices.Add("3BB9A49AB695C0BCBF877809C4B682E4");//Rocky Vai

//            b.TestDevices.Add("460a00aa47076038f72067078c292b8e");//IPad Rocky Vai

//            return b.Build();
//        }

//#region rewarded
//        private RewardedAd rewardedAd;
//        private bool rewardPending;
//        private UnityAction<bool> onRWAdLoadCallback;
//        private UnityAction<bool> onRWCloseCallback;

//        public void InitRewardedAd()
//        {
//            Log.L("AdsMediation AdmobController->InitRewardedAd");
//        }

//        public void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback)
//        {
//            Log.L("AdsMediation AdmobController->LoadRewardedAd adUnitId:"+ adUnitId);

//            onRWAdLoadCallback = adLoadCallback;

//            //if(rewardedAd != null)
//            //{
//            //    rewardedAd.Dispose();
//            //}
//            rewardedAd = new RewardedAd(adUnitId);

//            rewardedAd.OnAdLoaded += HandleRewardedAdLoaded;
//            rewardedAd.OnAdFailedToLoad += HandleRewardedAdFailedToLoad;
//            rewardedAd.OnUserEarnedReward += HandleUserEarnedReward;
//            rewardedAd.OnAdFailedToShow += HandleRewardedAdFailedToShow;
//            rewardedAd.OnAdClosed += HandleRewardedAdClosed;

//            //AdRequest request = new AdRequest.Builder().Build();
//            rewardedAd.LoadAd(BuildAdRequest());
//        }

//#region rewarded ad interface
//        public void HandleRewardedAdLoaded(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("AdsMediation AdmobController->HandleRewardedAdLoaded event received");
//            onRWAdLoadCallback?.Invoke(true);
//            //StartCoroutine(RunPostUpdate(onRWAdLoadCallback, true));
//        }

//        public void HandleRewardedAdFailedToLoad(object sender, AdErrorEventArgs args)
//        {
//            MonoBehaviour.print(
//                "AdsMediation AdmobController->HandleRewardedAdFailedToLoad event received with message: "
//                                 + args.Message);
//            onRWAdLoadCallback?.Invoke(false);
//            //StartCoroutine(RunPostUpdate(onRWAdLoadCallback, false));
//        }

//        public void HandleUserEarnedReward(object sender, Reward args)
//        {
//            string type = args.Type;
//            double amount = args.Amount;
//            MonoBehaviour.print(
//                "AdsMediation AdmobController->HandleRewardedAdRewarded event received for "
//                            + amount.ToString() + " " + type);

//            rewardPending = true;
//        }

//        public void HandleRewardedAdFailedToShow(object sender, AdErrorEventArgs args)
//        {
//            MonoBehaviour.print(
//                "AdsMediation AdmobController->HandleRewardedAdFailedToShow event received with message: "
//                                 + args.Message);
//            //onRWCloseCallback?.Invoke(false);
//            StartCoroutine(RunPostUpdate(onRWCloseCallback, false));
//        }

//        public void HandleRewardedAdClosed(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("AdsMediation AdmobController->HandleRewardedAdClosed event received");
//            //onRWCloseCallback?.Invoke(rewardPending);

//            if (rewardPending)
//            {
//                StartCoroutine(RunPostUpdate(onRWCloseCallback, true));
//            }
//            else
//            {
//                StartCoroutine(RunPostUpdate(onRWCloseCallback, false));
//            }
//            rewardPending = false;
//        }


//#endregion rewarded ad interface

//        public bool IsRewardedAdAvailable()
//        {
//            bool aval = rewardedAd != null && rewardedAd.IsLoaded();
//            Log.L("AdsMediation AdmobController->IsRewardedAdAvailable aval. " + aval);
//            return aval;
//        }

//        public void ShowRewardedAd(UnityAction<bool> closeCallback)
//        {
//            Log.L("AdsMediation AdmobController->ShowRewardedAd");
//            onRWCloseCallback = closeCallback;
//            if (IsRewardedAdAvailable())
//            {
//                rewardedAd.Show();
//            }
//        }
//#endregion rewarded


//#region interstitial

//        private InterstitialAd interstitialAd;
//        private UnityAction<bool> onINTAdLoadCallback;
//        private UnityAction onINTCloseCallback;

//        public void InitInterstitialAd()
//        {
//            Log.L("AdsMediation AdmobController->InitInterstitialAd");
//        }

//        public void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback)
//        {
//            Log.L("AdsMediation AdmobController->LoadInterstitialAd adUnitId: "+ adUnitId);
//            onINTAdLoadCallback = adLoadCallback;

//            interstitialAd = new InterstitialAd(adUnitId);

//            interstitialAd.OnAdLoaded += HandleOnAdLoaded;
//            interstitialAd.OnAdFailedToLoad += HandleOnAdFailedToLoad;
//            interstitialAd.OnAdClosed += HandleOnAdClosed;
//            interstitialAd.OnAdLeavingApplication += HandleOnAdLeavingApplication;

//            //AdRequest request = new AdRequest.Builder().Build();
//            interstitialAd.LoadAd(BuildAdRequest());
//        }

//#region interstitial ad interface
//        public void HandleOnAdLoaded(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("AdsMediation AdmobController->HandleAdLoaded event received");
//            onINTAdLoadCallback?.Invoke(true);
//            //StartCoroutine(RunPostUpdate(onINTAdLoadCallback, true));
//        }

//        public void HandleOnAdFailedToLoad(object sender, AdFailedToLoadEventArgs args)
//        {
//            MonoBehaviour.print("AdsMediation AdmobController->HandleFailedToReceiveAd event received with message: "
//                                + args.Message);
//            onINTAdLoadCallback?.Invoke(false);
//            //StartCoroutine(RunPostUpdate(onINTAdLoadCallback, false));
//        }

//        public void HandleOnAdClosed(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("AdsMediation AdmobController->HandleAdClosed event received");
//            //onINTCloseCallback?.Invoke();
//            StartCoroutine(RunPostUpdate(onINTCloseCallback));
//        }

//        public void HandleOnAdLeavingApplication(object sender, EventArgs args)
//        {
//            MonoBehaviour.print("AdsMediation AdmobController->HandleAdLeavingApplication event received");
//        }

//#endregion interstitial ad interface

//        public bool IsInterstitialAdAvailable()
//        {
//            bool aval = interstitialAd != null && interstitialAd.IsLoaded();
//            Log.L("AdsMediation AdmobController->LoadInterstitialAd IsInterstitialAdAvailable aval. " + aval);
//            return aval;
//        }
//        public void ShowIterstitialAd(UnityAction closeCallback)
//        {
//            Log.L("AdsMediation AdmobController->LoadInterstitialAd ShowIterstitialAd");
//            onINTCloseCallback = closeCallback;
//            if (IsInterstitialAdAvailable())
//            {
//                interstitialAd.Show();
//            }
//        }
//#endregion interstitial


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