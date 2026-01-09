using System;

namespace ThirdPartiesIntegrations.Services
{
    public interface IServiceInitializer
    {
        event Action ServiceInitialized;

        void InitializeManually();
        void SubscribeToInitializationEvent(Action callback);
    }
}