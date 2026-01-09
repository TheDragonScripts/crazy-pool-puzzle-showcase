using UnityEngine;
using System.Collections.Generic;

public class BallSpawnArea : MonoBehaviour
{
    private List<Collider> _colliders = new();

    public bool IsAreaTaken
    {
        get
        {
            return GetValidCollidersCount() > 0;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_colliders.Contains(other))
            _colliders.Add(other);
    }

    private void OnTriggerExit(Collider other)
    {
        if (_colliders.Contains(other))
            _colliders.Remove(other);
    }

    private int GetValidCollidersCount()
    {
        int count = 0;
        foreach (Collider collider in _colliders.ToArray())
        {
            if (collider == null)
                _colliders.Remove(collider);
            else
                count++;
        }
        return count;
    }
}
