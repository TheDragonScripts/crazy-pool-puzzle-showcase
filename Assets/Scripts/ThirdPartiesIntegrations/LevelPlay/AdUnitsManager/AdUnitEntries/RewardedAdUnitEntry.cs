using System;
using Unity.Services.LevelPlay;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public partial class AdUnitsManager
    {
        private class RewardedAdUnitEntry : IAdUnitEntry
        {
            private readonly LevelPlayRewardedAd _ad;
            private readonly string _adUnitId;

            public string AdUnitId => _adUnitId;
            public bool IsRequested { get; set; }

            public RewardedAdUnitEntry(string adUnitId, LevelPlayRewardedAd ad)
            {
                _adUnitId = adUnitId;
                _ad = ad;
            }

            public void ConnectToAdUnitEvents(Action<LevelPlayAdInfo, LevelPlayReward> rewardedCallback,
                Action<LevelPlayAdInfo> adLoadedCallback, Action<LevelPlayAdError> adLoadFailedCallback, 
                Action<LevelPlayAdInfo> adClosedCallback, Action<LevelPlayAdInfo> adDisplayedCallback)
            {
                _ad.OnAdRewarded += rewardedCallback;
                _ad.OnAdLoaded += adLoadedCallback;
                _ad.OnAdLoadFailed += adLoadFailedCallback;
                _ad.OnAdClosed += adClosedCallback;
                _ad.OnAdDisplayed += adDisplayedCallback;
            }

            public bool IsAdReady()
            {
                return _ad.IsAdReady();
            }

            public void LoadAd()
            {
                _ad.LoadAd();
            }

            public void ShowAd()
            {
                _ad.ShowAd();
            }
        }
    }
}
