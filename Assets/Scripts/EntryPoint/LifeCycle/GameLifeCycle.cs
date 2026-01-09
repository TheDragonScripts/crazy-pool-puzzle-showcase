using Cysharp.Threading.Tasks;
using System;
using UnityEngine;

namespace EntryPoint.LifeCycle
{
    public class GameLifeCycle : MonoBehaviour, IGameLifeCycle
    {
        private static GameLifeCycle _instance;
        public LifeCycleStage CurrentStage { get; private set; }

        public event Action<LifeCycleStage> OnStateChanged;

        private void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(gameObject);
            SetStage(LifeCycleStage.PreInit);
        }

        private void Start()
        {
            SetStage(LifeCycleStage.Init);
            _ = FinishInitialization();
        }

        public void SubscribeToStage(LifeCycleStage stage, Action callback)
        {
            if (CurrentStage >= stage)
            {
                callback();
            }
            else
            {
                void CallbackHandler(LifeCycleStage s)
                {
                    if (s == stage)
                    {
                        OnStateChanged -= CallbackHandler;
                        callback();
                    }
                }
                OnStateChanged += CallbackHandler;
            }
        }

        private async UniTaskVoid FinishInitialization()
        {
            await UniTask.WaitForEndOfFrame();
            SetStage(LifeCycleStage.PostInit);
            await UniTask.WaitForEndOfFrame();
            SetStage(LifeCycleStage.Ready);
        }

        private void SetStage(LifeCycleStage stage)
        {
            CurrentStage = stage;
            OnStateChanged?.Invoke(stage);
        }
    }
}
