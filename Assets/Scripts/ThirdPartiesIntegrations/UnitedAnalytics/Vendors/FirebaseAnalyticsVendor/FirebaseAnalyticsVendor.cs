using Firebase.Analytics;
using System;
using System.Collections.Generic;
using System.Reflection;
using ThirdPartiesIntegrations.Resolver;
using ThirdPartiesIntegrations.Services;
using ThirdPartiesIntegrations.Services.Firebase;

namespace ThirdPartiesIntegrations.UnitedAnalytics.Vendors
{
    public class FirebaseAnalyticsVendor : IAnalyticsVendor
    {
        private bool _isFirebaseInitialized;
        private bool _isPoliciesResolved;

        private IPoliciesResolver _policiesResolver;

        public FirebaseAnalyticsVendor(IFirebaseInitializer firebaseInitializer, IPoliciesResolver policiesResolver)
        {
            _policiesResolver = policiesResolver;
            ((ServiceInitializer)firebaseInitializer).SubscribeToInitializationEvent(OnFirebaseInitialized);
        }

        public void LogEvent(string name, params EventParameter[] parameters)
        {
            if (!_isFirebaseInitialized || !_isPoliciesResolved)
            {
                CSDL.LogWarning($"{nameof(FirebaseAnalyticsVendor)} can't log event because Firebase is not initialized yet");
                return;
            }
            Parameter[] firebaseParams = ConvertEventParametersToFirebaseParameters(parameters);
            if (firebaseParams == null)
            {
                return;
            }
            FirebaseAnalytics.LogEvent(GetAppropriateFirebaseEventName(name), firebaseParams);
        }

        private string GetAppropriateFirebaseEventName(string name)
        {
            return name switch
            {
                "unitedAnalytics_levelStarted" => FirebaseAnalytics.EventLevelStart,
                "unitedAnalytics_tutorialBegin" => FirebaseAnalytics.EventTutorialBegin,
                "unitedAnalytics_tutorialCompleted" => FirebaseAnalytics.EventTutorialComplete,
                _ => name
            };
        }

        private Parameter[] ConvertEventParametersToFirebaseParameters(EventParameter[] paramteters)
        {
            List<Parameter> convertedParams = new List<Parameter>();
            for (int i = 0; i < paramteters.Length; i++)
            {
                ConstructorInfo firebaseParamCtor = GetSuitableCtorByValue(paramteters[i].Value);
                if (firebaseParamCtor != null)
                {
                    Parameter instance = (Parameter)firebaseParamCtor.Invoke(new object[] { paramteters[i].Name, paramteters[i].Value });
                    convertedParams.Add(instance);
                }
            }
            if (convertedParams.Count != paramteters.Length)
            {
                CSDL.LogError($"{nameof(FirebaseAnalyticsVendor)} can't create suitable {nameof(Parameter)}' " +
                    $"for provided {nameof(EventParameter)}");
                return null;
            }
            return convertedParams.ToArray();
        }

        private ConstructorInfo GetSuitableCtorByValue(object value)
        {
            Type firebaseParamType = typeof(Parameter);
            foreach (ConstructorInfo ctor in firebaseParamType.GetConstructors())
            {
                ParameterInfo ctorValueParam = ctor.GetParameters()[1];
                if (ctorValueParam.ParameterType.IsAssignableFrom(value.GetType()))
                {
                    return ctor;
                }
            }
            return null;
        }

        private void OnPoliciesResolved()
        {
            bool coppa = _policiesResolver.IsPolicyConsentObtained(PolicyType.COPPA);
            bool publisherConsent = _policiesResolver.IsPolicyConsentObtained(PolicyType.PUBLISHER);
            bool gdpr = _policiesResolver.IsPolicyConsentObtained(PolicyType.GDPR);
            bool ccpa = _policiesResolver.IsPolicyConsentObtained(PolicyType.CCPA);
            FirebaseAnalytics.SetAnalyticsCollectionEnabled(coppa && publisherConsent && gdpr && ccpa);
            _isPoliciesResolved = true;
        }

        private void OnFirebaseInitialized()
        {
            _isFirebaseInitialized = true;
            _policiesResolver.SubscribeToPoliciesResolutionEvent(OnPoliciesResolved);
        }
    }
}
