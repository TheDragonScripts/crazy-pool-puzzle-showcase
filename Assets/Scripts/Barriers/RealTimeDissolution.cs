using System;
using UnityEngine;


/*
* Code here is temporary disabled because it references old Audio system.
* Pending refactoring.
*/
public class RealTimeDissolution : MonoBehaviour
{
    private MeshRenderer _meshRenderer;
    private Material _dissolveMaterial;

    private void Start()
    {
        TryGetComponent(out _meshRenderer);
        if (_meshRenderer == null) throw new System.Exception("Real Time Dissolution requires mesh renderer component");

        _dissolveMaterial = Resources.Load("Materials/DissolveHDR_Mat", typeof(Material)) as Material;
        if (_dissolveMaterial == null) throw new System.Exception("Real Time Dissoultion material can not be found");

        Color baseColor = _meshRenderer.material.GetColor("_BaseColor");
        Color hdrColor = _meshRenderer.material.GetColor("_EmissionColor");
        float metallic = _meshRenderer.material.GetFloat("_Metallic");
        float smoothness = _meshRenderer.material.GetFloat("_Smoothness");

        _meshRenderer.material = _dissolveMaterial;

        _meshRenderer.material.SetColor("_BaseColor", baseColor);
        _meshRenderer.material.SetColor("_EmissionColor", hdrColor);
        _meshRenderer.material.SetFloat("_Metallic", metallic);
        _meshRenderer.material.SetFloat("_Smoothness", smoothness);

        DissolvableObject dissolvableObject = gameObject.AddComponent<DissolvableObject>();
        dissolvableObject.StartDissolvableDestory();
        //AudioManager.Instance.DissolutionSFX.CopyAndPlay(transform.position);
    }
}