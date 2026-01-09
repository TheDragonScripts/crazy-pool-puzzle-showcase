using DG.Tweening;
using UnityEngine;

namespace ModificatedUISystem.UIElements.Animation.Strategies {
    public class SlideAnimationStrategy : MonoBehaviour, IUIAnimationStrategy
    {
        [SerializeField] private RectTransform _target;
        [SerializeField, Range(0f, 10f)] private float _duration = 0.5f;

        public event AnimationControllerEventHandler AnimationFinished;

        public void Hide()
        {
            _target.DOKill();
            _target.DOAnchorPosY(_target.sizeDelta.y + Screen.height, _duration)
                .OnComplete(() => AnimationFinished?.Invoke(AnimationType.Hide));
        }

        public void Show()
        {
            _target.DOKill();
            _target.anchoredPosition = new Vector2(0, _target.sizeDelta.y + Screen.height);
            _target.DOAnchorPosY(0, _duration)
                .OnComplete(() => AnimationFinished?.Invoke(AnimationType.Show));
        }
    }
}