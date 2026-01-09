using Cysharp.Threading.Tasks;
using EntryPoint.Levels;
using SDI;
using ThirdPartiesIntegrations.IAP;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public class AdsInvoker : IAdsInvoker
    {
        private int _interstitialAdsCooldown;
        private int _lastLevelWithoutAds;
        private string _interstitialAdInGameId;

        private bool _canShowInterstitialAd;

        private ILevelManager _levelManager;
        private IInAppPurchasingIntegration _inAppPurchasingIntegration;

        private static ILevelPlayIntegration _levelPlayIntegration;
        private static bool _isAdsEnabled = true;

        public AdsInvoker(AdsInvokerSettingsScriptableObject adsInvokerSettings)
        {
            adsInvokerSettings.Deconstruct(out _interstitialAdsCooldown, out _lastLevelWithoutAds, out _interstitialAdInGameId);
        }

        [InjectionMethod]
        public void Inject(ILevelManager levelManager, IInAppPurchasingIntegration inAppPurchasingIntegration,
             ILevelPlayIntegration levelPlayIntegration)
        {
            _levelManager = levelManager;
            _inAppPurchasingIntegration = inAppPurchasingIntegration;
            _levelPlayIntegration ??= levelPlayIntegration;

            _levelManager.PlayableLevelLoaded += OnPlayableLevelLoaded;
            _levelPlayIntegration.AdClosed += OnAdClosed;
            _inAppPurchasingIntegration.SubscribeToFetchEvent(() => _isAdsEnabled = !_inAppPurchasingIntegration.IsAnyProductPurchased());

            _ = DoInterstitialAdCooldownAsync();
        }

        public void InvokeAd(string inGameAdId)
        {
            TryShowAd(inGameAdId);
        }

        private void TryShowAd(string inGameAdId)
        {
            if (_isAdsEnabled)
            {
                _levelPlayIntegration?.ShowAd(inGameAdId);
            }
        }

        private void OnAdClosed(string inGameAdId)
        {
            if (inGameAdId == _interstitialAdInGameId)
            {
                _canShowInterstitialAd = false;
                _ = DoInterstitialAdCooldownAsync();
            }
        }

        private void OnPlayableLevelLoaded(int level)
        {
            if (level <= _lastLevelWithoutAds || !_canShowInterstitialAd)
            {
                return;
            }
            TryShowAd(_interstitialAdInGameId);
        }

        private async UniTaskVoid DoInterstitialAdCooldownAsync()
        {
            await UniTask.WaitForSeconds(_interstitialAdsCooldown);
            _canShowInterstitialAd = true;
        }
    }
}
