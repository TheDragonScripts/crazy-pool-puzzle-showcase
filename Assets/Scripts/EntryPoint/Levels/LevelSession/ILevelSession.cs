namespace EntryPoint.Levels
{
    public interface ILevelSession
    {
        int UsedBalls { get; }
        string FailureReason { get; }

        void SetFailureReason(string reason);
    }
}