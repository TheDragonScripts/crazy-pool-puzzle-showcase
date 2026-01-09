using System;
using UnityEngine;

namespace PlayerInputs.ObjectsPicker
{
    public readonly struct PickedObjectInfo
    {
        public RaycastHit RaycastHit { get; }
        public MouseButtonAction PerformedButtonAction { get; }
        public GameObject GameObject { get; }

        public PickedObjectInfo(RaycastHit raycastHit, MouseButtonAction mouseButtonAction)
        {
            RaycastHit = raycastHit;
            PerformedButtonAction = mouseButtonAction;
            GameObject = raycastHit.rigidbody?.gameObject;
        }
    }

    public interface IRaycastObjectPicker
    {
        event Action<PickedObjectInfo> SceneObjectPicked;
    }
}