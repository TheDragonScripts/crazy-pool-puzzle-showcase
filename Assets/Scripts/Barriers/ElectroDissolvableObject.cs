using UnityEngine;

public class ElectroDissolvableObject : DissolvableObject
{
    protected new void Start()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        SetDissolve(0f);
    }

    protected new void FixedUpdate()
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

    protected new void SetDissolve(float dissolve) => _meshRenderer.material.SetFloat("_Invisibility", dissolve);
}
