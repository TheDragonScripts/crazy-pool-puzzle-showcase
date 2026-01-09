using UnityEngine;

public class BallShadow : MonoBehaviour
{
    [SerializeField] private BallController _target;
    [SerializeField] private Vector3 _offset;

    private void Update()
    {
        if (_target != null && gameObject != null && transform != null)
            transform.position = _target.transform.position + _offset;
    }

    public void SetTarget(BallController target) => _target = target;
}
