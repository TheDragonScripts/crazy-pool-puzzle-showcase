using UnityEngine;

namespace SpecialBalls
{
    [RequireComponent(typeof(BallController))]
    public class Eight : MonoBehaviour, ISpecialBall
    {
        public BallController Controller { get; private set; }
        public bool CanBeColoured { get; private set; } = true;
        public bool CanBeMovedByMouse { get; private set; } = false;
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

        public void HandleCollision(Collision collision) { }
    }
}