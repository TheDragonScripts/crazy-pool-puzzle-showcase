using UnityEngine;

[RequireComponent (typeof(MeshRenderer))]
public class DissolvableObject : MonoBehaviour
{
    [SerializeField] protected float _dissolveSpeed = 0.5f;
    [SerializeField] protected float _startDissolve = 1f;
    [SerializeField] protected float _targetDissolve = -1f;

    protected float _currentDissolve;
    protected bool _isDissolve;
    protected MeshRenderer _meshRenderer;

    public bool IsDissolve => _isDissolve;

    protected virtual void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        SetDissolve(_startDissolve);
    }

    protected virtual void FixedUpdate()
    {
        if (_isDissolve)
        {
            if (_currentDissolve > _targetDissolve)
            { 
                _currentDissolve -= _dissolveSpeed * Time.deltaTime;
                SetDissolve(_currentDissolve);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    public void StartDissolvableDestory() => _isDissolve = true;

    protected virtual void SetDissolve(float dissolve) => _meshRenderer.material.SetFloat("_CutoffHeightF", dissolve);
}
