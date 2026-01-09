using EnumerableAnchors;
using UnityEngine;

namespace InteractiveTutorial
{
    public class MessageAction : IInteractiveTutorialAction
    {
        private readonly Vector3 _position;
        private readonly EnumerableAnchor _anchor;
        private readonly Vector2 _customOffset;
        private readonly string _message;
        private readonly InteractiveTutorialMessage _interactiveTutorialMessage;
        private readonly bool _useAnchor;
        private readonly bool _useCustomOffset;

        private InteractiveTutorialMessage _storedMessage;

        public MessageAction(InteractiveTutorialMessage performer, Vector3 position, string message)
        {
            _interactiveTutorialMessage = performer;
            _position = position;
            _message = message;
        }

        public MessageAction(InteractiveTutorialMessage performer, EnumerableAnchor anchor, string message)
        {
            _interactiveTutorialMessage = performer;
            _anchor = anchor;
            _message = message;
            _useAnchor = true;
        }

        public MessageAction(InteractiveTutorialMessage performer, EnumerableAnchor anchor, Vector2 customOffset, string message)
        {
            _interactiveTutorialMessage = performer;
            _anchor = anchor;
            _customOffset = customOffset;
            _message = message;
            _useAnchor = true;
            _useCustomOffset = true;
        }

        public void Do()
        {
            if (_interactiveTutorialMessage == null)
            {
                Debug.LogError("Interactive Tutorial Message is null");
                return;
            }
            if (_storedMessage == null)
                _storedMessage = _interactiveTutorialMessage.CreateCopy();
            if (_useCustomOffset)
                _storedMessage.Show(_anchor, _customOffset, _message);
            else if (_useAnchor)
                _storedMessage.Show(_anchor, _message);
            else
                _storedMessage.Show(_position, _message);
        }

        public void Undo()
        {
            if (_interactiveTutorialMessage == null)
            {
                Debug.LogError("Interactive Tutorial Message is null");
                return;
            }
            if (_storedMessage == null)
                return;
            _storedMessage.Hide();
        }
    }
}