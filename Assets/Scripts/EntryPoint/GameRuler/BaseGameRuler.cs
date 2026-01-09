using Cysharp.Threading.Tasks;
using SpecialBalls;
using System.Threading;
using SDI;
using EntryPoint.Levels;
using EntryPoint.GameState;
using Balls.Management;

namespace EntryPoint.GameRuler
{
    public class BaseGameRuler : IGameRuler
    {
        private IGlobalGameState _globalGameState;
        private IBallsRegistry _ballsRegistry;
        private ILevelManager _levelManager;
        private ILevelSession _levelSession;
        private readonly int _gameLossCheckTime;
        private bool _canCheck = true;
        private bool _isEightBallTouched;
        private CancellationTokenSource _gameLostCheckerCTSource;

        public event GameRulerEventHandler GameWon;
        public event GameRulerEventHandler GameLost;

        public BaseGameRuler(int gameLossCheckTime)
        {
            _gameLossCheckTime = gameLossCheckTime;
        }

        [InjectionMethod]
        public void Inject(IGlobalGameState globalGameState, IBallsRegistry ballsRegistry,
            ILevelManager levelManager, ILevelSession levelSession)
        {
            _levelManager = levelManager;
            _globalGameState = globalGameState;
            _ballsRegistry = ballsRegistry;
            _levelSession = levelSession;

            _levelManager.LevelLoaded += ResetCheckAbility;
        }

        public void CheckForCompletion(BallController caller)
        {
            if (!_canCheck || _globalGameState.Status != GameStatus.Level)
            {
                return;
            }
            if (caller.SpeicalBall.GetType() == typeof(Eight))
            {
                _isEightBallTouched = true;
            }
            (int colored, int uncolored, int stashed) = _ballsRegistry.GetBallsSummary();
            if (uncolored == 0 && _isEightBallTouched)
            {
                UpdateGameStatus(GameStatus.LevelWon, "All balls was colored");
            }
            else if (uncolored > 0 && _isEightBallTouched)
            {
                UpdateGameStatus(GameStatus.LevelLoss, "Locale.Failure.Reason.EightBallTouch");
            }
        }

        public void CheckForLoss()
        {
            if (!_canCheck || _globalGameState.Status != GameStatus.Level)
            {
                return;
            }
            _ = CheckForLossWithDelay();
        }

        private async UniTaskVoid CheckForLossWithDelay()
        {
            PrepareGameLostCTSource();
            await UniTask.WaitForSeconds(
                duration: _gameLossCheckTime,
                cancellationToken: _gameLostCheckerCTSource.Token);
            if (_gameLostCheckerCTSource.IsCancellationRequested || _ballsRegistry.IsMoveableBallsAvailable())
            {
                return;
            }
            (int colored, int uncolored, int stashed) = _ballsRegistry.GetBallsSummary();
            if (stashed == 0 && uncolored > 0)
            {
                UpdateGameStatus(GameStatus.LevelLoss, "Locale.Failure.Reason.NoMovesLeft");
            }
        }

        private void UpdateGameStatus(GameStatus newGameStatus, string reason)
        {
            _canCheck = false;
            _globalGameState.Set(newGameStatus);
            if (newGameStatus == GameStatus.LevelWon)
            {
                GameWon?.Invoke(reason, _levelManager.CurrentLevel, _levelSession.UsedBalls);
            }
            else if (newGameStatus == GameStatus.LevelLoss)
            {
                GameLost?.Invoke(reason, _levelManager.CurrentLevel, _levelSession.UsedBalls);
            }
        }

        private void PrepareGameLostCTSource()
        {
            _gameLostCheckerCTSource?.Cancel();
            _gameLostCheckerCTSource?.Dispose();
            _gameLostCheckerCTSource = new CancellationTokenSource();
        }

        private void ResetCheckAbility(int obj) => _canCheck = true;
    }
}