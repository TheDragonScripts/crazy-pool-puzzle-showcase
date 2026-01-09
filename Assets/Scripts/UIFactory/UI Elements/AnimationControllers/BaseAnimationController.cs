using ModificatedUISystem.UIElements.Animation.Strategies;
using System;
using UnityEngine;

namespace ModificatedUISystem.UIElements.Animation
{
    [RequireComponent(typeof(Canvas))]
    public class BaseAnimationController : MonoBehaviour, IUIAnimationController
    {
        private Canvas _canvas;
        private IUIAnimationStrategy[] _strategies;
        private bool _isGameObjectDestroyRequired;

        public bool IsTemporaryHidden { get; private set; }
        public bool IsAnimating { get; private set; }

        public event AnimationControllerEventHandler AnimationFinished;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _strategies = GetComponents<IUIAnimationStrategy>();
            if (_strategies == null)
            {
                throw new NullReferenceException($"{nameof(BaseAnimationController)} can't find any strategies on {gameObject.name}");
            }
            _strategies[_strategies.Length - 1].AnimationFinished += LastObjectFinishedAnimation;
        }

        public void Hide()
        {
            IsAnimating = true;
            _isGameObjectDestroyRequired = true;
            foreach (var strategy in _strategies)
                strategy.Hide();
        }

        public void HideTemporary()
        {
            IsAnimating = true;
            IsTemporaryHidden = true;
            foreach (var strategy in _strategies)
                strategy.Hide();
        }

        public void Show()
        {
            IsAnimating = true;
            IsTemporaryHidden = false;
            _canvas.enabled = true;
            foreach (var strategy in _strategies)
                strategy.Show();
        }

        private void LastObjectFinishedAnimation(AnimationType animationType)
        {
            IsAnimating = false;
            AnimationFinished?.Invoke(animationType);
            if (animationType == AnimationType.Hide)
            {
                _canvas.enabled = false;
                if (_isGameObjectDestroyRequired)
                {
                    Destroy(gameObject);
                }
            }
        }
    }
}