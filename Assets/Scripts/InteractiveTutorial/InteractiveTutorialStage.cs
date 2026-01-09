using System;
using System.Collections.Generic;
using UnityEngine;

namespace InteractiveTutorial
{
    /// <summary>
    /// Interactive tutorial stage class that contains actions and completion condition to switch to the next stage. 
    /// </summary>
    public class InteractiveTutorialStage : IInteractiveTutorialStage
    {
        protected List<IInteractiveTutorialAction> _actions;
        protected ICompletionCondition _completionCondition;
        protected IStepBackCondition _stepBackCondition;
        private bool _isSealed;

        public bool IsSealed => _isSealed;
        /// <summary>
        /// Returns stage's actions list.
        /// </summary>
        /// <remarks>Call Seal() method before access this property.</remarks>
        /// <exception cref="InvalidOperationException">
        /// Throws when trying to access propery before stage sealing.
        /// </exception>
        public List<IInteractiveTutorialAction> Actions
        {
            get
            {
                if (!_isSealed) throw new InvalidOperationException("Stage must be sealed before access.");
                return _actions;
            }
        }
        /// <summary>
        /// Returns completion condition.
        /// </summary>
        /// <remarks>Call Seal() method before access this property.</remarks>
        /// <exception cref="InvalidOperationException">
        /// Throws when trying to access propery before stage sealing.
        /// </exception>
        public ICompletionCondition Condition
        {
            get
            {
                if (!_isSealed) throw new InvalidOperationException("Stage must be sealed before access.");
                return _completionCondition;
            }
        }

        public IStepBackCondition StepBackCondition
        {
            get
            {
                if (!_isSealed) throw new InvalidOperationException("Stage must be sealed before access.");
                return _stepBackCondition;
            }
        }

        public InteractiveTutorialStage()
        {
            _actions = new List<IInteractiveTutorialAction>();
        }

        public virtual IInteractiveTutorialStage InsertActions(params IInteractiveTutorialAction[] actions)
        {
            if (_isSealed)
                throw new InvalidOperationException("Stage is sealed and cannot be edited anymore");
            if (actions == null)
                throw new ArgumentNullException(nameof(actions) + "cannot be null");
            foreach (IInteractiveTutorialAction action in actions)
                InsertAction(action);
            return this;
        }

        public virtual IInteractiveTutorialStage InsertAction(IInteractiveTutorialAction action)
        {
            if (_isSealed)
                throw new InvalidOperationException("Stage is sealed and cannot be edited anymore");
            if (action == null)
                throw new ArgumentNullException(nameof(action), "Action cannot be null");
            if (_actions.Contains(action))
            {
                Debug.LogWarning("Be advised, action already exists in the list");
                return this;
            }
            _actions.Add(action);
            return this;
        }

        public virtual IInteractiveTutorialStage InsertStepBackCondition(IStepBackCondition stepBackCondition)
        {
            if (_isSealed)
                throw new InvalidOperationException("Stage is sealed and cannot be edited anymore");
            if (stepBackCondition == null)
                throw new ArgumentNullException(nameof(stepBackCondition), "Step back condition cannot be null");
            _stepBackCondition = stepBackCondition;
            return this;
        }

        public virtual IInteractiveTutorialStage InsertCompletionCondition(ICompletionCondition completionCondition)
        {
            if (_isSealed)
                throw new InvalidOperationException("Stage is sealed and cannot be edited anymore");
            if (completionCondition == null)
                throw new ArgumentNullException(nameof(completionCondition), "Completion condition cannot be null");
            _completionCondition = completionCondition;
            return this;
        }
        /// <summary>
        /// Seals this stage. After sealing completion condition and actions cannot be edited anymore.
        /// </summary>
        /// <remarks>
        /// Call this method only after adding completion condition and at least one action.
        /// Otherwise it will throw an exception.
        /// </remarks>
        /// <returns>Itself to create a convenient call chain. (Fluent interface).</returns>
        /// <exception cref="InvalidOperationException">
        /// Throws when completion condition is null or actions count is 0.
        /// </exception>
        public IInteractiveTutorialStage Seal()
        {
            if (_completionCondition == null || _actions.Count == 0)
                throw new InvalidOperationException($"Stage cannot be sealed. Missing: " +
                    $"{(_completionCondition == null ? "completion condition" : "")}" +
                    $"{(_actions.Count == 0 ? ", actions" : "")}");
            _isSealed = true;
            return this;
        }
    }
}