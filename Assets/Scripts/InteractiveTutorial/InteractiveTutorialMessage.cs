using DG.Tweening;
using EntryPoint;
using EntryPoint.Levels;
using EnumerableAnchors;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;

namespace InteractiveTutorial
{
    public class InteractiveTutorialMessage : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private TextMeshProUGUI _message;
        [SerializeField] private RectTransform _rectTransform;
        [SerializeField] private LocalizeStringEvent _localizationEvent;
        [SerializeField] private RectTransform _copiesContainerReference;
        [Space(5f)]
        [Header("Settings")]
        [SerializeField] private float _animationSpeed = 1f;
        [SerializeField] private bool _isReference;

        private static RectTransform _copiesContainer;
        private static StringTable _stringTable;
        private ILevelManager _levelManager;
        private bool _firstInitCompleted;

        public bool IsReference => _isReference;

        private void Awake() =>
            StartCoroutine(Initialize());

        private void OnDisable() =>
            _levelManager.LevelLoaded -= HideWhenNotPlayablelevel;

        private void OnDestroy() =>
            _levelManager.LevelLoaded -= HideWhenNotPlayablelevel;

        public InteractiveTutorialMessage CreateCopy()
        {
            GameObject copy = Instantiate(gameObject, _copiesContainer);
            InteractiveTutorialMessage message = copy.GetComponent<InteractiveTutorialMessage>();
            message._isReference = false;
            return message;
        }

        public InteractiveTutorialMessage CreateTemporaryCopy(float lifeTime)
        {
            GameObject copy = Instantiate(gameObject, _copiesContainer);
            InteractiveTutorialMessage message = copy.GetComponent<InteractiveTutorialMessage>();
            message._isReference = false;
            TemporaryMessage tempmsg = message.gameObject.AddComponent<TemporaryMessage>();
            tempmsg.SetLifetime(lifeTime).StartDestroying();
            return message;
        }

        public void Hide()
        {
            if (_isReference)
            {
                Debug.LogWarning("You're trying to use reference object. Use CreateCopy() method.");
                return;
            }
            HandleVisiblity(false, _animationSpeed);
        }

        public void Show(EnumerableAnchor anchor, string stringReferenceEntry)
        {
            if (_isReference)
            {
                Debug.LogWarning("You're trying to use reference object. Use CreateCopy() method.");
                return;
            }
            EnumerableAnchorsUtility.SetEnumerableAnchor(anchor, ref _rectTransform);
            SetupMessage(stringReferenceEntry);
            HandleVisiblity(true, _animationSpeed);
        }

        public void Show(EnumerableAnchor anchor, Vector2 customOffset, string stringReferenceEntry)
        {
            if (_isReference)
            {
                Debug.LogWarning("You're trying to use reference object. Use CreateCopy() method.");
                return;
            }
            EnumerableAnchorsUtility.SetEnumerableAnchor(anchor, customOffset, ref _rectTransform);
            SetupMessage(stringReferenceEntry);
            HandleVisiblity(true, _animationSpeed);
        }

        public void Show(Vector3 worldPos, string stringReferenceEntry)
        {
            if (_isReference)
            {
                Debug.LogWarning("You're trying to use reference object. Use CreateCopy() method.");
                return;
            }
            EnumerableAnchorsUtility.SetEnumerableAnchor(EnumerableAnchor.MiddleCenter, Vector2.zero, ref _rectTransform);
            Vector3 pos = Camera.main.WorldToScreenPoint(worldPos);
            transform.position = pos;
            SetupMessage(stringReferenceEntry);
            HandleVisiblity(true, _animationSpeed);
        }

        public void Show(GameObject target, string stringReferenceEntry)
        {
            if (_isReference)
            {
                Debug.LogWarning("You're trying to use reference object. Use CreateCopy() method.");
                return;
            }
            EnumerableAnchorsUtility.SetEnumerableAnchor(EnumerableAnchor.MiddleCenter, Vector2.zero, ref _rectTransform);
            Vector3 pos = Camera.main.WorldToScreenPoint(target.transform.position);
            transform.position = pos;
            SetupMessage(stringReferenceEntry);
            HandleVisiblity(true, _animationSpeed);
        }

        private void HandleVisiblity(bool isVisible, float speed)
        {
            if (this == null || gameObject == null || _message == null) return;
            float visibiltyValue = Convert.ToSingle(isVisible);
            _message.DOFade(visibiltyValue, speed)
                .OnComplete(() =>
                {
                    if (this == null || gameObject == null) return;
                    if (!_isReference && !isVisible && _firstInitCompleted)
                        Destroy(gameObject);
                    else if (!_firstInitCompleted)
                        _firstInitCompleted = true;
                });
        }

        private void SetupMessage(string stringReference)
        {
            if (_stringTable == null)
                throw new NullReferenceException("String database is not initialzied yet");

            if (_stringTable.GetEntry(stringReference) != null)
                _localizationEvent.SetEntry(stringReference);
            else
                _message.text = stringReference;
        }

        private IEnumerator Initialize()
        {
            HandleVisiblity(false, 0f);
            yield return LocalizationSettings.StringDatabase.DefaultTable != null && GameEntryPoint.Instance != null;
            yield return GameEntryPoint.Instance.LevelManager != null;
            if (_isReference)
            {
                _copiesContainer = _copiesContainerReference;
                _stringTable = LocalizationSettings.StringDatabase.GetTable(LocalizationSettings.StringDatabase.DefaultTable.TableCollectionName);
            }
            _levelManager = GameEntryPoint.Instance.LevelManager;
            _levelManager.LevelLoaded += HideWhenNotPlayablelevel;
        }

        private void HideWhenNotPlayablelevel(int level)
        {
            HandleVisiblity(false, 0f);
        }
    }
}