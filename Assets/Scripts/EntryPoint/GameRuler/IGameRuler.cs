namespace EntryPoint.GameRuler
{
    public interface IGameRuler
    {
        event GameRulerEventHandler GameWon;
        event GameRulerEventHandler GameLost;

        void CheckForLoss();
        void CheckForCompletion(BallController caller);
    }
}