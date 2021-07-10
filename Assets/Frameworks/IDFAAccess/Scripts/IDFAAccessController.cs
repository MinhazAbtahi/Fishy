using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_IOS
using System.Runtime.InteropServices;
using UnityEngine.iOS;
#endif

namespace FPG
{
    public static class IDFAAccessController
    {
#if UNITY_IOS

        #region Custom Native Popup Bridge
        delegate void NativeAlertResultCallback(bool result);

        [AOT.MonoPInvokeCallback(typeof(NativeAlertResultCallback))]
        static void NativeAlertResultReceived(bool result)
        {
            if (result) UserAllowedIDFAPopUp();
            else UserPostponedIDFAPopUp();
        }

        [DllImport("__Internal")]
        private static extern void ShowNativeAlertForIDFA(string title, string message, NativeAlertResultCallback callback);

        #endregion Custom Native Popup Bridge

        #region IDFA Popup Bridge
        delegate void IDFAAccessRequestCallback(bool status);

        [AOT.MonoPInvokeCallback(typeof(IDFAAccessRequestCallback))]
        static void IDFAAccessRequestFulfilled(bool status)
        {
            ProcessPermission(status);
            RequestFulfillCallback?.Invoke(status);
            RequestFulfillCallback = null;
        }

        [DllImport("__Internal")]
        private static extern void RequestIDFAAccess(IDFAAccessRequestCallback callback);
        #endregion IDFA Popup Bridge

        #region Authorize Status Bridge
        [DllImport("__Internal")]
        private static extern bool IsAuthorized();
        [DllImport("__Internal")]
        private static extern bool IsDenied();
        #endregion Authorize Status Bridge

#endif
        public static bool IOS14OrGreater()
        {
#if UNITY_IOS && !UNITY_EDITOR
                        //Determine whether its ios 14 or greater
            Version currentVersion = new Version(Device.systemVersion); // Parse the version of the current OS
            Version ios14_5 = new Version("14.5"); // Parse the iOS 14.0 version constant

            if (currentVersion >= ios14_5)
            {
                return true;
            }
            else
                return false;
#else
            return true;
#endif
        }

        private static void ProcessPermission(bool granted)
        {
            if (granted)
            {
                FirebaseManager.GetInstance().LogAnalyticsEvent("IDFA_AccessGranted");
            }
            else FirebaseManager.GetInstance().LogAnalyticsEvent("IDFA_AccessDenied");
        }

        public static System.Action<bool> RequestFulfillCallback = null;
        public static void RequestAccess(System.Action<bool> callback = null)
        {
#if UNITY_IOS
            if (IOS14OrGreater())
            {
                RequestFulfillCallback = callback;
                RequestIDFAAccess(IDFAAccessRequestFulfilled);
            }
            else
            {
                callback?.Invoke(true);
            }
#else
            callback?.Invoke(true);
#endif
        }
        
        private static bool IsGenerallyEligible()
        {
#if UNITY_IOS && !UNITY_EDITOR
            if (!IOS14OrGreater() || IsAuthorized() || IsDenied()) return false;
            
            return true;
#else
            return false;
#endif
        }

#region Public API

        private const string KEY_ASKED_AT_BEGINNING = "IDFAAskedAtBeginning";
        private const string KEY_LAST_ACCESS_ASKED_LEVEL = "LastIDFAAskedLevel";

        public static bool AskIDFAAccessAtTheBeginning()
        {
            //Debug.Log("IDFA_ method called...");
            if (!IsGenerallyEligible()) return false;

            bool shouldAskAtBeginning = Array.Exists(TagManager.GetIDFAPromptShowLevels(), lvl => lvl == 0);

            bool eligible = shouldAskAtBeginning && !PlayerPrefs.HasKey(KEY_ASKED_AT_BEGINNING);
            //Debug.Log("IDFA_ shouldAskAtBeginning: " + shouldAskAtBeginning + "... Eligible: "+ eligible);
            if (eligible)
            {
                PlayerPrefs.SetInt(KEY_ASKED_AT_BEGINNING, 1);
                ShowPrompt();
                //Debug.Log("IDFA_ popup show ...");
            }
            return eligible;
        }
        public static bool AskIDFAAccessAtLevel(int gameLevel)
        {
            if (!IsGenerallyEligible()) return false;

            bool shouldAskAtThisLevel = Array.Exists(TagManager.GetIDFAPromptShowLevels(), lvl => lvl == gameLevel);

            bool eligible = shouldAskAtThisLevel && gameLevel > PlayerPrefs.GetInt(KEY_LAST_ACCESS_ASKED_LEVEL, -1);

            if (eligible)
            {
                PlayerPrefs.SetInt(KEY_LAST_ACCESS_ASKED_LEVEL, gameLevel);
                ShowPrompt();
            }
            return eligible;
        }
#endregion Public API

#region Show Popup
        private static void ShowPrompt()
        {
            switch (TagManager.GetIDFAPromptType())
            {
                case 0: RequestAccessDirectly(); break;
                case 1: ShowiOSNativeCustomPrompt(); break;
                case 2: PanelIDFAPrompt.ShowUnityIDFAPrompt();break;
                default:
                    break;
            }
        }

        private static void ShowiOSNativeCustomPrompt()
        {
#if UNITY_IOS && !UNITY_EDITOR
            //call objc wrapper
            ShowNativeAlertForIDFA("You are in control", TagManager.GetIDFAPromptMessage() ,NativeAlertResultReceived);
#endif
        }

        private static void RequestAccessDirectly()
        {
#if UNITY_IOS && !UNITY_EDITOR
            RequestAccess();
#endif
            FirebaseManager.GetInstance().LogAnalyticsEvent("IDFA_AccessRequested");
        }
#endregion Show Popup

#region Callbacks
        public static void UserAllowedIDFAPopUp()
        {
#if UNITY_IOS && !UNITY_EDITOR
            RequestAccess();
#endif
            FirebaseManager.GetInstance().LogAnalyticsEvent("IDFA_BtnAllowPressed");
        }

        public static void UserPostponedIDFAPopUp()
        {
            FirebaseManager.GetInstance().LogAnalyticsEvent("IDFA_BtnNotNowPressed");
        }
#endregion Callbacks

    }
}
