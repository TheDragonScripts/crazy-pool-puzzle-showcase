namespace EntryPoint.GameState
{
    public interface IGlobalGameState
    {
        GameStatus Status { get; }
        void Set(GameStatus status);
    }
}