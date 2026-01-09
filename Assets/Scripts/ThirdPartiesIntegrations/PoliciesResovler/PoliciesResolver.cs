using GoogleMobileAds.Api;
using ModificatedUISystem;
using SDI;
using System;
using System.Collections.Generic;
using ThirdPartiesIntegrations.AgeGateSystem;
using ThirdPartiesIntegrations.GPP;
using ThirdPartiesIntegrations.UMP;
using UnityEngine;

namespace ThirdPartiesIntegrations.Resolver
{
    public class PoliciesResolver : IPoliciesResolver
    {
        private IAgeGate _ageGate;
        private IUmpInvoker _umpInvoker;
        private Dictionary<PolicyType, Policy> _policies;
        private bool _isPoliciesResolved;

        public event Action PoliciesResolved;

        private class Policy
        {
            public bool IsRequired;
            public bool IsResolved;
            public bool IsConsentObtained;
        }

        [InjectionMethod]
        public void Inject(IUIFactory uiFactroy)
        {
            _ageGate = new AgeGateSystem.AgeGate(uiFactroy);
            _policies = new Dictionary<PolicyType, Policy>();
            foreach (PolicyType policyId in Enum.GetValues(typeof(PolicyType)))
            {
                _policies.Add(policyId, new Policy());
            }
            _ageGate.SubscribeToAgeGroupObtainingEvent(ResolveCoppa);
        }

        public bool IsPolicyConsentObtained(PolicyType policyType)
        {
            Policy policy = _policies[policyType];
            return policy.IsResolved && (!policy.IsRequired || policy.IsConsentObtained);
        }

        public void SubscribeToPoliciesResolutionEvent(Action callback)
        {
            if (_isPoliciesResolved)
            {
                callback();
                return;
            }
            else
            {
                void AutoUnsubscribe()
                {
                    callback();
                    PoliciesResolved -= AutoUnsubscribe;
                }
                PoliciesResolved += AutoUnsubscribe;
            }
        }

        private void ResolveGdpr()
        {
            string encodedConsent = ApplicationPreferences.GetString("IABTCF_AddtlConsent")
                ?? PlayerPrefs.GetString("IABTCF_AddtlConsent");
            if (string.IsNullOrEmpty(encodedConsent))
            {
                SetPolicyAsNotReqiured(PolicyType.GDPR);
                return;
            }
            string[] slices = encodedConsent.Split('~');
            if (slices.Length < 2)
            {
                UpdatePolicyResolution(PolicyType.GDPR, false);
                return;
            }
            string version = slices[0];
            string section = slices[1];
            UpdatePolicyResolution(PolicyType.GDPR, (version == "1" || version == "2")
                && section.Contains("2878"));
        }

        private void ResolveCcpa()
        {
            string gpp = ApplicationPreferences.GetString("IABGPP_HDR_GppString");
            if (string.IsNullOrEmpty(gpp))
            {
                SetPolicyAsNotReqiured(PolicyType.CCPA);
                return;
            }
            PrimitiveGppParser parser = new PrimitiveGppParser();
            if (parser.Parse(gpp, out DecodedGppData decodedData))
            {
                bool isConsentObtained = decodedData.SaleOptOut switch
                {
                    0 => true,  // Not applicable or Opt in
                    1 => false, // Opt out
                    2 => true,  // Opt in
                    _ => false
                };
                UpdatePolicyResolution(PolicyType.CCPA, isConsentObtained);
            }
            else
            {
                UpdatePolicyResolution(PolicyType.CCPA, false);
            }
        }

        private void ResolvePublisher()
        {
            string encodedConsent = ApplicationPreferences.GetString("IABTCF_PublisherConsent");
            if (string.IsNullOrEmpty(encodedConsent))
            {
                SetPolicyAsNotReqiured(PolicyType.PUBLISHER);
                return;
            }
            char storeAndAccessData = encodedConsent[0];
            char developAndImprove = encodedConsent[9];
            UpdatePolicyResolution(PolicyType.PUBLISHER, storeAndAccessData == '1'
                && developAndImprove == '1');
        }

        private void ResolveCoppa(bool isUserAdult)
        {
            UpdatePolicyResolution(PolicyType.COPPA, isUserAdult);
            if (!isUserAdult)
            {
                return;
            }
            CallUmp();
        }

        private void CallUmp()
        {
            _umpInvoker = new UmpInvoker(_policies[PolicyType.COPPA].IsConsentObtained);
            _umpInvoker.SubscribeToConsentStringsReceiveEvent(() =>
            {
                ResolveGdpr();
                ResolveCcpa();
                ResolvePublisher();
                _isPoliciesResolved = true;
                PoliciesResolved?.Invoke();
            });
        }

        private void SetPolicyAsNotReqiured(PolicyType policyType)
        {
            Policy policy = _policies[policyType];
            policy.IsRequired = false;
            policy.IsResolved = true;
        }

        private void UpdatePolicyResolution(PolicyType policyType, bool consentStatus)
        {
            Policy policy = _policies[policyType];
            policy.IsRequired = true;
            policy.IsResolved = true;
            policy.IsConsentObtained = consentStatus;
        }
    }
}
