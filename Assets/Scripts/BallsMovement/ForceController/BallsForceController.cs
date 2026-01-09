using Cysharp.Threading.Tasks;
using EntryPoint.GameState;
using ModificatedUISystem;
using PlayerInputs;
using PlayerInputs.ObjectsPicker;

namespace BallsMovement
{
    public class BallsForceController : IBallsForceController
    {
        private readonly float _minForce;
        private readonly float _maxForce;
        private readonly float _forceIncrementer;
        private readonly IGlobalGameState _globalGameState;
        private readonly IUIFactory _uiFactory;
        private readonly IRaycastObjectPicker _raycastObjectPicker;

        private BallController _selectedBall;
        private MouseButtonAction _ballPressState;
        private float _direction = -1;

        public float Force { get; private set; }

        public BallsForceController(IGlobalGameState globalGameState, IUIFactory uiFactory,
            IRaycastObjectPicker raycastObjectPicker, BallsForceControllerSettingsScriptableObject settings)
        {
            _globalGameState = globalGameState;
            _uiFactory = uiFactory;
            _raycastObjectPicker = raycastObjectPicker;
            settings.Deconstruct(out _minForce, out _maxForce, out _forceIncrementer);

            _raycastObjectPicker.SceneObjectPicked += OnRigidbodyClicked;

            Force = _maxForce;
        }

        private void OnRigidbodyClicked(PickedObjectInfo pickedObjectInfo)
        {
            if (_globalGameState.Status != GameStatus.Level || pickedObjectInfo.GameObject == null 
                || !pickedObjectInfo.GameObject.TryGetComponent(out BallController ball))
                return;

            _ballPressState = pickedObjectInfo.PerformedButtonAction;
            if (_ballPressState == MouseButtonAction.Pressed)
            {
                _selectedBall = ball;
                _ = ManageMouseSpeedMultiplierAsync();
                ShowAimHelpers();
            }
            else
            {
                _selectedBall = null;
                HideAimHelpers();
            }
        }

        private void ShowAimHelpers()
        {
            CSDL.LogWarning($"{nameof(BallsForceController)} is trying to call AimHelpers, but " +
                $"{nameof(UIFactory)} does not contain them");
        }
        private void HideAimHelpers()
        {
            CSDL.LogWarning($"{nameof(BallsForceController)} is trying to call AimHelpers, but " +
               $"{nameof(UIFactory)} does not contain them");
        }

        private bool CanManipulateMultiplier()
        {
            return _globalGameState.Status == GameStatus.Level && _ballPressState == MouseButtonAction.Pressed &&
                _selectedBall != null && _selectedBall.CanBeMovedByMouse();
        }

        private async UniTaskVoid ManageMouseSpeedMultiplierAsync()
        {
            while (CanManipulateMultiplier())
            {
                await UniTask.WaitForFixedUpdate();
                Force += _forceIncrementer * _direction;
                if (Force >= _maxForce || Force <= _minForce)
                {
                    _direction *= -1;
                }
            }
            _direction = -1;
            Force = _maxForce;
        }
    }
}
