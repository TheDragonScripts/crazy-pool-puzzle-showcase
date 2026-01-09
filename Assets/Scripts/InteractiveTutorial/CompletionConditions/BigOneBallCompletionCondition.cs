using UnityEngine;

namespace InteractiveTutorial
{
    public class BigOneBallCompletionCondition : ICompletionCondition
    {
        private readonly Stash _stash;
        private readonly BallController _ballController;

        public BigOneBallCompletionCondition(Stash stash, BallController ballController)
        {
            _stash = stash;
            _ballController = ballController;
        }

        public bool Check()
        {
            if (_stash == null || _ballController == null)
            {
                Debug.LogWarning("Stash or ball controller are null");
                return false;
            }
            return _stash.CurrentBigOneBall == _ballController;
        }
    }
}