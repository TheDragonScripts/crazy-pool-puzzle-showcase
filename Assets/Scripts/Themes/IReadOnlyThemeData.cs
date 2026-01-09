using ThemesManagement.Environment;
using UnityEngine;
using UnityEngine.Localization;

namespace ThemesManagement
{
    public interface IReadOnlyThemeData
    {
        string ThemeProductId { get; }
        string MusicAudioClipsResourcePath { get; }
        LocalizedString DisplayName { get; }
        LocalizedString DisplayDescription { get; }
        Environment.LightingSettings LightingSettings { get; }
        EnvironmentSettings EnvironmentSettings { get; }
        GameObject GameBoardPrefab { get; }
        IReadOnlyBallSkin[] BallSkins { get; }
    }
}
