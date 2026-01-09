using System;
using UnityEngine;

namespace SpecialBalls
{
    public interface ISpecialBall
    {
        BallController Controller { get; }
        void HandleCollision(Collision collision);
        bool CanBeMovedByMouse();
        bool CanCountAsWinnable();
        bool CanBeColoured();
    }
}