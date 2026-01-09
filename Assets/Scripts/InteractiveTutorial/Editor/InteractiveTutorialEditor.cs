using UnityEditor;
using UnityEngine;

namespace InteractiveTutorial
{
    /*[CustomEditor(typeof(InteractiveTutorialController))]
    public class InteractiveTutorialEditor : Editor
    {
        private InteractiveTutorialController _controller;

        public override void OnInspectorGUI()
        {
            if (_controller == null)
            {
                _controller = (InteractiveTutorialController)target;
            }
            Rect defaultRect = new Rect(0, 0, 100, 16);
            EditorGUI.LabelField(defaultRect, "Stages");
            EditorGUI.LabelField(new Rect(defaultRect.x, defaultRect.y + 16, defaultRect.width, defaultRect.height), "Stages");
        }
    }*/
}
