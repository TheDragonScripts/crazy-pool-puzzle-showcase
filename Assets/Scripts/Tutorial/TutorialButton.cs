using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Tutorial
{
    public class TutorialButton : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _titleText;
        [SerializeField] private LocalizeStringEvent _titleLocalizeEvent;
        [SerializeField] private Image _chevron;
        [SerializeField] private Image _buttonBackground;
        [Header("Settings")]
        [SerializeField, Range(0f, 5f)] private float _animationSpeed = 0.5f;

        private bool _isExpanded;
        
        public bool IsActive
        {
            get { return _button.interactable; }
        }

        public void SetLocalizedTitle(string stringReferenceEntry) => _titleLocalizeEvent.SetEntry(stringReferenceEntry);

        public void Expand()
        {
            if (_isExpanded) return;
            _isExpanded = true;
            _buttonBackground.DOFade(1f, _animationSpeed);
            _titleText.DOFade(1f, _animationSpeed);
            _chevron.DOFade(0f, _animationSpeed);
            _rectTransform.DOSizeDelta(new Vector2(221, _rectTransform.sizeDelta.y), _animationSpeed);
        }

        public void Collapse()
        {
            if (!_isExpanded) return;
            _isExpanded = false;
            _buttonBackground.DOFade(0f, _animationSpeed);
            _titleText.DOFade(0f, _animationSpeed);
            _chevron.DOFade(1f, _animationSpeed);
            _rectTransform.DOSizeDelta(new Vector2(68, _rectTransform.sizeDelta.y), _animationSpeed);
        }

        public void SetActivity(bool activity) => _button.interactable = activity;
    }
}