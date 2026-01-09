using System;
using UnityEngine.Purchasing;

namespace ThirdPartiesIntegrations.IAP
{
    public class InAppPurchasingIntegration : IInAppPurchasingIntegration
    {
        private ProductCollection _products;
        private IAPListener _iapListener;
        private bool _isProductsFetchedAtLeastOnce;

        public event Action ProductsFetched;
        public event Action<Product> ProductPurchased;

        public InAppPurchasingIntegration(IAPListener iapListener)
        {
            _iapListener = iapListener;
            _iapListener.onProductsFetched.AddListener(OnProductsFetched);
            _iapListener.onPurchaseComplete.AddListener(OnProductPurchased);
        }

        public void SubscribeToFetchEvent(Action callback)
        {
            if (_isProductsFetchedAtLeastOnce)
            {
                callback();
                return;
            }
            else
            {
                void AutoUnsubscribe()
                {
                    callback();
                    ProductsFetched -= AutoUnsubscribe;
                }
                ProductsFetched += AutoUnsubscribe;
            }
        }

        public string GetProductPriceLocalizedString(string productId)
        {
            Product product = _products?.WithID(productId);
            return product?.metadata?.localizedPriceString ?? string.Empty;
        }

        public bool IsAnyProductPurchased()
        {
            foreach (Product product in _products?.all)
            {
                if (product.hasReceipt)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsSpecificProductPurchased(string productId)
        {
            Product product = _products?.WithID(productId);
            return product != null && product.hasReceipt;
        }

        public void Purchase(string productId)
        {
            CodelessIAPStoreListener.Instance.InitiatePurchase(productId);
        }

        private void OnProductPurchased(Product product)
        {
            ProductPurchased?.Invoke(product);
        }

        private void OnProductsFetched(ProductCollection products)
        {
            _isProductsFetchedAtLeastOnce = true;
            _products = products;
            ProductsFetched?.Invoke();
        }
    }
}
