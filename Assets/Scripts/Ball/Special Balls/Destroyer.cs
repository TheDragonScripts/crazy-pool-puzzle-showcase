using EntryPoint;
using UnityEngine;

namespace SpecialBalls
{
    [RequireComponent(typeof(DissolvableObject))]
    public class Destroyer : MonoBehaviour, ISpecialBall
    {
        [SerializeField] private BallController _ballController;
        [SerializeField] private BallAppearance _ballAppearance;
        [SerializeField] private DissolvableObject _dissolvableObject;
        [SerializeField] private Rigidbody _rigidBody;
        [SerializeField] private Collider _collider;

        public BallController Controller => _ballController;

        public bool CanBeColoured()
        {
            return false;
        }

        public bool CanBeMovedByMouse()
        {
            return _ballController.GameStatus == BallGameStatus.OnBoard && _ballController.Side == BallSide.Red;
        }

        public bool CanCountAsWinnable()
        {
            return true;
        }

        public void HandleCollision(Collision collision)
        {
            IBarrier barrier = collision.gameObject.GetComponent<IBarrier>();
            if (barrier == null) return;
            GameEntryPoint.Instance.BallsRegistry.RemoveBall(_ballController);
            barrier.DestoryBarrier();
            _ballAppearance.ForceDisableShadows();
            _dissolvableObject.StartDissolvableDestory();
            _rigidBody.isKinematic = true;
            _collider.isTrigger = true;
        }
    }
}