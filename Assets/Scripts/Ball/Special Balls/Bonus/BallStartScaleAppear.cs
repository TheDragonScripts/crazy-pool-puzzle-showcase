using DG.Tweening;
using System;
using UnityEngine;

public class BallStartScaleAppear : MonoBehaviour
{
    [SerializeField] private float _scaleSpeed = 0.1f;
    [SerializeField] private float _delay = 0f;
    [SerializeField] private Action _onCompleteCallback = () => { };
    [SerializeField] private bool _autoStart = false;

    private Vector3 _savedScale;

    private void Awake()
    {
        _savedScale = transform.localScale;
        if (_autoStart) ScaleAppear();
    }

    private void OnDestroy() => transform.DOKill();

    public void ManualStart(float speed, float delay, Action onCompleteCallback)
    {
        _scaleSpeed = speed;
        _delay = delay;
        _onCompleteCallback = onCompleteCallback;
        ScaleAppear();
    }

    private void ScaleAppear()
    {
        transform.DOScale(Vector3.zero, 0f);
        transform.DOScale(_savedScale, _scaleSpeed)
            .SetDelay(_delay)
            .OnComplete(() => _onCompleteCallback());
    }
}
