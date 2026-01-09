using Cysharp.Threading.Tasks;
using ThirdPartiesIntegrations.Resolver;
using Unity.Services.Core;
using UnityEngine.UnityConsent;

namespace ThirdPartiesIntegrations.Services.UnityServicesIntegration
{
    public class UnityServicesInitializer : ServiceInitializer, IUnityServicesInitializer
    {
        private IPoliciesResolver _policiesResolver;
        private bool _isPoliciesResolved;

        public UnityServicesInitializer(bool isManualInitilizationEnabled, IPoliciesResolver policiesResolver)
            : base(isManualInitilizationEnabled)
        {
            _policiesResolver = policiesResolver;
            _policiesResolver.SubscribeToPoliciesResolutionEvent(OnPoliciesResolved);
        }

        protected override void Initialize()
        {
            _ = InitiliazeAsync();
        }

        private void OnPoliciesResolved()
        {
            bool coppa = _policiesResolver.IsPolicyConsentObtained(PolicyType.COPPA);
            bool publisherConsent = _policiesResolver.IsPolicyConsentObtained(PolicyType.PUBLISHER);
            bool gdpr = _policiesResolver.IsPolicyConsentObtained(PolicyType.GDPR);
            bool ccpa = _policiesResolver.IsPolicyConsentObtained(PolicyType.CCPA);

            ConsentStatus adsConsentStatus = coppa && gdpr && ccpa ? ConsentStatus.Granted : ConsentStatus.Denied;
            ConsentState consentState = EndUserConsent.GetConsentState();
            consentState.AdsIntent = adsConsentStatus;

            EndUserConsent.SetConsentState(consentState);
            _isPoliciesResolved = true;
        }

        private async UniTaskVoid InitiliazeAsync()
        {
            await UniTask.WaitUntil(() => _isPoliciesResolved);
            await UnityServices.InitializeAsync();
            base.Initialize();
        }
    }
}
