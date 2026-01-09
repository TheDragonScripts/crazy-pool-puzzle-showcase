using System.Linq;
using UnityEngine;

namespace InteractiveTutorial
{
    public class HighlightAvailableBallAction : IInteractiveTutorialAction, IFallbackableAction
    {
        private readonly Color _color;
        private readonly float _width;
        private readonly ObjectsHighlighter _highlighter;

        private BallController _target;
        private string _registredHighlightedObjectID;

        public HighlightAvailableBallAction(BallController defaultHighlightTarget, ObjectsHighlighter highlighter,
            float width = 4f, Color? color = null)
        {
            _target = defaultHighlightTarget;
            _highlighter = highlighter;
            _width = width;
            _color = color ?? Color.white;
        }

        public void CheckForFallback()
        {
            if (_target == null) return;
            if (!_target.CanBeMovedByMouse() && _target.GameStatus == BallGameStatus.OnBoard)
            {
                BallController[] balls = Resources.FindObjectsOfTypeAll<BallController>();
                BallController newTarget = balls.FirstOrDefault(b => (b.CanBeMovedByMouse() || b.GameStatus == BallGameStatus.OnStash)
                    && b.GetHashCode() != _target.GetHashCode());
                if (newTarget != null)
                    _target = newTarget;
                _highlighter.UnregisterObject(_registredHighlightedObjectID);
                Do();
            }
        }

        public void Do()
        {
            if (_highlighter == null)
            {
                Debug.LogError("Highligher is null");
                return;
            }
            if (_target == null)
            {
                Debug.LogWarning("Target is null");
                return;
            }
            _registredHighlightedObjectID = "HOA_" + _target.gameObject.GetHashCode().ToString();
            _highlighter.RegisterObject(_target.gameObject, _registredHighlightedObjectID);
            _highlighter.MangeHighlightedObject(_registredHighlightedObjectID, _color, _width, true);
        }

        public void Undo()
        {
            if (_highlighter == null)
            {
                Debug.LogError("Highligher is null");
                return;
            }
            if (_target == null)
            {
                Debug.LogWarning("Target is null");
                return;
            }
            _highlighter.MangeHighlightedObject(_registredHighlightedObjectID, _color, _width, false);
        }
    }
}