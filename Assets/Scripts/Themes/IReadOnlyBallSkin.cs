using UnityEngine;

namespace ThemesManagement
{
    public interface IReadOnlyBallSkin
    {
        string SpecialBallClassName { get; }
        Material Colored { get; }
        Material Uncolored { get; }
    }
}
