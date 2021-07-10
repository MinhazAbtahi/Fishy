using UnityEngine;
using System;
using System.Threading.Tasks;
using Firebase;
using Firebase.Analytics;
using Firebase.Extensions;
using Firebase.RemoteConfig;
using UnityEngine.Events;

namespace FPG
{
    public class FirebaseManager
    {
        public static FirebaseManager Instance { get; set; }

        private DependencyStatus dependencyStatus = DependencyStatus.UnavailableOther;
        public bool firebaseInitialized = false;

        private FirebaseManager() { }


        public static FirebaseManager GetInstance()
        {
            if (Instance == null)
            {
                Instance = new FirebaseManager();
            }
            return Instance;
        }

        public void Init()
        {
            UpdateSessionCount();
            FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
            {
                dependencyStatus = task.Result;
                if (dependencyStatus == DependencyStatus.Available)
                {
                    InitializeFirebase();
                }
                else
                {
                    Debug.LogError("FirebaseManager -> Could not resolve all Firebase dependencies: " + dependencyStatus);
                }
            });
        }

        private void UpdateSessionCount()
        {
            //Session Counter is for the app rating purposes.
            //PlayerPrefs.SetInt(Constants.SessionCounter, PlayerPrefs.GetInt(Constants.SessionCounter, 0) + 1);
        }


        private void InitializeFirebase()
        {
            //Log.L("FirebaseManager -> Enabling data collection.");
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(true);

            FirebaseAnalytics.SetSessionTimeoutDuration(new TimeSpan(0, 30, 0));
            firebaseInitialized = true;

            //Send app open event
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventAppOpen);

