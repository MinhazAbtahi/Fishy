using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using Facebook.Unity;

namespace FPG
{
    public class FacebookManager
    {

        private static FacebookManager Instance;
        private FacebookManager() { }

        public static FacebookManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new FacebookManager();
            }
            return Instance;
        }

        public void Init()
        {
            //if (!FB.IsInitialized)
            //{
            //    // Initialize the Facebook SDK
            //    FB.Init(InitCallback, OnHideUnity);
            //}
            //else
            //{
            //    // Already initialized, signal an app activation App Event
            //    FB.ActivateApp();
            //}
        }

        private void InitCallback()
        {
            //if (FB.IsInitialized)
            //{
            //    // Signal an app activation App Event
            //    FB.ActivateApp();
            //    // Continue with Facebook SDK
            //    // ...
            //}
            //else
            //{
            //    Log.L("Failed to Initialize the Facebook SDK");
            //}
        }

        private void OnHideUnity(bool isGameShown)
        {
            if (!isGameShown)
            {
                // Pause the game - we will need to hide
                Time.timeScale = 0;
            }
            else
            {
                // Resume the game - we're getting focus again
                Time.timeScale = 1;
            }
        }

        #region Events

        public void logEvent(string eventName, float eventValue, Dictionary<string, object> parameters)
        {
            //FB.LogAppEvent(eventName, eventValue, parameters);
        }

        public void LogStartLevelEvent(int level)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters["Level"] = level;
            //FB.LogAppEvent(
            //    "LevelStart",
            //    null,
            //    parameters
            //);
        }

        public void LogFailedLevelEvent(int level)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters["Level"] = level;
            //FB.LogAppEvent(
            //    "LevelFailed",
            //    null,
            //    parameters
            //);
        }

        public void LogCompleteLevelEvent(int level)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters["Level"] = level;
            //FB.LogAppEvent(
            //    "LevelComplete",
            //    null,
            //    parameters
            //);
        }

        public void LogCompleteBonusLevelEvent(int level)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters["Level"] = level;
            //FB.LogAppEvent(
            //    "BonusLevelComplete",
            //    null,
            //    parameters
            //);
        }

        //IAP

        public void ProductPurchaseEvent(string productID)
        {
            //var parameters = new Dictionary<string, object>();
            //parameters["ProductID"] = productID;
            //decimal purchaseAmount = 1;

            //FB.LogPurchase(purchaseAmount, null, parameters);
        }

        //Ad

        public void InterstitialAdShownEvent()
        {
            //FB.LogAppEvent("Interstitial_Shown");
        }

        public void RewardedAdCompleteEvent()
        {
            //FB.LogAppEvent("RewardedVideo_Complete");
        }
    }
    #endregion
}