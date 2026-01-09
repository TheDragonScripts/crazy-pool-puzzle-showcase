using Cysharp.Threading.Tasks;
using ModificatedUISystem.UIElements.Animation.Strategies;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace ModificatedUISystem.SpecialElements
{
    public class ScrollDownPointer : MonoBehaviour
    {
        [SerializeField] private FlashAnimationStrategy _flashAnimationStrategy;
        [SerializeField] private ScrollRect _scrollRect;
        [Space(5f), Header("Title references")]
        [SerializeField] private TextMeshProUGUI _title;
        [SerializeField] private LocalizeStringEvent _titleLocalizeStringEvent;
        [SerializeField] private RectTransform _titleRectTransform;
        [SerializeField] private float _delayBeforeCanBeHiddenByScrollRect = 0.5f;

        private float _titlePreferredWidth;
        private bool _canHide;

        private void Awake()
        {
            _titleLocalizeStringEvent.OnUpdateString.AddListener(OnTitleStringUpdated);
            _scrollRect?.onValueChanged.AddListener(OnScrollRectValueChanged);
        }

        public void Activate()
        {
            _canHide = false;
            _flashAnimationStrategy.gameObject.SetActive(true);
            _flashAnimationStrategy.Show();
            _ = SetCanHideAsync();
        }

        private void OnScrollRectValueChanged(Vector2 _)
        {
            if (_canHide)
            {
                _flashAnimationStrategy.Hide();
                _flashAnimationStrategy.gameObject.SetActive(false);
            }
        }

        private void OnTitleStringUpdated(string _)
        {
            _titlePreferredWidth = _title.preferredWidth;
            UpdateTextRectSize();
        }

        private void UpdateTextRectSize()
        {
            _titleRectTransform.sizeDelta = new Vector2(_titlePreferredWidth, _titleRectTransform.sizeDelta.y);
        }

        private async UniTaskVoid SetCanHideAsync()
        {
            await UniTask.WaitForSeconds(_delayBeforeCanBeHiddenByScrollRect);
            if (this != null)
            {
                _canHide = true;
            }
        }
    }
}
