using System.Collections.Generic;
using Unity.Services.LevelPlay;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public partial class AdUnitsManager : IAdUnitsManager
    {
        private Dictionary<string, IAdUnitEntry> _ads = new();

        public event AdUnitsManangerEventHandler RewardGranted;
        public event AdUnitsManangerEventHandler AdClosed;
        public event AdUnitsManangerEventHandler AdDisplayed;

        public AdUnitsManager(AdUnitInfo[] adsInfo)
        {
            LoadAds(adsInfo);
        }

        public void RequestAd(string inGameAdId)
        {
            if (string.IsNullOrEmpty(inGameAdId) || !_ads.ContainsKey(inGameAdId))
            {
                CSDL.LogError($"Provided id {inGameAdId} is not valid");
                return;
            }
            IAdUnitEntry ad = _ads[inGameAdId];
            ad.IsRequested = true;
            ad.LoadAd();
        }

        private void LoadAds(AdUnitInfo[] adsInfo)
        {
            foreach (AdUnitInfo info in adsInfo)
            {
                if (info.AdType == AdUnitType.Rewarded)
                {
                    LevelPlayRewardedAd rewarded = new LevelPlayRewardedAd(info.AdUnitId);
                    RewardedAdUnitEntry adUnitEntry = new RewardedAdUnitEntry(info.AdUnitId, rewarded);
                    adUnitEntry.ConnectToAdUnitEvents(OnAdRewarded, OnAdLoaded, OnAdLoadFailed, OnAdClosed, OnAdDisplayed);
                    _ads.Add(info.InGameAdId, adUnitEntry);
                }
                else if (info.AdType == AdUnitType.Interstitial)
                {
                    LevelPlayInterstitialAd interstitial = new LevelPlayInterstitialAd(info.AdUnitId);
                    InterstitialAdUnitEntry adUnitEntry = new InterstitialAdUnitEntry(info.AdUnitId, interstitial);
                    adUnitEntry.ConnectToAdUnitEvents(OnAdLoaded, OnAdLoadFailed, OnAdClosed, OnAdDisplayed);
                    _ads.Add(info.InGameAdId, adUnitEntry);
                }
            }
        }

        private void ShowAdIfRequested(string adId)
        {
            IAdUnitEntry ad = _ads[adId];
            if (!ad.IsRequested || !ad.IsAdReady())
            {
                return;
            }
            ad.ShowAd();
            ad.IsRequested = false;
        }

        private string GetInGameIdByUnitId(string id)
        {
            foreach (var ad in _ads)
            {
                if (ad.Value.AdUnitId == id)
                {
                    return ad.Key;
                }
            }
            return null;
        }

        private void OnAdRewarded(LevelPlayAdInfo info, LevelPlayReward reward)
        {
            RewardGranted?.Invoke(GetInGameIdByUnitId(info.AdUnitId));
        }

        private void OnAdClosed(LevelPlayAdInfo info)
        {
            AdClosed?.Invoke(GetInGameIdByUnitId(info.AdUnitId));
        }

        private void OnAdDisplayed(LevelPlayAdInfo info)
        {
            AdDisplayed?.Invoke(GetInGameIdByUnitId(info.AdUnitId));
        }

        private void OnAdLoaded(LevelPlayAdInfo info)
        {
            ShowAdIfRequested(GetInGameIdByUnitId(info.AdUnitId));
        }

        private void OnAdLoadFailed(LevelPlayAdError error)
        {
            CSDL.Log(error.ErrorMessage);
        }
    }
}
