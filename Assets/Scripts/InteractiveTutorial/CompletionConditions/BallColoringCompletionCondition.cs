using UnityEngine;

namespace InteractiveTutorial
{
    public class BallColoringCompleitionCondition : ICompletionCondition
    {
        private readonly BallController _ballController;
        private readonly BallColoring _expectedColoring;

        public BallColoringCompleitionCondition(BallController ballController, BallColoring expectedColoring)
        {
            _ballController = ballController;
            _expectedColoring = expectedColoring;
        }

        public bool Check()
        {
            if (_ballController == null)
            {
                Debug.LogWarning("Ball contoller is null");
                return false;
            }
            return _ballController.Coloring == _expectedColoring;
        }
    }
}