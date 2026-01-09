using Cysharp.Threading.Tasks;
using ModificatedUISystem.UIElements.Animation.Strategies;
using CustomAttributes;
using System;
using UnityEngine;

namespace ModificatedUISystem.UIElements.Animation
{
    [RequireComponent(typeof(Canvas))]
    public class DelayedAnimationController : MonoBehaviour, IUIAnimationController
    {
        [SerializeField, ForceInterface(typeof(IUIAnimationStrategy))]
        private UnityEngine.Object[] _serializableStrategies;

        [SerializeField] private float _delay = 0.2f;

        private Canvas _canvas;
        private IUIAnimationStrategy[] _strategies;
        private bool _isGameObjectDestroyRequired;

        public bool IsTemporaryHidden { get; private set; }
        public bool IsAnimating { get; private set; }

        public event AnimationControllerEventHandler AnimationFinished;

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            int? length = _serializableStrategies?.Length;
            if (length == null)
            {
                throw new NullReferenceException($"{nameof(DelayedAnimationController)} can't find any strategies on {gameObject.name}");
            }
            _strategies = new IUIAnimationStrategy[length.Value];
            for (int i = 0; i < length.Value; i++)
            {
                _strategies[i] = _serializableStrategies[i] as IUIAnimationStrategy;
            }
            _strategies[length.Value - 1].AnimationFinished += LastObjectFinishedAnimation;
        }

        public void Hide()
        {
            IsAnimating = true;
            _isGameObjectDestroyRequired = true;
            for (int i = 0; i < _strategies.Length; i++)
            {
                int index = i;
                _ = DoDelayedAnimation(_delay * index, () => _strategies?[index]?.Hide());
            }
        }

        public void HideTemporary()
        {
            IsAnimating = true;
            IsTemporaryHidden = true;
            for (int i = 0; i < _strategies.Length; i++)
            {
                int index = i;
                _ = DoDelayedAnimation(_delay * index, () => _strategies?[index]?.Hide());
            }
        }

        public void Show()
        {
            IsAnimating = true;
            IsTemporaryHidden = false;
            _canvas.enabled = true;
            for (int i = 0; i < _strategies.Length; i++)
            {
                int index = i;
                _ = DoDelayedAnimation(_delay * index, () => _strategies?[index]?.Show());
            }
        }

        private async UniTaskVoid DoDelayedAnimation(float delay, Action animation)
        {
            await UniTask.WaitForSeconds(delay);
            animation?.Invoke();
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
