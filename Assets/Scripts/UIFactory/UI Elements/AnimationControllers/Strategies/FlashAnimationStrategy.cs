using UnityEngine;

namespace ModificatedUISystem.UIElements.Animation.Strategies
{
    public class FlashAnimationStrategy : MonoBehaviour, IUIAnimationStrategy
    {
        [SerializeField] private FadeAnimationStrategy _fadeAnimationStrategy;
        private bool _isPlaying;

        public event AnimationControllerEventHandler AnimationFinished;

        private void Awake()
        {
            _fadeAnimationStrategy.AnimationFinished += OnAnimationFinished;
        }

        public void Hide()
        {
            _isPlaying = false;
            _fadeAnimationStrategy.Hide();
        }

        public void Show()
        {
            _isPlaying = true;
            _fadeAnimationStrategy.Show();
        }

        private void OnAnimationFinished(AnimationType animationType)
        {
            if (!_isPlaying)
            {
                return;
            }
            switch (animationType)
            {
                case AnimationType.Show:
                    _fadeAnimationStrategy.Hide();
                    break;
                case AnimationType.Hide:
                    _fadeAnimationStrategy.Show();
                    break;
                default:
                    break;
            }
            AnimationFinished?.Invoke(animationType);
        }
    }
}
