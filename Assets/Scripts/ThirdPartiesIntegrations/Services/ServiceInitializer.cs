using System;

namespace ThirdPartiesIntegrations.Services
{
    public abstract class ServiceInitializer
    {
        protected readonly bool _isManualInitializtionEnabled;
        protected bool _isInitialized;

        public bool IsInitialized => _isInitialized;

        public event Action ServiceInitialized;

        public ServiceInitializer(bool isManualInitilizationEnabled)
        {
            _isManualInitializtionEnabled = isManualInitilizationEnabled;
            if (!_isManualInitializtionEnabled)
            {
                Initialize();
            }
        }

        public void SubscribeToInitializationEvent(Action callback)
        {
            if (_isInitialized)
            {
                callback();
            }
            else
            {
                void AutoUnsubscribe()
                {
                    callback();
                    ServiceInitialized -= AutoUnsubscribe;
                }
                ServiceInitialized += AutoUnsubscribe;
            }
        }

        public virtual void InitializeManually() => Initialize();

        protected virtual void Initialize()
        {
            if (_isInitialized)
            {
                CSDL.LogWarning($"{GetType().Name} already initialized! Check your initialization order.");
                return;
            }
            _isInitialized = true;
            ServiceInitialized?.Invoke();
        }
    }
}
