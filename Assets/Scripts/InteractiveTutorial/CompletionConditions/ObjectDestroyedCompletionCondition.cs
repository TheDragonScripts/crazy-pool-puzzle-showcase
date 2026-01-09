using UnityEngine;

namespace InteractiveTutorial
{
    public class ObjectDestroyedCompletionCondition : ICompletionCondition
    {
        private GameObject _target;

        public ObjectDestroyedCompletionCondition(GameObject target)
        {
            _target = target;
        }

        public bool Check()
        {
            return _target == null;
        }
    }
}