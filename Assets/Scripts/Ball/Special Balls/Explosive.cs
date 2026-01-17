using System.Collections;
using UnityEngine;

namespace SpecialBalls
{
    /*
     * Code here is temporary disabled because it references old Audio system.
     * Pending refactoring.
     */
    [RequireComponent(typeof(BallController), typeof(MeshRenderer))]
    public class Explosive : MonoBehaviour, ISpecialBall
    {
        [SerializeField] private GameObject _particles;
        [SerializeField] private float _explosionRaidus = 5f;
        [SerializeField] private float _explosionForce = 500f;
        [SerializeField] private float _flashingFrequency = 0.5f;
        [SerializeField] private float _explodeFlashingFrequency = 0.05f;

        private MeshRenderer _meshRenderer;
        private float _timeToBoom = 3f;
        private bool _isTriggered;

        public BallController Controller { get; private set; }
        public bool CanBeColoured { get; private set; } = false;
        public bool CanBeMovedByMouse { get; private set; } = false;
        public bool CanCountAsWinnable { get; private set; } = true;

        private void Start()
        {
            Controller = GetComponent<BallController>();
            _meshRenderer = GetComponent<MeshRenderer>();
            StartCoroutine(Flashing());
        }

        private void OnDisable() => StopAllCoroutines();
        private void OnDestroy() => StopAllCoroutines();

        public void HandleCollision(Collision collision)
        {
            BallController ball;
            if (collision == null || !collision.gameObject.TryGetComponent(out ball) || _isTriggered) return;
            _isTriggered = true;
            StartCoroutine(Boom());
        }

        private void AppyExplosionForce()
        {
            Collider[] surroundingObjects = Physics.OverlapSphere(transform.position, _explosionRaidus);
            foreach (Collider collider in surroundingObjects)
            {
                Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
                if (rb == null) continue;
                rb.AddExplosionForce(_explosionForce, transform.position, _explosionRaidus);
            }

            //AudioManager.Instance.ExplosionSFX.CopyAndPlay(transform.position);

            Instantiate(_particles, transform.position, Quaternion.identity);
            Destroy(gameObject);
        }

        private IEnumerator Boom()
        {
            _flashingFrequency = _explodeFlashingFrequency;
            //AudioManager.Instance.PreExplosionSFX.CopyAndPlay(transform.position);
            yield return new WaitForSeconds(_timeToBoom);
            StopCoroutine(Boom());
            AppyExplosionForce();
        }

        private IEnumerator Flashing()
        {
            while (true)
            {
                yield return new WaitForSeconds(_flashingFrequency);
                _meshRenderer.material.SetFloat("_Visibility", 1f);
                yield return new WaitForSeconds(_flashingFrequency);
                _meshRenderer.material.SetFloat("_Visibility", 0f);
            }
        }
    }
}
