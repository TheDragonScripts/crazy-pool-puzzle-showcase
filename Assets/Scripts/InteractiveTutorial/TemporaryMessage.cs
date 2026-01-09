using System.Collections;
using UnityEngine;

namespace InteractiveTutorial
{
    [RequireComponent (typeof(InteractiveTutorialMessage))]
    public class TemporaryMessage : MonoBehaviour
    {
        private float _lifeTime;
        private InteractiveTutorialMessage _message;
        private bool _inProcess;

        public TemporaryMessage SetLifetime(float lifeTime)
        {
            _lifeTime = lifeTime;
            return this;
        }

        public void StartDestroying()
        {
            if (_inProcess) return;
            _inProcess = true;
            _message = GetComponent<InteractiveTutorialMessage>();
            if (_message == null || _message.IsReference)
            {
                Debug.LogWarning("You're trying to add temporary message component on Reference.");
                Destroy(this);
                return;
            }
            StartCoroutine(DestoryItself());
        }

        private IEnumerator DestoryItself()
        {
            yield return new WaitForSeconds(_lifeTime);
            if (_message == null) yield break;
            _message.Hide();
        }
    }
}