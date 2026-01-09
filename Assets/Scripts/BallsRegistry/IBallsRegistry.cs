using System.Collections.Generic;

namespace Balls.Management
{
    public interface IBallsRegistry
    {
        (int colored, int uncolored, int stashed) GetBallsSummary();
        bool IsMoveableBallsAvailable();
        void ClearRegistry();
        void RegisterBall(BallController ball);
        void RemoveBall(BallController ball);
        List<BallController> GetBalls();

    }
}