using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
//using UnityEngine.Purchasing;
//using UnityEngine.Purchasing.MiniJSON;

namespace FPG
{
    public class TenjinManager : MonoBehaviour
    {
        private static TenjinManager Instance;

        public static TenjinManager GetInstance()
        {
            return Instance;
        }

        private static string tenjinApiKey = "KOTDHT7Q6W5Z83N4MJAXZBPKY1Z5KMES";
        private static BaseTenjin TenjinInstance => Tenjin.getInstance(tenjinApiKey);
        private static bool initialized = false;

        public float conversionValue;

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(this.gameObject);
            }
            else if (Instance != this)
            {
                Destroy(this.gameObject);
            }
            conversionValue = PlayerPrefs.GetFloat("ConversionValue", 0);
        }

        public void Init()
        {
            if (!TagManager.IsTenjinEnabled())
                return;

            initialized = true;

#if UNITY_IOS

            //Registers SKAdNetwork app for attribution
            TenjinInstance.RegisterAppForAdNetworkAttribution();

            //Sends install/open event to Tenjin
            TenjinInstance.Connect();

            CheckAndUpdateConversionTracking();

#elif UNITY_ANDROID

            TenjinInstance.Connect();
#endif
        }

        void OnApplicationPause(bool pauseStatus)
        {
            if (pauseStatus)
            {
                //do nothing
            }
            else
            {
                if (initialized) TenjinInstance.Connect();
            }
        }

        #region Conversion Tracking

        void CheckAndUpdateConversionTracking()
        {
            if (PlayerPrefs.GetInt("IsInstallTracked") == 0)
            {
                //New Install

                ulong installTime = (ulong)DateTime.Now.Ticks;
                PlayerPrefs.SetInt("IsInstallTracked", 1);
                PlayerPrefs.SetString("InstallDateTime", installTime.ToString());
            }
            else if (PlayerPrefs.GetInt("IsInstallTracked") == 1)
            {
                //Returning User
                ulong installTimeDiff = (ulong)DateTime.Now.Ticks - ulong.Parse(PlayerPrefs.GetString("InstallDateTime"));
                int conversionValueLifetime = TagManager.GetConversionValueLifetime();

                if (installTimeDiff <= (ulong)(TimeSpan.TicksPerHour * conversionValueLifetime)) //Check for Conversion Value event Lifetime
                {
                    //You will need to use a value between 0-63 for <YOUR 6 bit value>
                    int _conversionValue = Mathf.FloorToInt(conversionValue);
#if UNITY_IOS
                    TenjinInstance.UpdateConversionValue(_conversionValue);
#endif
                    PlayerPrefs.SetInt("LastSentConversionValue", _conversionValue);
                    FPG.FirebaseManager.GetInstance().Analytics_ConversionValueUpdated(_conversionValue);
                    //Debug.Log("Tenjin_ConversionValue INCREMENTED: " + conversionValue);
                }
            }
        }

        public void IncrementConversionValueForImpression()
        {
            int initialCVThresholdForImpression = TagManager.GetInitialCVThresholdForImpression();
            int impressionPointStep = TagManager.GetCVStepForImpression();

            if (conversionValue < initialCVThresholdForImpression)
            {
                conversionValue += 1f;
            }
            else if (conversionValue >= initialCVThresholdForImpression && conversionValue < (initialCVThresholdForImpression + impressionPointStep))
            {
                conversionValue += 0.5f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 2))
            {
                conversionValue += 0.25f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 2) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 3))
            {
                conversionValue += 0.125f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 3) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 4))
            {
                conversionValue += 0.0625f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 4) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 5))
            {
                conversionValue += 0.03125f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 5) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 6))
            {
                conversionValue += 0.015625f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 6) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 7))
            {
                conversionValue += 0.0078125f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 7) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 8))
            {
                conversionValue += 0.00390625f;
            }
            else if (conversionValue >= (initialCVThresholdForImpression + impressionPointStep * 8) && conversionValue < (initialCVThresholdForImpression + impressionPointStep * 9))
            {
                conversionValue += 0.001953125f;
            }

            StoreAndUpdateConversionValue();
        }

        public void IncrementConversionValueForPurchase(int purchaseAmount)
        {
            int initialCVThresholdForPurchase = TagManager.GetInitialCVThresholdForPurchase();
            int purchasePointStep = TagManager.GetCVStepForPurchase();
            float remainingPurchaseAmount;

            //For 1$
            if (purchaseAmount >= 1)
            {
                conversionValue += initialCVThresholdForPurchase;
            }

            remainingPurchaseAmount = purchaseAmount - 1;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 0.25$
            if (remainingPurchaseAmount >= 0.25f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 0.25f;
            }

            remainingPurchaseAmount -= 0.25f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 0.5$
            if (remainingPurchaseAmount >= 0.5f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 0.5f;
            }

            remainingPurchaseAmount -= 0.5f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 1$
            if (remainingPurchaseAmount >= 1f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 1f;
            }

            remainingPurchaseAmount -= 1f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 2$
            if (remainingPurchaseAmount >= 2f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 2f;
            }

            remainingPurchaseAmount -= 2f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 4$
            if (remainingPurchaseAmount >= 4f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 4f;
            }

            remainingPurchaseAmount -= 4f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 8$
            if (remainingPurchaseAmount >= 8f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 8f;
            }

            remainingPurchaseAmount -= 8f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 16$
            if (remainingPurchaseAmount >= 16f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 16f;
            }

            remainingPurchaseAmount -= 16f;
            if (remainingPurchaseAmount <= 0)
            {
                StoreAndUpdateConversionValue();
                return;
            }

            //For Next 32$
            if (remainingPurchaseAmount >= 32f)
            {
                conversionValue += purchasePointStep;
            }
            else
            {
                conversionValue += (remainingPurchaseAmount * purchasePointStep) / 32f;
            }
            //Debug.Log("CV_AFTER_Purchase: " + conversionValue);
            StoreAndUpdateConversionValue();
        }

        private void StoreAndUpdateConversionValue()
        {
            if (conversionValue > 63f)
                conversionValue = 63f;

            PlayerPrefs.SetFloat("ConversionValue", conversionValue);

            int _conversionValue = Mathf.FloorToInt(conversionValue);
            if (_conversionValue - PlayerPrefs.GetInt("LastSentConversionValue", 0) >= 1)
            {
                CheckAndUpdateConversionTracking();
            }
        }

