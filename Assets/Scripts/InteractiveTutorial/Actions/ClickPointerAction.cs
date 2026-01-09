using UnityEngine;

namespace InteractiveTutorial
{
    public class ClickPointerAction : IInteractiveTutorialAction
    {
        private readonly Vector3 _position;
        private readonly GameObject _targetObject;
        private readonly InteractiveTutorialPointer _pointer;

        public ClickPointerAction(InteractiveTutorialPointer pointer, Vector3 position)
        {
            _pointer = pointer;
            _position = position;
        }

        public ClickPointerAction(InteractiveTutorialPointer pointer, GameObject targetObject)
        {
            _pointer = pointer;
            _targetObject = targetObject;
        }

        public void Do()
        {
            if (_pointer == null)
            {
                Debug.LogError("Interactive Tutorial Pointer is null, but you're trying to access it");
                return;
            }
            if (_targetObject == null)
                _pointer.CycleClick(_position);
            else
                _pointer.CycleClick(_targetObject);
        }

        public void Undo()
        {
            if (_pointer == null)
            {
                Debug.LogError("Interactive Tutorial Pointer is null, but you're trying to access it");
                return;
            }
            _pointer.HidePointer();
        }
    }
}