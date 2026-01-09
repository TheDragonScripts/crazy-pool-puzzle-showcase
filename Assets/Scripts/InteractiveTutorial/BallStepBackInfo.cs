using UnityEngine;

namespace InteractiveTutorial
{
    public class BallStepBackInfo
    {
        public BallController Ball;
        public Vector3 Position;
        public BallColoring Coloring;
        public BallGameStatus Status;
        public BallSide Side;

        public BallStepBackInfo(BallController ball, Vector3 position, BallColoring coloring,
            BallGameStatus status, BallSide side)
        {
            Ball = ball;
            Position = position;
            Coloring = coloring;
            Status = status;
            Side = side;
        }
    }
}