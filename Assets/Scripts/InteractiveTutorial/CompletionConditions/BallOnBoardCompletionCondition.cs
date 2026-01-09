using UnityEngine;

namespace InteractiveTutorial
{
    public class BallOnBoardCompletionCondition : ICompletionCondition
    {
        private readonly BallController _ballController;

        public BallOnBoardCompletionCondition(BallController ballController)
        {
            _ballController = ballController;
        }

        public bool Check()
        {
            if (_ballController == null)
            {
                Debug.LogWarning("Ball cotnoller is null");
                return false;
            }
            return _ballController.GameStatus == BallGameStatus.OnBoard;
        }
    }
}