using System.Collections;
using UnityEngine;

public class AutoDestroyableParticle : MonoBehaviour
{
    [SerializeField] private float _particleLifeTime = 15f;

    private void Start() => StartCoroutine(AutoDestroy());
    private void OnEnable() => StartCoroutine(AutoDestroy());
    private void OnDisable() => StopAllCoroutines();
    private void OnDestroy() => StopAllCoroutines();

    private IEnumerator AutoDestroy()
    {
        yield return new WaitForSeconds(_particleLifeTime);
        Destroy(gameObject);
    }
}
