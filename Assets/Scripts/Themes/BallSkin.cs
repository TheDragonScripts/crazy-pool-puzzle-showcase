using System;
using UnityEngine;

namespace ThemesManagement
{
    [Serializable]
    public struct BallSkin : IReadOnlyBallSkin
    {
        [Tooltip("The class name of one of the implementations of ISpecialBall")]
        [SerializeField] private string _specialBallClassName;
        [SerializeField] private Material _colored;
        [SerializeField] private Material _uncolored;

        public string SpecialBallClassName => _specialBallClassName;
        public Material Colored => _colored;
        public Material Uncolored => _uncolored;
    }
}
