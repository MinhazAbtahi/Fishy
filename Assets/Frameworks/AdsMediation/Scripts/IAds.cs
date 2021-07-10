
namespace FPG
{
    using UnityEngine.Events;

    public interface IAds
    {
        void Init(UnityAction<bool> initCallback);

        void InitRewardedAd();
        void LoadRewardedAd(string adUnitId, UnityAction<bool> adLoadCallback);
        bool IsRewardedAdAvailable();
        void ShowRewardedAd(UnityAction<bool> closeCallback);

        void InitInterstitialAd();
        void LoadInterstitialAd(string adUnitId, UnityAction<bool> adLoadCallback);
        bool IsInterstitialAdAvailable();
        void ShowIterstitialAd(UnityAction closeCallback);
    }
}