using System;
using UnityEngine;

[RequireComponent(typeof(DissolvableObject))]
[RequireComponent(typeof(Collider))]
[RequireComponent(typeof(MeshRenderer))]
/*
 * Code here is temporary disabled because it references old Audio system.
 * Pending refactoring.
 */
public class HealthBarrier : MonoBehaviour, IBarrier
{
    [SerializeField] private int _health = 2;
    [SerializeField] private float _targetCutoffHeight = 1.5f;
    [SerializeField] private float _startCutoffHeight = 2f;
    [SerializeField] private float _dissolveSpeed = 2f;

    private DissolvableObject _dissolvableObject;
    private Collider _collider;
    private MeshRenderer _meshRenderer;

    private DissolveAnimStatus _currentStatus = DissolveAnimStatus.Idle;
    private float _currentDissolve = 2f;
    private bool _destroyed;

    private enum DissolveAnimStatus { Forward = -1, Back = 1, Idle = 0 }

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _dissolvableObject = GetComponent<DissolvableObject>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate()
    {
        if (_currentStatus == DissolveAnimStatus.Forward && !_destroyed)
        {
            if (_currentDissolve > _targetCutoffHeight)
            {
                _currentDissolve -= _dissolveSpeed * Time.deltaTime;
                SetDissolve(_currentDissolve);
            }
            else _currentStatus = DissolveAnimStatus.Back;
        }
        else if (_currentStatus == DissolveAnimStatus.Back && !_destroyed)
        {
            if (_currentDissolve < _startCutoffHeight)
            {
                _currentDissolve += _dissolveSpeed * Time.deltaTime;
                SetDissolve(_currentDissolve);
            }
            else _currentStatus = DissolveAnimStatus.Idle;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision == null || collision.collider == null ||
            collision.gameObject.GetComponent<BallController>() == null) return;
        if (_health > 0)
        {
            _health--;
            _currentStatus = DissolveAnimStatus.Forward;
            //AudioManager.Instance.HalfDissolutionSFX.CopyAndPlay(transform.position);
        }
        else
        {
            DestoryBarrier();
        }
    }

    public void DestoryBarrier()
    {
        if (_destroyed) return;
        _destroyed = true;

        if (_collider is MeshCollider meshCollider)
        {
            meshCollider.convex = true;
        }
        _collider.isTrigger = true;
        _dissolvableObject.StartDissolvableDestory();
        //AudioManager.Instance.DissolutionSFX.CopyAndPlay(transform.position);
    }

    private void SetDissolve(float dissolve) => _meshRenderer.material.SetFloat("_CutoffHeightF", dissolve);
}
