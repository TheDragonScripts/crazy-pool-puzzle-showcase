using AudioManagement.Core;
using SDI;
using ThemesManagement;

namespace AudioManagement
{
    /// <summary>
    /// Represents a main audio controller class for providing sounds and music into the game.
    /// This class is not universal and must be unique and rewritten for diffirent projects.
    /// Thus current controller contains logic applicable only to Crazy Pool Puzzle.
    /// </summary>
    /// <remarks>Please note that <see cref="SfxController"/> and <see cref="MusicController"/> 
    /// both use Resources.Load. This means you need to specify the paths to the files 
    /// located in the Resources folder.
    /// Resources folder.</remarks>
    public class ProjectAudioController
    {
        private SfxController _sfxController;
        private MusicController _musicController;
        private IThemesManager _themesManager;

        private const string AudioSourcePrefabPath = "Prefabs/Audio/Special/DefaultAudioSource";
        private const string DefaultSfxAudioClipsPath = "Prefabs/Audio/SFX/";
        private const string DefaultMusicAudioClipsPath = "Prefabs/Audio/Music/standart/";

        public ProjectAudioController()
        {
            _sfxController = new SfxController(AudioSourcePrefabPath, DefaultSfxAudioClipsPath);
            _musicController = new MusicController(AudioSourcePrefabPath, DefaultMusicAudioClipsPath);
            // Ok, now another issue has arisen where I can see flaws in the ActualPlayerData architecture. 
            // Refactoring required. A SetValue method and corresponding ValueChanged event need to be added.
            // ...
        }

        [InjectionMethod]
        public void Inject(IThemesManager themesManager)
        {
            _themesManager = themesManager;
            _themesManager.SubscribeToThemeChangeEvent(OnThemeChanged);
        }

        public void PlaySfx(string sfxName)
        {
            if (ActualPlayerData.Data.MuteSFX == 0)
            {
                _sfxController.Play(sfxName);
            }
        }

        private void PlayMusic()
        {
            if (ActualPlayerData.Data.MuteMusic == 0)
            {
                _musicController.Play();
            }
        }

        private void OnThemeChanged(ThemeData data)
        {
            _musicController.ProvideAudioClipsPath(data.MusicAudioClipsResourcePath);
            PlayMusic();
        }
    }
}
