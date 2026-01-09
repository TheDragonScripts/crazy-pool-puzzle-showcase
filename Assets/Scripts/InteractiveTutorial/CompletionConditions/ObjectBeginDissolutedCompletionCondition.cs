using UnityEngine;

namespace InteractiveTutorial
{
    public class ObjectBeginDissolutedCompletionCondition : ICompletionCondition
    {
        private readonly DissolutionObserver _observer;

        public ObjectBeginDissolutedCompletionCondition(DissolutionObserver observer)
        {
            _observer = observer;
        }

        public bool Check()
        {
            if (_observer == null)
            {
                Debug.LogWarning("Be adivsed. Dissoultion observer is null");
                return false;
            }
            return _observer.IsDissolutionInProcess;
        }
    }
}