using UnityEngine;

namespace InteractiveTutorial
{
    public class TutorialReferenceObject : MonoBehaviour, ITutorialReferenceObject
    {
        [SerializeField]
        [Tooltip("Unique id for reference object which you can use to find it on the scene")]
        private string _uniqueID;
        [SerializeField]
        [Tooltip("Object transform reference")]
        private Transform _transform;
        [SerializeField]
        [Tooltip("Ball controller reference")]
        private BallController _ballController;
        [SerializeField]
        [Tooltip("Dissolution observer reference")]
        private DissolutionObserver _dissolutionObserver;

        public string UniqueID => _uniqueID;

        public Transform Transform => _transform;

        public BallController BallController => _ballController;

        public DissolutionObserver DissolutionObserver => _dissolutionObserver;

        public TutorialReferenceObject(string uniqueID, Transform transform)
        {
            _uniqueID = uniqueID;
            _transform = transform;
        }

        public TutorialReferenceObject(string uniqueID, BallController ballController)
        {
            _uniqueID = uniqueID;
            _ballController = ballController;
        }

        public TutorialReferenceObject(string uniqueID, Transform tranform, BallController ballController)
        {
            _uniqueID = uniqueID;
            _transform = tranform;
            _ballController = ballController;
        }
    }
}