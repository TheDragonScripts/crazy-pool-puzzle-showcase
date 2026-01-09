using Cysharp.Threading.Tasks;
using DG.Tweening;
using ModificatedUISystem.UIElements.Animation;
using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace ModificatedUISystem.UIElements
{
    public class EnergyDecreaseBox : MonoBehaviour, IUIElement, IUIOfType<MessageBoxType>, IFrequentlyUsedUI, IAutoHidableUI
    {
        [Header("References")]
        [SerializeField] private BaseAnimationController _animationController;
        [SerializeField] private Image _icon;
        [SerializeField] private TextMeshProUGUI _energyCount;
        [SerializeField] private TextMeshProUGUI _decreaseNumber;
        [SerializeField] private RectTransform _decreaseNumberRT;
        [Space(5f)]
        [Header("Settings")]
        [SerializeField, Range(0f, 10f)] private float _showAnimSpeed = 1f;
        [SerializeField, Range(0f, 10f)] private float _hideAnimSpeed = 1f;
        [SerializeField, Range(0f, 10f)] private float _decreaseNumAppearSpeed = 1f;
        [SerializeField, Range(0f, 10f)] private float _animStandTime = 1f;

        private int _currentCapacity;
        private int _currentDecreaser;

        public IUIAnimationController AnimationController => _animationController;
        public bool IsAvailableForAutoHide { get; private set; }

        private void Awake() => HideAll(0f);

        public void Show(int capacity, int decreaser)
        {
            if (AnimationController.IsAnimating) return;
            HideAll(0f);
            _currentCapacity = capacity;
            _currentDecreaser = decreaser;
            _energyCount.text = capacity.ToString();
            _decreaseNumber.text = decreaser.ToString();
            _ = DoCustomAnimation();
        }

        private async UniTaskVoid DoCustomAnimation()
        {
            _decreaseNumberRT.anchoredPosition = new Vector2(0f, _decreaseNumberRT.sizeDelta.y + Screen.height);
            _icon.DOFade(1f, _showAnimSpeed);
            _energyCount.DOFade(1f, _showAnimSpeed);

            await UniTask.WaitForSeconds(_showAnimSpeed);
            _decreaseNumber.DOFade(1f, 0f);
            _decreaseNumberRT.DOAnchorPosY(0, _decreaseNumAppearSpeed);

            await UniTask.WaitForSeconds(_decreaseNumAppearSpeed);
            int localCapacity = _currentCapacity;
            int localDecreaser = _currentDecreaser;
            int targetDecrease = _currentCapacity - Mathf.Abs(_currentDecreaser);
            int oneIterationDecrase = Mathf.Abs(_currentDecreaser) / 5;
            while (localCapacity > targetDecrease)
            {
                localCapacity -= oneIterationDecrase;
                localDecreaser += oneIterationDecrase;
                await UniTask.WaitForSeconds(0.05f);
                _energyCount.text = localCapacity.ToString();
                _decreaseNumber.text = localDecreaser.ToString();
            }
            _energyCount.text = targetDecrease.ToString();
            _decreaseNumber.text = "0";

            await UniTask.WaitForSeconds(_animStandTime);
            HideAll(_hideAnimSpeed);
            IsAvailableForAutoHide = true;
        }

        private void HideAll(float speed)
        {
            _icon.DOFade(0f, speed);
            _energyCount.DOFade(0f, speed);
            _decreaseNumber.DOFade(0f, speed);
        }
    }
}
