using System;
using Unity.Services.LevelPlay;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public partial class AdUnitsManager
    {
        private class InterstitialAdUnitEntry : IAdUnitEntry
        {
            private readonly LevelPlayInterstitialAd _ad;
            private readonly string _adUnitId;

            public string AdUnitId => _adUnitId;
            public bool IsRequested { get; set; }

            public InterstitialAdUnitEntry(string adUnitId, LevelPlayInterstitialAd ad)
            {
                _adUnitId = adUnitId;
                _ad = ad;
            }

            public void ConnectToAdUnitEvents(Action<LevelPlayAdInfo> adLoadedCallback, Action<LevelPlayAdError> adLoadFailedCallback,
                Action<LevelPlayAdInfo> adClosedCallback, Action<LevelPlayAdInfo> adDisplayedCallback)
            {
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
