using System;

namespace ThirdPartiesIntegrations.Resolver
{
    public interface IPoliciesResolver
    {
        event Action PoliciesResolved;

        bool IsPolicyConsentObtained(PolicyType policyType);
        void SubscribeToPoliciesResolutionEvent(Action callback);
    }
}