#endregion

#region Purchase Validation

//        public static void OnProcessPurchase(PurchaseEventArgs purchaseEventArgs)
//        {
//            if (!initialized) return;

//            var price = purchaseEventArgs.purchasedProduct.metadata.localizedPrice;
//            double lPrice = decimal.ToDouble(price);
//            var currencyCode = purchaseEventArgs.purchasedProduct.metadata.isoCurrencyCode;

//            var wrapper = Json.Deserialize(purchaseEventArgs.purchasedProduct.receipt) as Dictionary<string, object>;  // https://gist.github.com/darktable/1411710
//            if (null == wrapper)
//            {
//                return;
//            }

//            var payload = (string)wrapper["Payload"]; // For Apple this will be the base64 encoded ASN.1 receipt
//            var productId = purchaseEventArgs.purchasedProduct.definition.id;

//#if UNITY_ANDROID

//            var gpDetails = Json.Deserialize(payload) as Dictionary<string, object>;
//            var gpJson = (string)gpDetails["json"];
//            var gpSig = (string)gpDetails["signature"];

//            CompletedAndroidPurchase(productId, currencyCode, 1, lPrice, gpJson, gpSig);

//#elif UNITY_IOS

//            var transactionId = purchaseEventArgs.purchasedProduct.transactionID;

//            CompletedIosPurchase(productId, currencyCode, 1, lPrice , transactionId, payload);

//#endif

//        }

//        private static void CompletedAndroidPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string Receipt, string Signature)
//        {
//            TenjinInstance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, null, Receipt, Signature);
//        }

//        private static void CompletedIosPurchase(string ProductId, string CurrencyCode, int Quantity, double UnitPrice, string TransactionId, string Receipt)
//        {
//            TenjinInstance.Transaction(ProductId, CurrencyCode, Quantity, UnitPrice, TransactionId, Receipt, null);
//        }

#endregion
    }
}