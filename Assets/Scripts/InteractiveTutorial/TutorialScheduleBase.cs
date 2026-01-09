using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;
using CustomAttributes;

namespace InteractiveTutorial
{
    /*
     * Code here is temporary disabled because it references old UI system.
     * Pending refactoring.
     */
    public class TutorialScheduleBase : MonoBehaviour, IInteractiveTutorialScheduleBase
    {
        [SerializeField] private InteractiveTutorial _tutorial;
        [SerializeField] private Stash _stash;
        [SerializeField] private ObjectsHighlighter _highlighter;

        [SerializeField][ForceInterface(typeof(ILevelTutorialSchedule))]
        private UnityEngine.Object _levelTutorialSchedule;

        private InteractiveTutorialPointer _pointer;
        private InteractiveTutorialMessage _message;
        private Dictionary<string, ITutorialReferenceObject> _referenceObjects;

        public Dictionary<string, ITutorialReferenceObject> TutorialReferencesObjects => _referenceObjects;

        //private void Awake() => StartCoroutine(Initialize());

        public ITutorialReferenceObject FindTutorialReferenceObject(string id)
        {
            _referenceObjects ??= FormReferencesObjects();
            if (_referenceObjects.TryGetValue(id, out ITutorialReferenceObject foundReference))
                return foundReference;
            else
                throw new NullReferenceException("There is no tutorial reference object with given id");
        }

        private Dictionary<string, ITutorialReferenceObject> FormReferencesObjects()
        {
            Dictionary<string, ITutorialReferenceObject> result = new();
            TutorialReferenceObject[] referencesArray = FindObjectsByType<TutorialReferenceObject>(FindObjectsSortMode.None);
            foreach (TutorialReferenceObject referenceObject in referencesArray)
                result.Add(referenceObject.UniqueID, referenceObject);
            return result;
        }

        private void FormTutorial()
        {
            ILevelTutorialSchedule levelSchedule = _levelTutorialSchedule as ILevelTutorialSchedule;
            levelSchedule.FormAndStart(this, _tutorial, _message, _pointer, _highlighter, _stash);
        }

        /*private IEnumerator Initialize()
        {
            yield return UIManager.Instance != null;
            _message = UIManager.Instance.InteractiveTutorialMessage;
            _pointer = UIManager.Instance.InteractiveTutorialPointer;
            FormTutorial();
        }*/
    }
}