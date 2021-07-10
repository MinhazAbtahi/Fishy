using Firebase.RemoteConfig;
using UnityEngine;

public static class TagManager
{


    private static bool IsNull()
    {
        return FPG.FirebaseManager.GetInstance() is null;
    }
    private static bool IsInitiated()
    {
        return FPG.FirebaseManager.GetInstance().firebaseInitialized;
    }
    private static bool Unsafe()
    {
        return IsNull() || !IsInitiated();
    }

    #region remote config getters

    public static readonly string KEY_AD_SEQUENCE = "adSearchOrder2";
    //public static readonly string VALUE_AD_SEQUENCE = "201,101,203,102,205,402,207,302,206,404,208,104,301,401";
    public static readonly string VALUE_AD_SEQUENCE = "701";
    private static int[] adOrderList;
    public static int[] GetAdSequence()
    {
        //if (adOrderList is null)
        {
            string[] orderListStr;

#if UNITY_ANDROID
            if (Unsafe()) orderListStr = VALUE_AD_SEQUENCE.Split(',');
            else orderListStr = FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_AD_SEQUENCE).StringValue.Split(',');
#else
            orderListStr = VALUE_AD_SEQUENCE.Split(',');
#endif

            adOrderList = new int[orderListStr.Length];

            for (int i = 0; i < orderListStr.Length; i++)
            {
                adOrderList[i] = System.Int32.Parse(orderListStr[i]);
            }
        }

        return adOrderList;
    }
    public static readonly string KEY_BANNER_ENABLED = "BannerStatus";
    public static readonly int VALUE_BANNER_ENABLED = 0;
    public static bool IsBannerActivated()
    {
        if (Unsafe()) return VALUE_BANNER_ENABLED == 0;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_BANNER_ENABLED).StringValue) == 1;
    }

    public static readonly string KEY_ENABLE_TENJIN = "EnableTenjin";
    public static readonly int VALUE_ENABLE_TENJIN = 1;
    public static bool IsTenjinEnabled()
    {
        if (Unsafe()) return VALUE_ENABLE_TENJIN == 1;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_ENABLE_TENJIN).StringValue) == 1;
    }

    //Conversion Value

    public static readonly string KEY_INITIAL_CV_IMPRESSION = "InitialCVThresholdForImpression";
    public static readonly int VALUE_INITIAL_CV_IMPRESSION = 20;
    public static int GetInitialCVThresholdForImpression()
    {
        if (Unsafe()) return VALUE_INITIAL_CV_IMPRESSION;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_INITIAL_CV_IMPRESSION).StringValue);
    }
    public static readonly string KEY_CV_STEP_IMPRESSION = "CVStepForImpression";
    public static readonly int VALUE_CV_STEP_IMPRESSION = 5;
    public static int GetCVStepForImpression()
    {
        if (Unsafe()) return VALUE_CV_STEP_IMPRESSION;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_CV_STEP_IMPRESSION).StringValue);
    }

    public static readonly string KEY_INITIAL_CV_PURCHASE = "InitialCVThresholdForPurchase";
    public static readonly int VALUE_INITIAL_CV_PURCHASE = 25;
    public static int GetInitialCVThresholdForPurchase()
    {
        if (Unsafe()) return VALUE_INITIAL_CV_PURCHASE;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_INITIAL_CV_PURCHASE).StringValue);
    }
    public static readonly string KEY_CV_STEP_PURCHASE = "CVStepForPurchase";
    public static readonly int VALUE_CV_STEP_PURCHASE = 5;
    public static int GetCVStepForPurchase()
    {
        if (Unsafe()) return VALUE_CV_STEP_PURCHASE;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_CV_STEP_PURCHASE).StringValue);
    }
    public static readonly string KEY_CONVERSION_VALUE_LIFETIME_HOUR = "ConversionValueLifetimeInHour";
    public static readonly int VALUE_CONVERSION_VALUE_LIFETIME_HOUR = 24;
    public static int GetConversionValueLifetime()
    {
        if (Unsafe()) return VALUE_CONVERSION_VALUE_LIFETIME_HOUR;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_CONVERSION_VALUE_LIFETIME_HOUR).StringValue);
    }

    //IDFA

    public static readonly string KEY_IDFA_MESSSAGE = "IDFAMsg";
    public static readonly string VALUE_IDFA_MESSSAGE = "Because of privacy regulations we need to ask your permission for using your data to serve personalised ads. If you don't allow tracking, we won't be able to serve more relevant ads to you.";
    public static string GetIDFAPromptMessage()
    {
        if (Unsafe()) return VALUE_IDFA_MESSSAGE;
        return (FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_IDFA_MESSSAGE).StringValue);
    }

    public static readonly string KEY_IDFA_LEVELS = "IDFAPromptlevels";
    public static readonly string VALUE_IDFA_LEVELS = "0,2,7";
    public static int[] GetIDFAPromptShowLevels()
    {
        string[] levelsInString;

        if (Unsafe()) levelsInString = VALUE_IDFA_LEVELS.Split(',');
        else levelsInString = FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_IDFA_LEVELS).StringValue.Split(',');

        int[] levels = new int[levelsInString.Length];
        for (int i = 0; i < levelsInString.Length; i++)
        {
            levels[i] = System.Int32.Parse(levelsInString[i]);
        }

        return levels;
    }

    public static readonly string KEY_IDFA_POPUP_TYPE = "IDFAPopupType";
    public static readonly int VALUE_IDFA_POPUP_TYPE = 0; // 0=NoPopUp, 1==NativePopUp, 2==UnityPopUp
    public static int GetIDFAPromptType()
    {
        if (Unsafe()) return VALUE_IDFA_POPUP_TYPE;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_IDFA_POPUP_TYPE).StringValue);
    }

    public static readonly string KEY_AD_TREE_REGENERATE_TIME = "AdTreeReGenerateTime";
    public static readonly int VALUE_AD_TREE_REGENERATE_TIME = 5;
    public static int GetAdTreeRegenerateTime()
    {
        if (Unsafe()) return VALUE_AD_TREE_REGENERATE_TIME;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_AD_TREE_REGENERATE_TIME).StringValue);
    }

    public static readonly string KEY_ENEMY_SPAWN_AMOUNT = "EnemySpawnAmount";
    public static readonly int VALUE_ENEMY_SPAWN_AMOUNT = 40;
    public static int GetEnemySpawnAmount()
    {
        if (Unsafe()) return VALUE_ENEMY_SPAWN_AMOUNT;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_ENEMY_SPAWN_AMOUNT).StringValue);
    }

    public static readonly string KEY_ENEMY_SPAWN_TIME = "EnemySpawnTime";
    public static readonly int VALUE_ENEMY_SPAWN_TIME = 20;
    public static int GetEnemySpawnTime()
    {
        if (Unsafe()) return VALUE_ENEMY_SPAWN_TIME;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_ENEMY_SPAWN_TIME).StringValue);
    }
    public static readonly string KEY_PET_ENABLED = "PetStatus";
    public static readonly int VALUE_PET_ENABLED = 0;
    public static bool IsPetActivated()
    {
        if (Unsafe()) return VALUE_PET_ENABLED == 0;
        return System.Int32.Parse(FirebaseRemoteConfig.DefaultInstance.GetValue(KEY_PET_ENABLED).StringValue) == 1;
    }
    #endregion remote config getters
}
