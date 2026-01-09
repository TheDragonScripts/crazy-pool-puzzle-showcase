using BallsMovement;
using EntryPoint;
using PlayerInputs.Swipes;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveTutorial
{
    /// <summary>
    /// Interactive tutorial object that controlling tutorial on current level.
    /// </summary>
    /// <remarks>
    /// Must be inherited from <see cref="MonoBehaviour"/ and implements interface <see cref="IInteractiveTutorial"/>>.
    /// </remarks>
    public class InteractiveTutorial : MonoBehaviour, IInteractiveTutorial
    {
        private List<IInteractiveTutorialStage> _stages;
        private int _currentStage;
        private bool _isSealed;
        private ISwipeController _swipeController;
        private IBallsForceApplication _ballsForceApplication;
        private bool _isStarted;
        private Coroutine _stepBackCoroutine;
        private bool _canDoStepBack = true;

        private const float TRACK_TIME = 0.5f;
        private const float USER_AFK_TIME = 5f;
        private const float FALLBACK_CHECK_TIME = 1f;
        private const float STEP_BACK_CHECK_TIME = 5f;

        public int CurrentStage => _currentStage;

        public bool IsSealed => _isSealed;

        /// <summary>
        /// Returns stages of the interactive tutorial.
        /// </summary>
        /// <remarks>
        /// Call Seal() method before access this property.
        /// </remarks>
        /// <exception cref="InvalidOperationException">
        /// Throws when tutorial is not sealed.
        /// </exception>
        public List<IInteractiveTutorialStage> Stages
        {
            get
            {
                if (!_isSealed)
                    throw new InvalidOperationException($"Seal tutorial before access {nameof(Stages)} property");
                return _stages;
            }
        }

        private void Awake() => StartCoroutine(InitializeASAP());

        private void OnDisable()
        {
            StopAllCoroutines();
            SubscribeToSwipeController(false);
        }

        private void OnDestroy()
        {
            StopAllCoroutines();
            SubscribeToSwipeController(false);
        }

        public IInteractiveTutorial InsertStages(params IInteractiveTutorialStage[] stages)
        {
            if (_isSealed)
                throw new InvalidOperationException("Tutorial is sealed and cannot be edited anymore");
            if (stages == null)
                throw new ArgumentNullException($"{nameof(stages)} cannot be null");
            foreach (var stage in stages)
                InsertStage(stage);
            return this;
        }

        public IInteractiveTutorial InsertStage(IInteractiveTutorialStage stage)
        {
            if (_isSealed)
                throw new InvalidOperationException($"Tutorial already sealed. New stage cannon be added");
            if (stage == null)
                throw new ArgumentNullException($"Stage {nameof(stage)} cannot be null");
            if (_stages.Contains(stage))
                Debug.LogWarning("Stage is already exists in the list");
            if (!stage.IsSealed)
                throw new InvalidOperationException("Stage is not sealed and cannot be added to the list");
            _stages.Add(stage);
            return this;
        }

        /// <summary>
        /// Seals current tutorial object.
        /// </summary>
        /// <remarks>
        /// Be sure that stages list have at least one stage in it.
        /// </remarks>
        /// <returns>Itself to create a convenient call chain. (Fluent interface)</returns>
        /// <exception cref="InvalidOperationException">
        /// Throws when stages list count is 0;
        /// </exception>
        public IInteractiveTutorial Seal()
        {
            if (_stages.Count == 0)
                throw new InvalidOperationException("Tutorial cannot be sealed, because there is no stages added to it.");
            _isSealed = true;
            return this;
        }
        /// <summary>
        /// Starts current tutorial. 
        /// </summary>
        /// <remarks>
        /// Seal current tutorial and all stages in list before call this method.
        /// </remarks>
        /// <returns>Itself to create a convenient call chain. (Fluent interface)</returns>
        /// <exception cref="InvalidOperationException">
        /// Throws when tutorial or any stages in list is not sealed.
        /// </exception>
        public IInteractiveTutorial StartTutorial()
        {
            if (_isStarted)
                return this;
            if (!_isSealed)
                throw new InvalidOperationException("Seal tutorial before calling start method");
            if (!IsAllStagesSealed())
                throw new InvalidOperationException("Some stages in tutorial is unselaed. Seal it all before calling start method");
            if (ActualPlayerData.Data == null)
            {
                Debug.LogWarning("Actual player data is not initalized yet. Skipping interactive tutorial.");
                return this;
            }
            if (ActualPlayerData.Data.RandomBallsInStash == 1)
            {
                Debug.LogWarning("Random balls is active. Skipping interactive tutorial.");
                return this;
            }
            _isStarted = true;
            ShowCurrentTutorialStage();
            return this;
        }

        private bool IsAllStagesSealed()
        {
            bool isAllSealed = true;
            foreach (IInteractiveTutorialStage stage in _stages)
            {
                if (!stage.IsSealed)
                {
                    isAllSealed = false;
                    break;
                }
            }
            return isAllSealed;
        }

        /// <summary>
        /// Initializes stages list and connecting to events from <see cref="SwipeController"/>
        /// </summary>
        /// <remarks>Coroutine method.</remarks>
        private IEnumerator InitializeASAP()
        {
            _stages = new List<IInteractiveTutorialStage>();
            while (_swipeController == null)
            {
                yield return new WaitForSeconds(TRACK_TIME);
                _swipeController = GameEntryPoint.Instance.SwipeController;
                _ballsForceApplication = GameEntryPoint.Instance.BallsForceApplication;
            }
            SubscribeToSwipeController(true);
        }

        private void SubscribeToSwipeController(bool subscribe)
        {
            if (_swipeController == null) return;

            if (subscribe)
            {
                _swipeController.SwipeDown += HideTutorialByUserActivity;
                _swipeController.SwipeUp += HideTutorialByUserActivity;
                _swipeController.SwipeLeft += HideTutorialByUserActivity;
                _swipeController.SwipeRight += HideTutorialByUserActivity;
                _ballsForceApplication.ForceApplied += StopStepBackCoroutineAndStartItAgain;
            }
            else
            {
                _swipeController.SwipeDown -= HideTutorialByUserActivity;
                _swipeController.SwipeUp -= HideTutorialByUserActivity;
                _swipeController.SwipeLeft -= HideTutorialByUserActivity;
                _swipeController.SwipeRight -= HideTutorialByUserActivity;
                _ballsForceApplication.ForceApplied -= StopStepBackCoroutineAndStartItAgain;
            }
        }

        private void HideTutorialByUserActivity()
        {
            if (!_isStarted) return;
            ShowOrHideActions(false);
            StopCoroutine(ShowTutorialAgainAfterAFK());
            StartCoroutine(ShowTutorialAgainAfterAFK());
        }

        private void ShowOrHideActions(bool showOrHide)
        {
            if (!_isStarted) return;
            if (_currentStage >= _stages.Count) return;
            IInteractiveTutorialStage stage = _stages[_currentStage];
            if (stage?.Actions == null) return;
            foreach (IInteractiveTutorialAction action in stage.Actions)
            {
                if (action == null) continue;
                if (showOrHide)
                    action.Do();
                else
                    action.Undo();
            }
        } 

        private void ShowCurrentTutorialStage()
        {
            if (!_isStarted) return;
            if (_currentStage >= _stages.Count || _currentStage < 0) return;
            ShowOrHideActions(true);
            StopTutorialCoroutines();
            StartCoroutine(TrackForCompletionCondition());
            StartCoroutine(CallActionsFallbacks());
            StartCoroutine(CheckStepBackConditions());
        }

        private void StopTutorialCoroutines()
        {
            StopCoroutine(ShowTutorialAgainAfterAFK());
            StopCoroutine(TrackForCompletionCondition());
            StopCoroutine(CallActionsFallbacks());
            StopCoroutine(CheckStepBackConditions());
            StopCoroutine(MakeStepbackAvailable());
        }

        private void StopStepBackCoroutineAndStartItAgain()
        {
            if (this == null || _stages == null || _currentStage >= _stages.Count || _stages[_currentStage] == null
                || _stages[_currentStage].Condition == null)
                return;

            if (_stepBackCoroutine != null)
                StopCoroutine(_stepBackCoroutine);

            _canDoStepBack = false;
            StartCoroutine(MakeStepbackAvailable());
        }

        private IEnumerator MakeStepbackAvailable()
        {
            yield return new WaitForSeconds(STEP_BACK_CHECK_TIME);
            _canDoStepBack = true;
        }

        private IEnumerator ShowTutorialAgainAfterAFK()
        {
            if (!_isStarted) yield break;
            yield return new WaitForSeconds(USER_AFK_TIME);
            if (this == null || _currentStage >= _stages.Count || _stages?[_currentStage] == null)
                yield break;
            ShowOrHideActions(true);
        }

        private IEnumerator CheckStepBackConditions()
        {
            if (!_isStarted || _stages[_currentStage].StepBackCondition == null) yield break;
            _stages[_currentStage].StepBackCondition.SetupStepBackInfo();
            bool conditionResult;
            while (true)
            {
                yield return new WaitForSeconds(STEP_BACK_CHECK_TIME);
                if (this == null || _stages == null || _currentStage >= _stages.Count || _stages[_currentStage] == null
                    || _stages[_currentStage].Condition == null)
                    yield break;
                yield return new WaitUntil(() => _canDoStepBack == true);
                if (this == null || _stages == null || _currentStage >= _stages.Count || _stages[_currentStage] == null
                    || _stages[_currentStage].Condition == null)
                    yield break;
                conditionResult = _stages[_currentStage].StepBackCondition.Check();
                if (conditionResult)
                {
                    HideTutorialByUserActivity();
                    _stepBackCoroutine = StartCoroutine(_stages[_currentStage].StepBackCondition.StepBack());
                }
            }
        }

        private IEnumerator CallActionsFallbacks()
        {
            if (!_isStarted) yield break;
            while (_currentStage < _stages.Count)
            {
                yield return new WaitForSeconds(FALLBACK_CHECK_TIME);
                if (_stages == null || _currentStage >= _stages.Count) yield break;
                foreach (IInteractiveTutorialAction action in _stages[_currentStage].Actions)
                {
                    if (action is IFallbackableAction fallbackableAction)
                        fallbackableAction.CheckForFallback();
                }
            }
        }

        /// <summary>
        /// Tracking for completion condition of the current tutorial every <see cref="TrackTime"/>. When
        /// condition is completed switches to the next stage.
        /// </summary>
        /// <remarks>Coroutine method.</remarks>
        private IEnumerator TrackForCompletionCondition()
        {
            if (!_isStarted) yield break;
            bool conditionResult = _stages[_currentStage].Condition.Check();
            while (!conditionResult)
            {
                yield return new WaitForSeconds(TRACK_TIME);
                if (this == null || _stages == null || _stages[_currentStage] == null || _stages[_currentStage].Condition == null)
                    yield break;
                conditionResult = _stages[_currentStage].Condition.Check();
            }
            ShowOrHideActions(false);
            _currentStage++;
            if (_currentStage >= _stages.Count)
                yield break;
            ShowCurrentTutorialStage();
        }
    }
}