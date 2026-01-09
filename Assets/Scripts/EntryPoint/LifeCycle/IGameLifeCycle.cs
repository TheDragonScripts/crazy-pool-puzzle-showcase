using System;

namespace EntryPoint.LifeCycle
{
    public interface IGameLifeCycle
    {
        LifeCycleStage CurrentStage { get; }
        event Action<LifeCycleStage> OnStateChanged;
        void SubscribeToStage(LifeCycleStage stage, Action callback);
    }
}
