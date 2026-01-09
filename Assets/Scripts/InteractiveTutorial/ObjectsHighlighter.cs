using System.Collections.Generic;
using UnityEngine;
using static Outline;

namespace InteractiveTutorial
{
    public class ObjectsHighlighter : MonoBehaviour
    {
        [Header("Settings")]
        [SerializeField] private Color _defaultHighlightColor = Color.white;
        [SerializeField] private float _highlightWidth = 4f;
        [SerializeField] private Mode _highlightMode = Mode.OutlineVisible;

        private Dictionary<string, Outline> _highlightedObjects = new();

        public void RegisterObject(GameObject g, string id)
        {
            if (_highlightedObjects.ContainsKey(id)) return;
            bool found = g.TryGetComponent(out Outline outline);
            if (!found) outline = g.AddComponent<Outline>();
            outline.enabled = true;
            outline.OutlineColor = _defaultHighlightColor;
            outline.OutlineWidth = _highlightWidth;
            outline.OutlineMode = _highlightMode;
            _highlightedObjects.Add(id, outline);
        }

        public void UnregisterObject(string id)
        {
            if (!_highlightedObjects.ContainsKey(id)) return;
            _highlightedObjects[id].enabled = false;
            _highlightedObjects.Remove(id);
        }

        public void MangeHighlightedObject(string objectID, bool show)
        {
            if (!_highlightedObjects.ContainsKey(objectID))
                throw new System.NullReferenceException("There is no outline object in dictionary." +
                    "Register this object first, before calling");

            _highlightedObjects[objectID].enabled = show;
        }

        public void MangeHighlightedObject(string objectID, Color color, bool show)
        {
            if (!_highlightedObjects.ContainsKey(objectID))
                throw new System.NullReferenceException("There is no outline object in dictionary." +
                    "Register this object first, before calling");

            _highlightedObjects[objectID].enabled = show;
            _highlightedObjects[objectID].OutlineColor = color;
        }

        public void MangeHighlightedObject(string objectID, Color color, float width, bool show)
        {
            if (!_highlightedObjects.ContainsKey(objectID))
                throw new System.NullReferenceException("There is no outline object in dictionary." +
                    "Register this object first, before calling");

            _highlightedObjects[objectID].enabled = show;
            _highlightedObjects[objectID].OutlineColor = color;
            _highlightedObjects[objectID].OutlineWidth = width;
        }
    }
}