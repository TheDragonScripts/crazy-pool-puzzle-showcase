namespace EntryPoint.GameState
{
    public class GlobalGameState : IGlobalGameState
    {
        public GameStatus Status { get; private set; }

        public void Set(GameStatus status)
        {
            Status = status;
        }
    }
}