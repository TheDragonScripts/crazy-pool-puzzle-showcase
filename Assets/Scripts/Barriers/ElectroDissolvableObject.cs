using UnityEngine;

public class ElectroDissolvableObject : DissolvableObject
{
    protected override void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        SetDissolve(0f);
    }

    protected override void FixedUpdate()
    {
        if (_isDissolve)
        {
            if (_currentDissolve < 1)
            {
                _currentDissolve += _dissolveSpeed * Time.deltaTime;
                SetDissolve(_currentDissolve);
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }

    protected override void SetDissolve(float dissolve) => _meshRenderer.material.SetFloat("_Invisibility", dissolve);
}
