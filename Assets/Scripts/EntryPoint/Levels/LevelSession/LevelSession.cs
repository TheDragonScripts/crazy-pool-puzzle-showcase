using BallsMovement;
using SDI;

namespace EntryPoint.Levels
{
    public class LevelSession : ILevelSession
    {
        private ILevelManager _levelManager;
        private IBallsForceApplication _ballsForceApplication;

        public int UsedBalls { get; private set; }
        public string FailureReason { get; private set; }

        [InjectionMethod]
        public void Inject(ILevelManager levelManager, IBallsForceApplication ballsForceApplication)
        {
            _levelManager = levelManager;
            _ballsForceApplication = ballsForceApplication;

            _levelManager.PlayableLevelLoaded += Reset;
            _ballsForceApplication.ForceApplied += IncreaseUsedBalls;
        }

        public void SetFailureReason(string reason) => FailureReason = reason;

        private void IncreaseUsedBalls() => UsedBalls++;

        private void Reset(int level) => UsedBalls = 0;
    }
}