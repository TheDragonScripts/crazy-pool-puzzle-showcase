using SpecialBalls;
using System;
using UnityEngine;

[RequireComponent(typeof(ElectroDissolvableObject))]
[RequireComponent(typeof(Collider))]
/*
 * Code here is temporary disabled because it references old Audio system.
 * Pending refactoring.
 */
public class ElectroBarrier : MonoBehaviour, IBarrier
{
    private ElectroDissolvableObject _dissolvableObject;
    private Collider _collider;

    private void Start()
    {
        _dissolvableObject = GetComponent<ElectroDissolvableObject>();
        _collider = GetComponent<Collider>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        BallController ball;
        if (collision == null || !collision.gameObject.TryGetComponent(out ball)) return;
        DissolvableObject dissolvableObjectBall;
        if (ball.TryGetComponent(out dissolvableObjectBall) || ball.SpeicalBall is Eight) return;
        ball.gameObject.AddComponent<RealTimeDissolution>();
        Rigidbody ballRb = ball.gameObject.GetComponent<Rigidbody>();
        ballRb.isKinematic = true;
        BallAppearance ballAppearance = ball.gameObject.GetComponent<BallAppearance>();
        ballAppearance.ForceDisableShadows();
    }

    public void DestoryBarrier()
    {
        if (_collider is MeshCollider meshCollider)
        {
            meshCollider.convex = true;
        }
        _collider.isTrigger = true;
        _dissolvableObject.StartDissolvableDestory();
        //AudioManager.Instance.DissolutionSFX.CopyAndPlay(transform.position);
    }
}
