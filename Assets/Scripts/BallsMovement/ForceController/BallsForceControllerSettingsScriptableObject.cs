using UnityEngine;

namespace BallsMovement
{
    [CreateAssetMenu(fileName = "BallsForceControllerSettingsScriptableObject",
        menuName = "Scriptable Objects/BallsForceControllerSettings")]
    public class BallsForceControllerSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private float _minForce = 100f;
        [SerializeField] private float _maxForce = 1000f;
        [SerializeField] private float _forceIncrementer = 1f;

        public void Deconstruct(out float minForce, out float maxForce, out float forceIncrementer)
        {
            minForce = _minForce;
            maxForce = _maxForce;
            forceIncrementer = _forceIncrementer;
        }
    }
}