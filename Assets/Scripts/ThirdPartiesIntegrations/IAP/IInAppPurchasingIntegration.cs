using System;
using UnityEngine.Purchasing;

namespace ThirdPartiesIntegrations.IAP
{
    public interface IInAppPurchasingIntegration
    {
        event Action<Product> ProductPurchased;
        event Action ProductsFetched;

        void SubscribeToFetchEvent(Action callback);
        string GetProductPriceLocalizedString(string productId);
        bool IsAnyProductPurchased();
        bool IsSpecificProductPurchased(string productId);
        void Purchase(string productId);
    }
}