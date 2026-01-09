using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace Tutorial
{
    /*
     * Code here is temporary disabled because it references old UI and Audio system.
     * Pending refactoring.
     */
    [Obsolete("Pending refactoring (UI, Audio)")]
    public class TutorialFrame : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _accompanyingImage;
        [SerializeField] private TextMeshProUGUI _informationText;
        [SerializeField] private LocalizeStringEvent _informationString;
        [SerializeField] private TextMeshProUGUI _pages;
        [SerializeField] private LocalizeStringEvent _buttonTextString;
        [SerializeField] private Image _background;
        [SerializeField] private RectTransform _contentBox;
        [SerializeField] private TutorialButton _nextButton;
        [SerializeField] private TutorialButton _previousButton;
        [Space(5f)]
        [Header("Settings")]
        [SerializeField, Range(0f, 10f)] private float _animationDuration = 1.5f;

        private TutorialTheme _currentTutorialTheme;
        private int _currentTutorialChapter = -1;
        private int _totalPagesCount;
        private int _currentPage;
        private float _savedBackgroundFade;
        private bool _isAnimating;
        private bool _isPagesLoaded;

        public bool IsTutorialActive => _currentTutorialTheme != null;

        public delegate void TutorialFrameDelegate(TutorialTheme tutorial);
        public event TutorialFrameDelegate OnTutorialCompleted;
        public event TutorialFrameDelegate OnTutorialRequested;

        private void Start()
        {
            _savedBackgroundFade = _background.color.a;
            _background.DOFade(0f, 0f);
            _contentBox.anchoredPosition = new Vector2(0, _contentBox.sizeDelta.y + Screen.height);
            _background.raycastTarget = false;
        }

        public void SetTheme(TutorialTheme tutorialTheme)
        {
            if (_currentTutorialTheme != null) return;
            _currentTutorialTheme = tutorialTheme;
            _currentTutorialChapter = -1;
            _currentPage = 1;
            _isPagesLoaded = false;
            _previousButton.SetActivity(false);
            _nextButton.Collapse();
            StartCoroutine(UpdatePagesCount());
            StartCoroutine(SetThemeCoroutine());
            if (OnTutorialRequested != null) OnTutorialRequested(tutorialTheme);
        }

        public void NextButtonClicked()
        {
            if (_currentTutorialTheme == null || _totalPagesCount == 0 || _isAnimating) return;
            /*if (ActualPlayerData.Data != null && ActualPlayerData.Data.MuteUIClick == 0)
                AudioManager.Instance.UIClickSFX.CopyAndPlay(transform.position);*/
            if (_currentPage < _totalPagesCount)
            {
                _currentPage++;
                _pages.text = $"{_currentPage} / {_totalPagesCount}";

                if (_informationText.pageToDisplay < _informationText.textInfo.pageCount)
                    _informationText.pageToDisplay++;
                else
                    NextChapter();

                if (_currentPage > 1)
                    _previousButton.SetActivity(true);

                if (_currentPage == _totalPagesCount)
                {
                    _nextButton.Expand();
                    _nextButton.SetLocalizedTitle("Locale.Close");
                }
            }
            else
            {
                Hide();
                if (OnTutorialCompleted != null) OnTutorialCompleted(_currentTutorialTheme);
                _currentTutorialTheme = null;
            }
        }

        public void PreviousButtonClicked()
        {
            if (_currentTutorialTheme == null || _totalPagesCount == 0 || _isAnimating) return;
            /*if (ActualPlayerData.Data != null && ActualPlayerData.Data.MuteUIClick == 0)
                AudioManager.Instance.UIClickSFX.CopyAndPlay(transform.position);*/
            if (_currentPage > 1)
            {
                _currentPage--;
                _pages.text = $"{_currentPage} / {_totalPagesCount}";

                if (_informationText.pageToDisplay > 1)
                    _informationText.pageToDisplay--;
                else
                {
                    StopCoroutine(PreviousChapter());
                    StartCoroutine(PreviousChapter());
                }

                if (_currentPage < _totalPagesCount)
                    _nextButton.Collapse();

                if (_currentPage == 1)
                    _previousButton.SetActivity(false);
            }
        }

        private IEnumerator SetThemeCoroutine()
        {
            yield return new WaitUntil(() => _isPagesLoaded);
            Show();
            NextChapter();
            _pages.text = $"{_informationText.pageToDisplay} / {_totalPagesCount}";
        }

        private void NextChapter()
        {
            _currentTutorialChapter++;
            _informationString.StringReference.SetReference("Strings",
                _currentTutorialTheme.Chapters[_currentTutorialChapter].Information.TableEntryReference);
            _informationText.pageToDisplay = 1;

            _accompanyingImage.sprite = _currentTutorialTheme.Chapters[_currentTutorialChapter].AccompanyingImage;
        }

        private IEnumerator PreviousChapter()
        {
            _currentTutorialChapter--;
            _informationString.StringReference.SetReference("Strings",
                _currentTutorialTheme.Chapters[_currentTutorialChapter].Information.TableEntryReference);
            _accompanyingImage.sprite = _currentTutorialTheme.Chapters[_currentTutorialChapter].AccompanyingImage;
            yield return new WaitForSeconds(0.025f);
            _informationText.pageToDisplay = _informationText.textInfo.pageCount;

        }

        private IEnumerator UpdatePagesCount()
        {
            _totalPagesCount = 0;
            foreach (TutorialChapter chapter in _currentTutorialTheme.Chapters)
            {
                yield return new WaitForSeconds(0.025f);
                _informationString.StringReference.SetReference("Strings", chapter.Information.TableEntryReference);
                yield return new WaitForSeconds(0.025f);
                _totalPagesCount += _informationText.textInfo.pageCount;

            }
            _isPagesLoaded = true;
        }

        /*private void OnContentFinishedAnimation(UIAnimations animType)
        {
            _isAnimating = false;
            if (animType == UIAnimations.Hide)
                _background.raycastTarget = false;
        }*/

        private void Show()
        {
            _background.DOFade(0f, 0f);
            _contentBox.DOAnchorPosY(_contentBox.sizeDelta.y + Screen.height, 0f);

            _isAnimating = true;
            _background.raycastTarget = true;
            _background.DOFade(_savedBackgroundFade, _animationDuration);
            /*_contentBox.DOAnchorPosY(0, _animationDuration).
                OnComplete(() => OnContentFinishedAnimation(UIAnimations.Show));*/
        }

        private void Hide()
        {
            _isAnimating = true;
            _background.DOFade(0f, _animationDuration);
            /*_contentBox.DOAnchorPosY(-_contentBox.sizeDelta.y - Screen.height, _animationDuration).
                OnComplete(() => OnContentFinishedAnimation(UIAnimations.Hide));*/
        }
    }
}
