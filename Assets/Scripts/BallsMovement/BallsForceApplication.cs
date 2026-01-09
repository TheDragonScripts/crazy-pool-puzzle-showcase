using AudioManagement;
using EntryPoint.GameRuler;
using EntryPoint.GameState;
using ModificatedUISystem;
using PlayerInputs;
using PlayerInputs.ObjectsPicker;
using SDI;
using System;
using UnityEngine;

namespace BallsMovement
{
    public class BallsForceApplication : IBallsForceApplication
    {
        private readonly BallsForceControllerSettingsScriptableObject _ballsForceControllerSettings;

        private IBallsForceController _forceController;
        private IGlobalGameState _globalGameState;
        private ProjectAudioController _projectAudioController;
        private IGameRuler _gameRuler;
        private BallController _selectedBall;

        public event Action ForceApplied;

        public BallsForceApplication(BallsForceControllerSettingsScriptableObject ballsForceControllerSettings)
        {
            _ballsForceControllerSettings = ballsForceControllerSettings;
        }

        [InjectionMethod]
        public void Inject(IGlobalGameState globalGameState, IUIFactory uiFactory, IRaycastObjectPicker raycastObjectPicker,
            ProjectAudioController projectAudioController, IGameRuler gameRuler)
        {
            _globalGameState = globalGameState;
            _forceController = new BallsForceController(globalGameState, uiFactory,
                raycastObjectPicker, _ballsForceControllerSettings);
            _projectAudioController = projectAudioController;
            _gameRuler = gameRuler;

            raycastObjectPicker.SceneObjectPicked += OnSceneObjectClicked;
        }

        private void OnSceneObjectClicked(PickedObjectInfo pickedObjectInfo)
        {
            if (_globalGameState.Status != GameStatus.Level)
                return;

            if (pickedObjectInfo.PerformedButtonAction == MouseButtonAction.Pressed)
            {
                SelectBall(pickedObjectInfo.GameObject);
            }
            else
            {
                if (_selectedBall != null && _selectedBall.CanBeMovedByMouse())
                {
                    ApplyForceOnBall(pickedObjectInfo.RaycastHit.point);
                    _projectAudioController.PlaySfx("ball-hit");
                    _gameRuler.CheckForLoss();

                    _selectedBall = null;
                }
            }
        }

        private void SelectBall(GameObject gameObject)
        {
            if (gameObject == null || !gameObject.TryGetComponent(out BallController ball))
                return;

            _selectedBall = ball;
        }

        private void ApplyForceOnBall(Vector3 destinationPoint)
        {
            Vector3 direction = destinationPoint - _selectedBall.gameObject.transform.position;
            direction.Normalize();

            _selectedBall.ApplyForce(new Vector3(direction.x, 0, direction.z) * _forceController.Force);
            ForceApplied?.Invoke();
        }
    }
}
