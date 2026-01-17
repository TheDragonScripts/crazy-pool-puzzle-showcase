using UnityEngine;

namespace SpecialBalls
{
    [RequireComponent(typeof(BallController))]
    public class Simple : MonoBehaviour, ISpecialBall
    {
        public BallController Controller { get; private set; }
        public bool CanBeColoured { get; private set; } = true;

        public bool CanBeMovedByMouse
        {
            get
            {
                return Controller.GameStatus == BallGameStatus.OnBoard &&
                    Controller.Coloring == BallColoring.Coloured &&
                    Controller.Side == BallSide.Red;
            }
        }

        public bool CanCountAsWinnable
        {
            get
            {
                return Controller.Coloring == BallColoring.Coloured;
            }
        }

        private void Start()
        {
            Controller = GetComponent<BallController>();
        }

        public void HandleCollision(Collision collision)
        {
            if (Controller.Coloring == BallColoring.Uncoloured) return;
            BallController ball = collision.gameObject.GetComponent<BallController>();
            if (ball == null) return;
            ball.SetBallColoring(BallColoring.Coloured);
        }
    }
}