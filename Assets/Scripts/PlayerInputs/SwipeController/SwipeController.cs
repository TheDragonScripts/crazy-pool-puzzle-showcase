using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInputs.Swipes
{
    public class SwipeController : ISwipeController
    {
        private readonly float _swipeDetection = 100;
        private InputAction _pointInputAction;
        private Vector2 _firstMousePos;
        private Vector2 _lastMousePos;

        public event Action SwipeUp;
        public event Action SwipeDown;
        public event Action SwipeLeft;
        public event Action SwipeRight;

        public SwipeController(float swipeDetection)
        {
            _swipeDetection = swipeDetection;

            InputAction clickInputAction = InputSystem.actions.FindAction("Click");
            clickInputAction.started += OnClickActionStarted;
            clickInputAction.canceled += OnClickActionCanceled;

            _pointInputAction = InputSystem.actions.FindAction("Point");
        }

        private void OnClickActionStarted(InputAction.CallbackContext _)
        {
            _firstMousePos = _pointInputAction.ReadValue<Vector2>();
        }

        private void OnClickActionCanceled(InputAction.CallbackContext _)
        {
            _lastMousePos = _pointInputAction.ReadValue<Vector2>();
            DetectSwipe();
        }

        private void DetectSwipe()
        {
            float deltaX = _firstMousePos.x - _lastMousePos.x;
            float deltaY = _firstMousePos.y  - _lastMousePos.y;

            if (Mathf.Abs(deltaX) < _swipeDetection && Mathf.Abs(deltaY) < _swipeDetection)
                return;

            bool isHorizontal = Mathf.Abs(deltaX) > Mathf.Abs(deltaY);

            if (isHorizontal)
            {
                (deltaX > 0 ? SwipeRight : SwipeLeft)?.Invoke();
            }
            else
            {
                (deltaY > 0 ? SwipeUp : SwipeDown)?.Invoke();
            }
        }
    }
}