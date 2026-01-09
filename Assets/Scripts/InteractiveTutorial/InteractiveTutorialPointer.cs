using DG.Tweening;
using EntryPoint;
using EntryPoint.Levels;
using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace InteractiveTutorial {
    public class InteractiveTutorialPointer : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _opaque;
        [SerializeField] private Image _transparent;
        [Space(5f)]
        [Header("Settings")]
        [SerializeField] private float _animationSpeed = 1f;

        private PointerAnimationCycle _cycle;
        private GameObject _targetObject;
        private ILevelManager _levelManager;

        public enum PointerAnimationCycle { Idle, Click, Swipe }

        private void Awake() => StartCoroutine(Initialize());

        private void Update()
        {
            if (_targetObject != null && Camera.main != null)
                transform.position = Camera.main.WorldToScreenPoint(_targetObject.transform.position);
        }

        public void HidePointer()
        {
            _cycle = PointerAnimationCycle.Idle;
            _targetObject = null;
            StopAllCoroutines();
            HandleVisibilty(false, _animationSpeed);
        }

        public void CycleSwipe(Vector3 from, Vector3 to)
        {
            HandleVisibilty(true, _animationSpeed);
            StopAllCoroutines();
            StartCoroutine(CycleSwipeAnimation(from, to));
        }

        public void CycleSwipe(GameObject fromObject, GameObject toObject)
        {
            HandleVisibilty(true, _animationSpeed);
            StopAllCoroutines();
            StartCoroutine(CycleSwipeAnimationTargeted(fromObject, toObject));
        }

        public void CycleClick(Vector3 pos)
        {
            HandleVisibilty(true, _animationSpeed);
            StopAllCoroutines();
            StartCoroutine(CycleClickAnim(pos));
        }

        public void CycleClick(GameObject targetObject)
        {
            _targetObject = targetObject;
            CycleClick(Camera.main.WorldToScreenPoint(targetObject.transform.position));
        }

        private IEnumerator CycleSwipeAnimation(Vector3 from, Vector3 to)
        {
            _cycle = PointerAnimationCycle.Swipe;
            _transparent.transform.DOScale(1f, 0f);
            transform.position = from;

            while (_cycle == PointerAnimationCycle.Swipe)
            {
                yield return new WaitForSeconds(1f);
                _transparent.transform.DOScale(1.5f, 1f);
                yield return new WaitForSeconds(1f);
                transform.DOMove(to, 1f);
                yield return new WaitForSeconds(1f);
                _transparent.transform.DOScale(1f, 1f);
            }
        }

        private IEnumerator CycleSwipeAnimationTargeted(GameObject from, GameObject to)
        {
            _cycle = PointerAnimationCycle.Swipe;
            _transparent.transform.DOScale(1f, 0f);
            transform.position = Camera.main.WorldToScreenPoint(from.transform.position);

            while (_cycle == PointerAnimationCycle.Swipe)
            {
                yield return new WaitForSeconds(1f);
                _transparent.transform.DOScale(1.5f, 1f);
                yield return new WaitForSeconds(1f);
                if (Camera.main == null || to == null || to.transform == null) yield break;
                transform.DOMove(Camera.main.WorldToScreenPoint(to.transform.position), 1f);
                yield return new WaitForSeconds(1f);
                _transparent.transform.DOScale(1f, 1f);
                yield return new WaitForSeconds(1f);
                if (Camera.main == null || from == null || from.transform == null) yield break;
                transform.position = Camera.main.WorldToScreenPoint(from.transform.position);
            }
        }

        private IEnumerator CycleClickAnim(Vector3 pos)
        {
            _cycle = PointerAnimationCycle.Click;
            _transparent.transform.DOScale(1f, 0f);
            transform.position = pos;

            while (_cycle == PointerAnimationCycle.Click)
            {
                yield return new WaitForSeconds(1f);
                _transparent.transform.DOScale(1.5f, 1f);
                yield return new WaitForSeconds(1f);
                _transparent.transform.DOScale(1f, 1f);
            }
        }

        private IEnumerator Initialize()
        {
            HandleVisibilty(false, 0f);
            yield return GameEntryPoint.Instance != null;
            yield return GameEntryPoint.Instance.LevelManager != null;
            _levelManager = GameEntryPoint.Instance.LevelManager;
            _levelManager.LevelLoaded += HideWhenNotPlayableLevel;
        }

        private void HideWhenNotPlayableLevel(int level)
        {
            _opaque.DOKill();
            _transparent.DOKill();
            HandleVisibilty(false, 0f);
        }

        private void HandleVisibilty(bool visibility, float speed)
        {
            float visibiltyValue = Convert.ToSingle(visibility);
            _opaque.DOFade(visibiltyValue, speed);
            _transparent.DOFade(visibiltyValue, speed);
        }
    }
}