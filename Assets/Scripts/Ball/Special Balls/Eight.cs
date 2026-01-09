using UnityEngine;

namespace SpecialBalls
{
    public class Eight : MonoBehaviour, ISpecialBall
    {
        [SerializeField] private BallController _controller;
        public BallController Controller => _controller;

        public bool CanBeColoured()
        {
            return true;
        }

        public bool CanBeMovedByMouse()
        {
            return false;
        }

        public bool CanCountAsWinnable()
        {
            return _controller.Coloring == BallColoring.Coloured;
        }

        public void HandleCollision(Collision collision) { }
    }
}