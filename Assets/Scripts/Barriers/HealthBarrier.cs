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
    [SerializeField] protected int _health = 2;
    [SerializeField] protected float _targetCutoffHeight = 1.5f;
    [SerializeField] protected float _startCutoffHeight = 2f;
    [SerializeField] protected float _dissolveSpeed = 2f;

    protected DissolvableObject _dissolvableObject;
    protected Collider _collider;
    protected MeshRenderer _meshRenderer;

    protected DissolveAnimStatus _currentStatus = DissolveAnimStatus.Idle;
    protected float _currentDissolve = 2f;
    protected bool _destroyed;

    protected enum DissolveAnimStatus { Forward = -1, Back = 1, Idle = 0 }

    protected void Start()
    {
        _collider = GetComponent<Collider>();
        _dissolvableObject = GetComponent<DissolvableObject>();
        _meshRenderer = GetComponent<MeshRenderer>();
    }

    protected void FixedUpdate()
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

    protected void OnCollisionEnter(Collision collision)
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

        if (_collider is MeshCollider)
        {
            MeshCollider meshCollider = (MeshCollider)_collider;
            meshCollider.convex = true;
        }
        _collider.isTrigger = true;
        _dissolvableObject.StartDissolvableDestory();
        //AudioManager.Instance.DissolutionSFX.CopyAndPlay(transform.position);
    }

    protected void SetDissolve(float dissolve) => _meshRenderer.material.SetFloat("_CutoffHeightF", dissolve);
}