            SetGameRetentionData();
            InitializeRemoteConfigData();
        }


        void SetGameRetentionData()
        {
            ///////////////////////// retention data in firebase/////////////////

            //DateTime currentDate = DateTime.Now;

            //Date dt2;
            //dt2.d = currentDate.Day;
            //dt2.m = currentDate.Month;
            //dt2.y = currentDate.Year;

            //bool canRetentionCount = false;
            //int lastTimeRetentionCount = PlayerPrefs.GetInt("lastTimeRetentionTimeStamp", 0);
            //if (lastTimeRetentionCount == 0)
            //{
            //    canRetentionCount = true;
            //}
            //else
            //{
            //    int retentionTimeStamp = lastTimeRetentionCount;
            //    DateTime retentionDate = AppDelegate.ConvertFromUnixTimestamp(retentionTimeStamp);

            //    Date dt3;
            //    dt3.d = retentionDate.Day;
            //    dt3.m = retentionDate.Month;
            //    dt3.y = retentionDate.Year;

            //    int lastRetentionDayDiff = Math.Abs(AppDelegate.GetNumberOfDaysDifference(dt3, dt2));
            //    if (lastRetentionDayDiff > 0)
            //        canRetentionCount = true;

            //}

            //if (canRetentionCount)
            //{
            //    string firstOpenTimeStampKey = "firstOpenTimeStamp";
            //    int firstAppOpenDate = AppDelegate.GetTime();
            //    if (!PlayerPrefs.HasKey(firstOpenTimeStampKey))
            //    {
            //        PlayerPrefs.SetInt(firstOpenTimeStampKey, firstAppOpenDate);
            //    }

            //    int firstTimeStamp = PlayerPrefs.GetInt(firstOpenTimeStampKey, firstAppOpenDate);
            //    DateTime startDate = AppDelegate.ConvertFromUnixTimestamp(firstTimeStamp);

            //    Date dt1;
            //    dt1.d = startDate.Day;
            //    dt1.m = startDate.Month;
            //    dt1.y = startDate.Year;

            //    int retentionDay = AppDelegate.GetNumberOfDaysDifference(dt1, dt2);
            //    string strDay = (retentionDay <= 9) ? "0" + retentionDay.ToString() : retentionDay.ToString();
            //    string retentionEventName = "retentionDay" + strDay;

            //    LogAnalyticsEvent(retentionEventName);
            //    PlayerPrefs.SetInt("lastTimeRetentionTimeStamp", AppDelegate.GetTime());

            //}
        }

        #region Remote Config

        private void InitializeRemoteConfigData()
        {
            //Set default values (These values will be used unless Fetched data from the console)

            System.Collections.Generic.Dictionary<string, object> defaults =
                new System.Collections.Generic.Dictionary<string, object>();

            // These are the values that are used if we haven't fetched data from the
            // service yet, or if we ask for values that the service doesn't have:

            defaults.Add(TagManager.KEY_AD_SEQUENCE, TagManager.VALUE_AD_SEQUENCE);
            defaults.Add(TagManager.KEY_BANNER_ENABLED, TagManager.VALUE_BANNER_ENABLED);
            defaults.Add(TagManager.KEY_ENABLE_TENJIN, TagManager.VALUE_ENABLE_TENJIN);
            defaults.Add(TagManager.KEY_IDFA_MESSSAGE, TagManager.VALUE_IDFA_MESSSAGE);
            defaults.Add(TagManager.KEY_IDFA_LEVELS, TagManager.VALUE_IDFA_LEVELS);
            defaults.Add(TagManager.KEY_IDFA_POPUP_TYPE, TagManager.VALUE_IDFA_POPUP_TYPE);
            defaults.Add(TagManager.KEY_INITIAL_CV_IMPRESSION, TagManager.VALUE_INITIAL_CV_IMPRESSION);
            defaults.Add(TagManager.KEY_CV_STEP_IMPRESSION, TagManager.VALUE_CV_STEP_IMPRESSION);
            defaults.Add(TagManager.KEY_INITIAL_CV_PURCHASE, TagManager.VALUE_INITIAL_CV_PURCHASE);
            defaults.Add(TagManager.KEY_CV_STEP_PURCHASE, TagManager.VALUE_CV_STEP_PURCHASE);
            defaults.Add(TagManager.KEY_CONVERSION_VALUE_LIFETIME_HOUR, TagManager.VALUE_CONVERSION_VALUE_LIFETIME_HOUR);
            defaults.Add(TagManager.KEY_AD_TREE_REGENERATE_TIME, TagManager.VALUE_AD_TREE_REGENERATE_TIME);
            defaults.Add(TagManager.KEY_ENEMY_SPAWN_AMOUNT, TagManager.VALUE_ENEMY_SPAWN_AMOUNT);
            defaults.Add(TagManager.KEY_ENEMY_SPAWN_TIME, TagManager.VALUE_ENEMY_SPAWN_TIME);
            defaults.Add(TagManager.KEY_PET_ENABLED, TagManager.VALUE_PET_ENABLED);

            FirebaseRemoteConfig.DefaultInstance.SetDefaultsAsync(defaults);
            FetchDataAsync();
        }

        private Task FetchDataAsync()
        {
            //Log.L("FirebaseManager -> Fetching data...");
            // FetchAsync only fetches new data if the current data is older than the provided
            // timespan.  Otherwise it assumes the data is "recent enough", and does nothing.
            // By default the timespan is 12 hours, and for production apps, this is a good
            // number.  For this example though, it's set to a timespan of zero, so that
            // changes in the console will always show up immediately.
            System.Threading.Tasks.Task fetchTask = Firebase.RemoteConfig.FirebaseRemoteConfig.DefaultInstance.FetchAsync(
                TimeSpan.Zero);
            return fetchTask.ContinueWithOnMainThread(FetchComplete);
        }


        private void FetchComplete(Task fetchTask)
        {
            if (fetchTask.IsCanceled)
            {
                //Log.L("FirebaseManager -> Fetch canceled.");
                // Defult level loader
            }
            else if (fetchTask.IsFaulted)
            {
                //Log.L("FirebaseManager -> Fetch encountered an error.");
                // Defult level loader
            }
            else if (fetchTask.IsCompleted)
            {
                //Log.L("FirebaseManager -> Fetch completed successfully!");
                FirebaseRemoteConfig.DefaultInstance.ActivateAsync();
            }
            RemoteConfigFetchComplete();
        }

        public UnityAction FetchCompleteCallback;
        void RemoteConfigFetchComplete()
        {
            FetchCompleteCallback?.Invoke();
            TenjinManager.GetInstance().Init();
        }

        #endregion

        #region Events

        public void Analytics_LevelFinished(string levelCounter)
        {
            FirebaseAnalytics.LogEvent(
                FirebaseAnalytics.EventLevelEnd,
                new Parameter(FirebaseAnalytics.ParameterLevelName, levelCounter),
                new Parameter(FirebaseAnalytics.ParameterSuccess, "Success"));

            string param = "level_finished_" + levelCounter;
            FirebaseAnalytics.LogEvent(param);
        }

        public void Analytics_LevelFailed(string levelCounter)
        {
            FirebaseAnalytics.LogEvent(
                FirebaseAnalytics.EventLevelEnd,
                new Parameter(FirebaseAnalytics.ParameterLevelName, levelCounter),
                new Parameter(FirebaseAnalytics.ParameterSuccess, "Fail"));

            string param = "level_failed_" + levelCounter;
            FirebaseAnalytics.LogEvent(param);
        }

        public void Analytics_Login()
        {
            //Log.L("Logging a login event.");
            FirebaseAnalytics.LogEvent(FirebaseAnalytics.EventLogin);
        }

        public void Analytics_ConversionValueUpdated(int conversionValue)
        {
            FirebaseAnalytics.LogEvent("conversion_value_updated", "Conversion_Value", conversionValue);
        }

        /*public void LogAnalyticsEvent(string eventName, params Parameter[] parameters)
        {
            FirebaseAnalytics.LogEvent(eventName, parameters);
        }*/

        public void LogAnalyticsEvent(string eventName)
        {
            if (!firebaseInitialized) return;

            FirebaseAnalytics.LogEvent(eventName.Replace(".", "_"));
        }

        public void LogAnalyticsEvent(string eventName, string paramName, string paramValue)
        {
            if (!firebaseInitialized) return;

            FirebaseAnalytics.LogEvent(eventName.Replace(".", "_"), paramName.Replace(".", "_"), paramValue.Replace(".", "_"));
        }
        #endregion
    }
}