using UnityEngine;

namespace InteractiveTutorial
{
    public class SwipePointerAction : IInteractiveTutorialAction
    {
        private readonly Vector3 _from;
        private readonly Vector3 _to;
        private readonly GameObject _fromTarget;
        private readonly GameObject _toTarget;
        private readonly InteractiveTutorialPointer _pointer;

        public SwipePointerAction(InteractiveTutorialPointer pointer, Vector3 from, Vector3 to)
        {
            _pointer = pointer;
            _from = from;
            _to = to;
        }

        public SwipePointerAction(InteractiveTutorialPointer pointer, GameObject from, GameObject to)
        {
            _pointer = pointer;
            _fromTarget = from;
            _toTarget = to;
        }

        public void Do()
        {
            if (_pointer == null)
            {
                Debug.LogError("Interactive Tutorial Pointer is null, but you're trying to access it");
                return;
            }
            if (_fromTarget != null && _toTarget != null)
                _pointer.CycleSwipe(_fromTarget, _toTarget);
            else
                _pointer.CycleSwipe(_from, _to);
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