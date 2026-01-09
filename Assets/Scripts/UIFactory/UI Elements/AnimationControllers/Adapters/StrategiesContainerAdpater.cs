using ModificatedUISystem.UIElements.Animation.Strategies;
using CustomAttributes;
using System;
using UnityEngine;

namespace ModificatedUISystem.UIElements.Animation.Adapters
{
    public class StrategiesContainerAdpater : MonoBehaviour, IUIAnimationStrategy
    {
        [SerializeField, ForceInterface(typeof(IUIAnimationStrategy))]
        private UnityEngine.Object[] _serializedStrategies;
        private IUIAnimationStrategy[] _strategies;

        public event AnimationControllerEventHandler AnimationFinished;

        private void Awake()
        {
            int? length = _serializedStrategies?.Length;
            if (length == null)
            {
                throw new NullReferenceException($"{nameof(StrategiesContainerAdpater)} on {gameObject.name} strategies not provided!");
            }
            _strategies = new IUIAnimationStrategy[length.Value];
            for (int i = 0; i < length.Value; i++)
            {
                _strategies[i] = _serializedStrategies[i] as IUIAnimationStrategy;
            }
            _strategies[length.Value - 1].AnimationFinished += LastObjectFinishedAnimation;
        }

        public void Hide()
        {
            foreach (var strategy in _strategies)
                strategy.Hide();
        }

        public void Show()
        {
            foreach (var strategy in _strategies)
                strategy.Show();
        }

        private void LastObjectFinishedAnimation(AnimationType animationType)
            => AnimationFinished?.Invoke(animationType);
    }
}
