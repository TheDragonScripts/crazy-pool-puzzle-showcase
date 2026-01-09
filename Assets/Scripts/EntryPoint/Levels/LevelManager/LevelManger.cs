using System;
using EntryPoint.GameState;
using SDI;
using UnityEngine.SceneManagement;


namespace EntryPoint.Levels
{
    public class LevelManager : ILevelManager
    {
        private IGlobalGameState _globalGameState;
        private const int _firstPlayableLevelID = 2;
        private int _currentLevel;
        private bool _isLevelLoading;
        private bool _isAlreadyInjected;

        public int CurrentLevel => _currentLevel;
        public int FirstPlayableLevelID => _firstPlayableLevelID;

        public event Action<int> LevelBeginChanged;
        public event Action<int> PlayableLevelBeginChanged;
        public event Action<int> LevelLoaded;
        public event Action<int> PlayableLevelLoaded;

        [InjectionMethod]
        public void Inject(IGlobalGameState globalGameState)
        {
            if (_isAlreadyInjected)
            {
                return;
            }
            _globalGameState = globalGameState;
            SceneManager.sceneLoaded += OnSceneLoaded;
            _isAlreadyInjected = true;
        }

        public bool IsPlayableLevel(int level)
        {
            return level >= _firstPlayableLevelID;
        }

        public bool PlayLevel(int level)
        {
            if (!IsLevelUnlocked(level) || IsSameLevelOrLevelIsLoading(level))
            {
                return false;
            }
            LevelBeginChanged?.Invoke(level);
            if (IsPlayableLevel(level))
            {
                ActualPlayerData.Data.LastLevel = level;
                _globalGameState.Set(GameStatus.Level);
                PlayableLevelBeginChanged?.Invoke(level);
            }
            else
            {
                _globalGameState.Set(GameStatus.Menu);
            }
            _isLevelLoading = true;
            _currentLevel = level;
            SceneManager.LoadScene(level);
            return true;
        }

        public bool ReplayLevel() => PlayLevel(_currentLevel);

        private void OnSceneLoaded(Scene arg0, LoadSceneMode arg1)
        {
            _isLevelLoading = false;
            LevelLoaded?.Invoke(_currentLevel);
            if (IsPlayableLevel(_currentLevel))
            {
                PlayableLevelLoaded(_currentLevel);
            }
        }

        private bool IsSameLevelOrLevelIsLoading(int level) => level == _currentLevel || _isLevelLoading;

        private bool IsLevelUnlocked(int level) => ActualPlayerData.Data.MaxLevelUnlocked >= level;
    }
}