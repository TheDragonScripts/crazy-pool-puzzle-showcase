using System;

namespace EntryPoint.Levels
{
    public interface ILevelManager
    {
        int CurrentLevel { get; }
        int FirstPlayableLevelID { get; }

        event Action<int> LevelBeginChanged;
        event Action<int> PlayableLevelBeginChanged;
        event Action<int> LevelLoaded;
        event Action<int> PlayableLevelLoaded;

        bool IsPlayableLevel(int level);
        bool PlayLevel(int level);
        bool ReplayLevel();
    }
}