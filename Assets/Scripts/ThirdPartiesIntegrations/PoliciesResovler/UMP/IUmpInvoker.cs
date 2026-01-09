using System;

namespace ThirdPartiesIntegrations.UMP
{
    public interface IUmpInvoker
    {
        bool IsConsentStringsReceived { get; }
        bool IsUmpUpdatedConsentAtLeastOnce { get; }

        event Action UmpReceivedConsentStrings;

        bool IsPrivacyOptionsButtonRequired();
        void ShowPrivacyOptionsForm();
        void SubscribeToConsentStringsReceiveEvent(Action callback);
    }
}