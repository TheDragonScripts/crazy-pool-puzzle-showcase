using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using ThirdPartiesIntegrations.Resolver;
using ThirdPartiesIntegrations.Services;
using ThirdPartiesIntegrations.Services.UnityServicesIntegration;
using Unity.Services.Analytics;
using UnityEngine.UnityConsent;

namespace ThirdPartiesIntegrations.UnitedAnalytics.Vendors
{
    public partial class UnityAnalyticsVendor : IAnalyticsVendor
    {
        private Dictionary<string, Type> _cachedEventTypes = new Dictionary<string, Type>();
        private ServiceInitializer _unityServicesInitializer;
        private IPoliciesResolver _policiesResolver;
        private bool _isUnityServicesInitialized;
        private bool _isAnalyticsConsentObtained;

        public UnityAnalyticsVendor(IUnityServicesInitializer unityServicesInitializer, IPoliciesResolver policiesResolver)
        {
            _unityServicesInitializer = (UnityServicesInitializer)unityServicesInitializer;
            _unityServicesInitializer.SubscribeToInitializationEvent(OnUnityServicesInitialized);

            _policiesResolver = policiesResolver;
            _policiesResolver.SubscribeToPoliciesResolutionEvent(OnPoliciesResolved);
        }

        public void LogEvent(string name, params EventParameter[] parameters)
        {
            if (!_isUnityServicesInitialized)
            {
                CSDL.LogWarning($"{nameof(UnityAnalyticsVendor)} {nameof(LogEvent)} method called too early. Unity Services " +
                    $"is not initialized yet. Initialization process is delegated to {nameof(IUnityServicesInitializer)}");
                return;
            }
            Event unityEvent = TryInstantiateEventByName(name, parameters);
            if (unityEvent != null)
            {
                AnalyticsService.Instance.RecordEvent(unityEvent);
            }
        }

        private Event TryInstantiateEventByName(string name, EventParameter[] parameters)
        {
            Type type = GetEventTypeByName(name);

            object[] convertedCtorParams = ConvertEventParametersToCtorParameters(parameters);
            ConstructorInfo ctor = GetSuitableConstructor(type, convertedCtorParams);
            object instance = ctor.Invoke(convertedCtorParams);

            return (Event)instance;
        }

        private object[] ConvertEventParametersToCtorParameters(EventParameter[] parameters)
        {
            object[] convertedParams = new object[parameters.Length];
            for (int i = 0; i < parameters.Length; i++)
            {
                convertedParams[i] = parameters[i].Value;
            }
            return convertedParams;
        }

        private ConstructorInfo GetSuitableConstructor(Type type, object[] parameters)
        {
            ConstructorInfo suitableCtor = null;
            ConstructorInfo[] ctors = type.GetConstructors(BindingFlags.Instance | BindingFlags.Public);

            foreach (ConstructorInfo ctor in ctors)
            {
                if (IsSuitableConstructor(ctor, parameters))
                {
                    suitableCtor = ctor;
                    break;
                }
            }
            if (suitableCtor == null)
            {
                CSDL.LogError($"{nameof(UnityAnalyticsVendor)} can' find suitable ctor for {nameof(Event)}. " +
                    $"Probably arguments is not suitable or their order is incorrect");
            }
            return suitableCtor;
        }

        private bool IsSuitableConstructor(ConstructorInfo ctor, object[] parameters)
        {
            ParameterInfo[] requestedParams = ctor.GetParameters();
            if (requestedParams.Length != parameters.Length)
            {
                return false;
            }
            for (int i = 0; i < requestedParams.Length; i++)
            {
                if (!IsParameterMatch(requestedParams[i], parameters[i]))
                {
                    return false;
                }
            }
            return true;
        }

        private bool IsParameterMatch(ParameterInfo parameterInfo, object value)
        {
            if (value == null)
            {
                return !parameterInfo.ParameterType.IsValueType
                    || Nullable.GetUnderlyingType(parameterInfo.ParameterType) != null;
            }
            return parameterInfo.ParameterType.IsAssignableFrom(value.GetType());
        }

        private Type GetEventTypeByName(string name)
        {
            if (!_cachedEventTypes.ContainsKey(name))
            {
                Type suitableType = Assembly.GetExecutingAssembly().GetTypes()
                    .FirstOrDefault(t => t.GetCustomAttribute<UnityEventNameAttribute>()?.EventName == name);
                if (suitableType != null)
                {
                    _cachedEventTypes.Add(name, suitableType);
                }
                else
                {
                    CSDL.LogError($"{nameof(UnityAnalyticsVendor)} can' find {nameof(Event)} marked " +
                        $"as {name} with {nameof(UnityEventNameAttribute)}");
                    return null;
                }
            }
            return _cachedEventTypes[name];
        }

        private void OnPoliciesResolved()
        {
            bool coppa = _policiesResolver.IsPolicyConsentObtained(PolicyType.COPPA);
            bool publisherConsent = _policiesResolver.IsPolicyConsentObtained(PolicyType.PUBLISHER);
            bool gdpr = _policiesResolver.IsPolicyConsentObtained(PolicyType.GDPR);
            bool ccpa = _policiesResolver.IsPolicyConsentObtained(PolicyType.CCPA);

            bool analyticsConsent = coppa && ccpa && gdpr && publisherConsent;
            ConsentStatus consentStatus = analyticsConsent ? ConsentStatus.Granted : ConsentStatus.Denied;
            ConsentState consentState = EndUserConsent.GetConsentState();
            consentState.AnalyticsIntent = consentStatus;
            EndUserConsent.SetConsentState(consentState);

            _isAnalyticsConsentObtained = analyticsConsent;
            _unityServicesInitializer.InitializeManually();
        }

        private void OnUnityServicesInitialized()
        {
            _isUnityServicesInitialized = true;
            if (!_isAnalyticsConsentObtained)
            {
                AnalyticsService.Instance.RequestDataDeletion();
            }
        }
    }
}
