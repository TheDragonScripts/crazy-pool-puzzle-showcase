using UnityEngine;

namespace InteractiveTutorial
{
    public class HighlightObjectAction : IInteractiveTutorialAction
    {
        private readonly GameObject _target;
        private readonly Color _color = Color.white;
        private readonly float _width = 4f;
        private readonly ObjectsHighlighter _highlighter;

        private string _registredHighlightedObjectID;

        public HighlightObjectAction(GameObject target, ObjectsHighlighter highlighter)
        {
            _target = target;
            _highlighter = highlighter;
        }

        public HighlightObjectAction(GameObject target, ObjectsHighlighter highlighter, Color color)
        {
            _target = target;
            _highlighter = highlighter;
            _color = color;
        }

        public HighlightObjectAction(GameObject target, ObjectsHighlighter highlighter, Color color, float width)
        {
            _target = target;
            _highlighter = highlighter;
            _color = color;
            _width = width;
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
            _registredHighlightedObjectID = "HOA_" + _target.GetHashCode().ToString();
            _highlighter.RegisterObject(_target, _registredHighlightedObjectID);
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