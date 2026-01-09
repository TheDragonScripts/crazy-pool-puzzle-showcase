using System;
using UnityEngine;
using UnityEngine.InputSystem;

namespace PlayerInputs.ObjectsPicker
{
    public class RaycastObjectPicker : IRaycastObjectPicker
    {
        private readonly InputAction _pointAction;
        private MouseButtonAction _performedButtonAction;

        public event Action<PickedObjectInfo> SceneObjectPicked;

        public RaycastObjectPicker()
        {
            InputAction clickAction = InputSystem.actions.FindAction("Click");
            clickAction.started += OnClickStarted;
            clickAction.canceled += OnClickCanceled;

            _pointAction = InputSystem.actions.FindAction("Point");
        }

        private void OnClickStarted(InputAction.CallbackContext _)
        {
            _performedButtonAction = MouseButtonAction.Pressed;
            DoRaycastHit();
        }

        private void OnClickCanceled(InputAction.CallbackContext _)
        {
            _performedButtonAction = MouseButtonAction.Released;
            DoRaycastHit();
        }

        private void DoRaycastHit()
        {
            if (this == null || _pointAction == null || Camera.main == null)
                return;

            Ray ray = Camera.main.ScreenPointToRay(_pointAction.ReadValue<Vector2>());
            if (Physics.Raycast(ray, out RaycastHit raycastHit))
            {
                SceneObjectPicked?.Invoke(new PickedObjectInfo(raycastHit, _performedButtonAction));
            }
        }
    }
}
