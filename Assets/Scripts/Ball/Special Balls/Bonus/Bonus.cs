using DG.Tweening;
using System;
using UnityEngine;
using WeightedRandomization;

namespace SpecialBalls
{
    [RequireComponent(typeof(Rigidbody))]
    /*
     * Code here is temporary disabled because it references old Audio system.
     * Pending refactoring.
     */
    public class Bonus : MonoBehaviour, ISpecialBall
    {
        [SerializeField] private BallController _controller;
        [SerializeField] private int _bonusAmount = 2;
        [SerializeField, Range(0f, 5f)] private float _jumpSpeed = 0.1f;
        [SerializeField, Range(0f, 5f)] private float _scaleSpeed = 0.2f;
        [SerializeField, Range(0f, 5f)] private float _newBallMoveSpeed = 0.3f;
        [SerializeField] private GameObject _particles;
        [SerializeField] private WeightedItem<GameObject>[] _possibleBonusBalls;

        private Rigidbody _rb;
        private Stash _stash;
        private bool _isTriggered;

        public BallController Controller => _controller;

        private void Start()
        {
            _rb = GetComponent<Rigidbody>();
            _stash = FindFirstObjectByType<Stash>();

            if (_stash == null) throw new NullReferenceException("Bonus ball can not find any stash elements on scene");
        }

        private void OnDestroy() => transform.DOKill();

        public bool CanBeColoured()
        {
            return false;
        }

        public bool CanBeMovedByMouse()
        {
            return false;
        }

        public bool CanCountAsWinnable()
        {
            return true;
        }

        public void HandleCollision(Collision collision)
        {
            BallController ball;
            if (collision == null || !collision.gameObject.TryGetComponent(out ball) || _isTriggered) return;
            _isTriggered = true;
            PlayBonusBallAnimation();
        }

        private void PlayBonusBallAnimation()
        {
            _rb.isKinematic = true;
            Action destroy = () =>
            {
                if (gameObject == null || transform == null) return;
                SpawnBonusBalls();
                Destroy(gameObject);
            };
            Action doscale = () =>
            {
                if (gameObject == null || transform == null) return;
                transform.DOScale(new Vector3(0, 0, 0), _scaleSpeed)
                    .OnComplete(() => destroy());
            };
            transform.DOJump(transform.position + new Vector3(0, 1, 0), _jumpSpeed, 1, 0.5f)
                .OnComplete(() => doscale());
        }

        private void SpawnBonusBalls()
        {
            for (int i = 0; i < _bonusAmount; i++)
            {
                GameObject item = WeightsRandom.Pick(_possibleBonusBalls);
                GameObject ball = Instantiate(item, transform.position, Quaternion.identity);

                BallController ballController = ball.GetComponent<BallController>();
                ballController.SetBallStatus(BallGameStatus.OnStash);

                BallStartScaleAppear newBallAnim = ball.AddComponent<BallStartScaleAppear>();
                Action newBallAnimCompletionCallback = () =>
                {
                    if (ball == null || ball.transform == null || _stash == null) return;
                    //if (AudioManager.Instance != null) AudioManager.Instance.BonusBallSFX.CopyAndPlay(ball.transform.position);
                    ball.transform.DOMove(_stash.InsisibleBallPosition.position, _newBallMoveSpeed)
                        .SetDelay(0.1f);
                    _stash.AddNewBall(ballController);
                };
                newBallAnim.ManualStart(_scaleSpeed, i, newBallAnimCompletionCallback);
            }
        }
    }
}
