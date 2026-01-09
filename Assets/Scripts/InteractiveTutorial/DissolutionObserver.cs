using UnityEngine;

namespace InteractiveTutorial
{
    public class DissolutionObserver : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("Dissolvable object if exists, otherwise it will try to find RealTimeDissoultion component")]
        private DissolvableObject _dissolvableObject;

        public bool IsDissolutionInProcess
        {
            get
            {
                if (_dissolvableObject != null)
                    return _dissolvableObject.IsDissolve;
                else
                    return gameObject.TryGetComponent(out RealTimeDissolution component);
            }
        }
    }
}