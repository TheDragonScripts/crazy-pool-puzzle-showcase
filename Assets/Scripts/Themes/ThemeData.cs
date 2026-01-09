using System;
using ThemesManagement.Environment;
using UnityEngine;
using UnityEngine.Localization;

namespace ThemesManagement
{
    [Serializable]
    public class ThemeData : IReadOnlyThemeData
    {
        [Tooltip("Must match the product id in Google Play")]
        [SerializeField] private string _themeProductId;

        [Tooltip("Consider to provide a path to a Resources folder, otherwise it is necessary to " +
            "change AudioManagement audio clips loading principle.")]
        [SerializeField] private string _musicAudioClipsResourcePath;

        [SerializeField] private LocalizedString _displayName;
        [SerializeField] private LocalizedString _displayDescription;
        [SerializeField] private Environment.LightingSettings _lightingSettings;
        [SerializeField] private EnvironmentSettings _environmentSettings;
        [SerializeField] private GameObject _gameBoardPrefab;
        [SerializeField] private BallSkin[] _ballSkins;

        public string ThemeProductId => _themeProductId;
        public string MusicAudioClipsResourcePath => _musicAudioClipsResourcePath;
        public LocalizedString DisplayName => _displayName;
        public LocalizedString DisplayDescription => _displayDescription;
        public Environment.LightingSettings LightingSettings => _lightingSettings;
        public EnvironmentSettings EnvironmentSettings => _environmentSettings;
        public GameObject GameBoardPrefab => _gameBoardPrefab;
        public IReadOnlyBallSkin[] BallSkins
        {
            get
            {
                if (_ballSkins == null)
                {
                    return Array.Empty<IReadOnlyBallSkin>();
                }
                IReadOnlyBallSkin[] result = new IReadOnlyBallSkin[_ballSkins.Length];
                for (int i = 0; i < _ballSkins.Length; i++)
                {
                    result[i] = _ballSkins[i];
                }
                return result;
            }
        }
    }
}
