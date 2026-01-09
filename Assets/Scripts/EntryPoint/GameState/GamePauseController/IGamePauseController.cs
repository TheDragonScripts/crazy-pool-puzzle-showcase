namespace EntryPoint.GameState
{
    public interface IGamePauseController
    {
        bool IsGamePaused { get; }

        void Pause();
        void Unpause();
    }
}