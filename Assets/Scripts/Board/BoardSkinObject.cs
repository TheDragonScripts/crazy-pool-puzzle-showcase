using UnityEngine;

public class BoardSkinObject : MonoBehaviour
{
    [SerializeField] private Transform[] _model;
    public Transform[] Model => _model;
}
