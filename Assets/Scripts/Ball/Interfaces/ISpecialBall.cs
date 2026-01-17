using System;
using UnityEngine;

namespace SpecialBalls
{
    public interface ISpecialBall
    {
        BallController Controller { get; }
        bool CanBeMovedByMouse { get; }
        bool CanCountAsWinnable { get; }
        bool CanBeColoured { get; }
        void HandleCollision(Collision collision);
    }
}