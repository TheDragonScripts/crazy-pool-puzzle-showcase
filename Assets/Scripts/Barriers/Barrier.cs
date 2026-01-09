using System;
using UnityEngine;

[RequireComponent (typeof(DissolvableObject))]
[RequireComponent(typeof(Collider))]
/*
 * Code here is temporary disabled because it references Audio system.
 * Pending refactoring.
 */
public class Barrier : MonoBehaviour, IBarrier
{
    private DissolvableObject _dissolvableObject;
    private Collider _collider;

    private void Start()
    {
        _collider = GetComponent<Collider>();
        _dissolvableObject = GetComponent<DissolvableObject>();
    }

    public void DestoryBarrier()
    {
        if (_collider is MeshCollider)
        {
            MeshCollider meshCollider = (MeshCollider) _collider;
            meshCollider.convex = true;
        }
        _collider.isTrigger = true;
        _dissolvableObject.StartDissolvableDestory();
        //AudioManager.Instance.DissolutionSFX.CopyAndPlay(transform.position);
    }
}
