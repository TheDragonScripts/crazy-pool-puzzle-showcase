using GoogleMobileAds.Ump.Api;
using System;

namespace ThirdPartiesIntegrations.UMP
{
    /// <summary>
    /// Invokes User Messaging Platform (UMP) form and reports when consents strings
    /// is received and can be accessed via PlayerPrefs.
    /// </summary>
    /// <remarks>Developed to work in tandem with Policies Resolver class.</remarks>
    public class UmpInvoker : IUmpInvoker
    {
        public bool IsUmpUpdatedConsentAtLeastOnce { get; private set; }
        public bool IsConsentStringsReceived { get; private set; }

        public event Action UmpReceivedConsentStrings;

        public UmpInvoker(bool isUserAdult)
        {
            ConsentRequestParameters request = new ConsentRequestParameters
            {
                TagForUnderAgeOfConsent = !isUserAdult
            };
            ConsentInformation.Update(request, OnConsentInfoUpdated);
        }

        public void SubscribeToConsentStringsReceiveEvent(Action callback)
        {
            if (IsConsentStringsReceived)
            {
                callback();
            }
            else
            {
                void AutoUnsubscribe()
                {
                    callback();
                    UmpReceivedConsentStrings -= AutoUnsubscribe;
                }
                UmpReceivedConsentStrings += AutoUnsubscribe;
            }
        }

        public bool IsPrivacyOptionsButtonRequired()
        {
            return IsUmpUpdatedConsentAtLeastOnce && ConsentInformation.PrivacyOptionsRequirementStatus ==
                PrivacyOptionsRequirementStatus.Required;
        }

        public void ShowPrivacyOptionsForm()
        {
            ConsentForm.ShowPrivacyOptionsForm((FormError showError) =>
            {
                if (showError != null)
                {
                    CSDL.LogError("[UMP] Error showing privacy options form with error: " + showError.Message);
                }
                CSDL.Log("[UMP] Privacy options form shown successfully.");
            });
        }

        private void ShowUmpForm()
        {
            ConsentForm.LoadAndShowConsentFormIfRequired((FormError formError) =>
            {
                if (formError != null)
                {
                    CSDL.LogError($"[UMP] {formError}");
                    return;
                }
                IsConsentStringsReceived = true;
                UmpReceivedConsentStrings?.Invoke();
                /*
                 * Yes, I know about ConsentInformation.CanRequestAds. I don't think we need it, nor do we need 
                 * further initialization of Google's mobile ads. Anyway, we will process these consents by 
                 * custom policies resolver.
                 */
            });
        }

        private void OnConsentInfoUpdated(FormError error)
        {
            if (error != null)
            {
                CSDL.LogError(error.Message);
                return;
            }
            IsUmpUpdatedConsentAtLeastOnce = true;
            ShowUmpForm();
        }
    }
}
