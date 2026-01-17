using EntryPoint;
using UnityEngine;

namespace SpecialBalls
{
    [RequireComponent(typeof(DissolvableObject), typeof(BallController), typeof(BallAppearance))]
    [RequireComponent(typeof(Rigidbody), typeof(Collider))]
    public class Destroyer : MonoBehaviour, ISpecialBall
    {
        private BallAppearance _ballAppearance;
        private DissolvableObject _dissolvableObject;
        private Rigidbody _rigidBody;
        private Collider _collider;

        public BallController Controller { get; private set; }
        public bool CanBeColoured { get; private set; } = false;

        public bool CanBeMovedByMouse
        {
            get
            {
                return Controller.GameStatus == BallGameStatus.OnBoard && Controller.Side == BallSide.Red;
            }
        }

        public bool CanCountAsWinnable { get; private set; } = true;

        private void Start()
        {
            Controller = GetComponent<BallController>();
            _ballAppearance = GetComponent<BallAppearance>();
            _dissolvableObject = GetComponent<DissolvableObject>();
            _rigidBody = GetComponent<Rigidbody>();
            _collider = GetComponent<Collider>();
        }

        public void HandleCollision(Collision collision)
        {
            IBarrier barrier = collision.gameObject.GetComponent<IBarrier>();
            if (barrier == null) return;
            GameEntryPoint.Instance.BallsRegistry.RemoveBall(Controller);
            barrier.DestoryBarrier();
            _ballAppearance.ForceDisableShadows();
            _dissolvableObject.StartDissolvableDestory();
            _rigidBody.isKinematic = true;
            _collider.isTrigger = true;
        }
    }
}