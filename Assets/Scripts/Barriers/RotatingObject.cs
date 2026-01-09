using UnityEngine;

[RequireComponent(typeof(Transform))]
public class RotatingObject : MonoBehaviour
{
    [SerializeField, Range(-10f, 10f)] private float _rotationSpeed = 0.25f;

    private void FixedUpdate()
    {
        if (gameObject != null && transform != null)
            transform.eulerAngles = new Vector3(transform.eulerAngles.x,
                transform.eulerAngles.y + 1 * _rotationSpeed, transform.eulerAngles.z);
    }
}
