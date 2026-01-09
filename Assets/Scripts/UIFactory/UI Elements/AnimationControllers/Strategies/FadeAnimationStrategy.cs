using DG.Tweening;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModificatedUISystem.UIElements.Animation.Strategies
{
    public class FadeAnimationStrategy : MonoBehaviour, IUIAnimationStrategy
    {
        [SerializeField] private Image[] _images;
        [SerializeField] private TextMeshProUGUI[] _texts;

        [SerializeField, Range(0f, 10f)]
        private float _duration = 0.5f;

        [SerializeField, Range(0f, 1f), Tooltip("The minimum possible fade value for targets.")]
        private float _minimumFadeValue = 0f;

        private Dictionary<ILayoutElement, float> _fadeTargets = new Dictionary<ILayoutElement, float>();

        public event AnimationControllerEventHandler AnimationFinished;

        private void Awake()
        {
            _images ??= new Image[0];
            _texts ??= new TextMeshProUGUI[0];
            foreach (var image in _images)
            {
                _fadeTargets.Add(image, image.color.a);
            }
            foreach (var text in _texts)
            {
                _fadeTargets.Add(text, text.color.a);
            }
            DoFade(0, AnimationType.Hide, false);
        }

        public void Hide()
        {
            KillAllTweens();
            DoFade(_duration, AnimationType.Hide);
        }

        public void Show()
        {
            KillAllTweens();
            DoFade(0, AnimationType.Hide, false);
            DoFade(_duration, AnimationType.Show);
        }

        private void KillAllTweens()
        {
            foreach (var element in _fadeTargets)
            {
                if (element.Key is Image image)
                {
                    image.DOKill();
                }
                else if (element.Key is TextMeshProUGUI text)
                {
                    text.DOKill();
                }
            }
        }

        private void DoFade(float duration, AnimationType animationType, bool invokeEvent = true)
        {
            ILayoutElement last = _fadeTargets.Last().Key;
            Action<ILayoutElement, AnimationType> animationCompletionCallback = (target, animationType) =>
            {
                if (target == last && invokeEvent)
                {
                    AnimationFinished?.Invoke(animationType);
                }
            };
            foreach (var target in _fadeTargets)
            {
                float to = animationType switch
                {
                    AnimationType.Show => target.Value,
                    AnimationType.Hide => _minimumFadeValue,
                    _ => _minimumFadeValue
                };

                if (target.Key is Image image)
                {
                    image.DOFade(to, duration)
                         .OnComplete(() => animationCompletionCallback(target.Key, animationType));
                }
                else if (target.Key is TextMeshProUGUI text)
                {
                    text.DOFade(to, duration)
                        .OnComplete(() => animationCompletionCallback(target.Key, animationType));
                }
            }
        }
    }
}
