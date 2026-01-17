using EntryPoint;
using EntryPoint.GameState;
using System;
using System.Collections;
using UnityEngine;

namespace SpecialBalls
{
    /*
     * Code here is temporary disabled because it references old UI system.
     * Pending refactoring.
     */
    [RequireComponent(typeof(Rigidbody), typeof(BallController), typeof(BallAppearance))]
    public class BoardFlipper : MonoBehaviour, ISpecialBall
    {
        [Header("Settings")]
        [SerializeField] private float _cooldown = 3f;
        [SerializeField] private int _lifes = 3;

        private BallAppearance _ballApperance;
        private IGlobalGameState _globalGameState;
        private Rigidbody _rb;
        private bool _isTriggered;
        //private FlipTitle _flipTitle;
        private GameBoard _gameBoard;

        public BallController Controller { get; private set; }
        public bool CanBeColoured { get; private set; } = false;
        public bool CanBeMovedByMouse { get; private set; } = false;
        public bool CanCountAsWinnable { get; private set; } = true;

        private void Start()
        {
            _ballApperance = GetComponent<BallAppearance>();
            _globalGameState = GameEntryPoint.Instance.GlobalGameState;
            _rb = GetComponent<Rigidbody>();
            _gameBoard = FindAnyObjectByType<GameBoard>();
            Controller = GetComponent<BallController>();
            //_flipTitle = UIManager.Instance.GetActionTitleByName("FlipTitle") as FlipTitle;
            //_flipTitle.OnColorChanged += FlipBoard;
        }

        /*private void OnDisable() => _flipTitle.OnColorChanged -= FlipBoard;
        private void OnDestroy() => _flipTitle.OnColorChanged -= FlipBoard;*/

        public void HandleCollision(Collision collision)
        {
            BallController ball;
            if (collision == null || !collision.gameObject.TryGetComponent(out ball) || _isTriggered) return;
            if (_globalGameState == null || _globalGameState.Status != GameStatus.Level) return;
            StartCoroutine(Cooldown());
            StartFlip();
        }

        private void StartFlip()
        {
            _ballApperance.SetMaterial(BallColoring.Uncoloured);
            //Color flipColor = (_gameBoard.IsFlipped) ? _flipTitle.Red : _flipTitle.Blue;
            //_flipTitle.Show(flipColor);
        }

        private IEnumerator Cooldown()
        {
            _isTriggered = true;
            yield return new WaitForSeconds(_cooldown);
            _lifes -= 1;
            if (_lifes <= 0)
            {
                _rb.isKinematic = true;
                _ballApperance.ForceDisableShadows();
                gameObject.AddComponent<RealTimeDissolution>();
            }
            else
            {
                _ballApperance.SetMaterial(BallColoring.Coloured);
                _isTriggered = false;
            }
        }

        private void FlipBoard()
        {
            /*if (_gameBoard == null || gameObject == null || AudioManager.Instance == null) return;
            _gameBoard.Flip();
            AudioManager.Instance.FlipSFX.CopyAndPlay(transform.position);*/
        }

    }
}