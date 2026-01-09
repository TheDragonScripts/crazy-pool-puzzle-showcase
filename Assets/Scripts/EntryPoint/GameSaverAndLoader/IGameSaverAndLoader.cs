using System;

namespace EntryPoint.GameData
{
    public interface IGameSaverAndLoader
    {
        event Action GameBeginSaved;
        void Initialize();
    }
}