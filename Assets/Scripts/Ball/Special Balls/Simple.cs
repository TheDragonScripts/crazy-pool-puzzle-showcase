using UnityEngine;

namespace SpecialBalls
{
    public class Simple : MonoBehaviour, ISpecialBall
    {
        [SerializeField] private BallController _controller;
        public BallController Controller => _controller;

        public bool CanBeColoured()
        {
            return true;
        }

        public bool CanBeMovedByMouse()
        {
            return _controller.GameStatus == BallGameStatus.OnBoard && 
                _controller.Coloring == BallColoring.Coloured &&
                _controller.Side == BallSide.Red;
        }

        public bool CanCountAsWinnable()
        {
            return _controller.Coloring == BallColoring.Coloured;
        }

        public void HandleCollision(Collision collision)
        {
            if (_controller.Coloring == BallColoring.Uncoloured) return;
            BallController ball = collision.gameObject.GetComponent<BallController>();
            if (ball == null) return;
            ball.SetBallColoring(BallColoring.Coloured);
        }
    }
}