using System;
using UnityEngine;

namespace ThemesManagement.Environment
{
    [Serializable]
    public struct EnvironmentSettings
    {
        [ColorUsage(false, true)] public Color SkyColor;
        [ColorUsage(false, true)] public Color EquatorColor;
        [ColorUsage(false, true)] public Color GroundColor;
    }
}
