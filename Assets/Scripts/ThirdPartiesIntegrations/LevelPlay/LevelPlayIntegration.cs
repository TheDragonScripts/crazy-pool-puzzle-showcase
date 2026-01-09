using EntryPoint.GameState;
using SDI;
using System;
using ThirdPartiesIntegrations.Resolver;
using Unity.Services.LevelPlay;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public class LevelPlayIntegration : ILevelPlayIntegration
    {
        private readonly string _appKey;
        private readonly bool _isTestSuiteEnabled;
        private readonly AdUnitInfo[] _adsInfo;

        private bool _canShowAds;

        private IPoliciesResolver _policiesResolver;
        private IAdUnitsManager _adUnitsManager;
        private IGamePauseController _gamePauseController;

        public event Action<string> RewardGranted;
        public event Action<string> AdClosed;
        public event Action<string> AdDisplayed;

        public LevelPlayIntegration(LevelPlaySettingsScriptableObject settings)
        {
            settings.Deconstruct(out _appKey, out _adsInfo, out _isTestSuiteEnabled);
            LevelPlay.OnInitSuccess += OnLevelPlayInitialized;
            LevelPlay.OnInitFailed += OnLevelPlayInitializationFailed;
        }

        [InjectionMethod]
        public void Inject(IPoliciesResolver policiesResolver, IGamePauseController gamePauseController)
        {
            _policiesResolver = policiesResolver;
            _gamePauseController = gamePauseController;
            _policiesResolver.SubscribeToPoliciesResolutionEvent(OnPoliciesResolved);
        }

        public void ShowAd(string inGameAdId)
        {
            if (!_canShowAds)
            {
                CSDL.LogWarning($"{nameof(LevelPlayIntegration)} can't show ads, LevelPlay is not initialized.");
                return;
            }
            _adUnitsManager.RequestAd(inGameAdId);
        }

        public void LaunchTestSuite()
        {
            if (_isTestSuiteEnabled)
            {
                IronSource.Agent.launchTestSuite();
                IronSource.Agent.validateIntegration();
            }
            else
            {
                CSDL.LogWarning("You're trying to open a LevelPlay test suite, but it's not enabled.");
            }
        }

        private void ConfigureIronSourceAgent()
        {
            if (_isTestSuiteEnabled)
            {
                IronSource.Agent.setMetaData("is_test_suite", "enable");
            }
            bool isCoppaConsentObtained = _policiesResolver.IsPolicyConsentObtained(PolicyType.COPPA);
            // Configure COPPA
            string reversedCoppaStringConsent = isCoppaConsentObtained ? "false" : "true";
            IronSource.Agent.setMetaData("UnityAds_coppa", reversedCoppaStringConsent);
            IronSource.Agent.setMetaData("AdMob_TFCD", reversedCoppaStringConsent);
            IronSource.Agent.setMetaData("Mintegral_COPPA", reversedCoppaStringConsent);
            IronSource.Agent.setMetaData("is_child_directed", reversedCoppaStringConsent);
            IronSource.Agent.setMetaData("is_deviceid_optout", reversedCoppaStringConsent);
            IronSource.Agent.setMetaData("Google_Family_Self_Certified_SDKS", reversedCoppaStringConsent);
            bool consent = isCoppaConsentObtained
                && _policiesResolver.IsPolicyConsentObtained(PolicyType.GDPR)
                && _policiesResolver.IsPolicyConsentObtained(PolicyType.CCPA);
            string reversedStringConsent = consent ? "false" : "true";
            IronSource.Agent.setConsent(consent);
            IronSource.Agent.setMetaData("is_deviceid_optout", reversedStringConsent);
            IronSource.Agent.setMetaData("do_not_sell", reversedStringConsent);
        }

        private void OnPoliciesResolved()
        {
            ConfigureIronSourceAgent();
            LevelPlay.Init(_appKey);
        }

        private void OnLevelPlayInitializationFailed(LevelPlayInitError error)
        {
            CSDL.LogError(error.ErrorMessage);
        }

        private void OnLevelPlayInitialized(LevelPlayConfiguration configuration)
        {
            _canShowAds = true;
            _adUnitsManager = new AdUnitsManager(_adsInfo);
            _adUnitsManager.RewardGranted += (string id) => RewardGranted?.Invoke(id);
            _adUnitsManager.AdClosed += (string id) =>
            {
                AdClosed?.Invoke(id);
                _gamePauseController.Unpause();
            };
            _adUnitsManager.AdDisplayed += (string id) =>
            {
                AdDisplayed?.Invoke(id);
                _gamePauseController.Pause();
            };
        }
    }
}
