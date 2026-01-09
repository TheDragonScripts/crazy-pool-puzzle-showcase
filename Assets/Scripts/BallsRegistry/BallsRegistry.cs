using System;
using System.Collections.Generic;
using System.Linq;
using SDI;
using EntryPoint.Levels;

namespace Balls.Management
{
    public class BallsRegistry : IBallsRegistry, IDisposable
    {
        private bool _isDisposed;
        private ILevelManager _levelManager;
        private static List<BallController> _registry;

        [InjectionMethod]
        public void Inject(ILevelManager levelManager)
        {
            if (_registry == null)
            {
                _registry = new();
                _levelManager = levelManager;
                _levelManager.LevelBeginChanged += ClearRegistryOnLevelChange;
            }
        }

        public void Dispose()
        {
            DisposeController(true);
            GC.SuppressFinalize(this);
        }

        public (int colored, int uncolored, int stashed) GetBallsSummary()
        {
            if (!CheckForRegistryExistence())
            {
                return (0, 0, 0);
            }
            (int colored, int uncolored, int stashed) result = new();
            foreach (BallController ball in _registry)
            {
                result.colored += (ball.Coloring == BallColoring.Coloured) ? 1 : 0;
                result.uncolored += (ball.Coloring == BallColoring.Uncoloured) ? 1 : 0;
                result.stashed += (ball.GameStatus == BallGameStatus.OnStash) ? 1 : 0;
            }
            return result;
        }

        public bool IsMoveableBallsAvailable()
        {
            if (!CheckForRegistryExistence())
            {
                return false;
            }
            int moveableBalls = 0;
            foreach (BallController ball in _registry)
            {
                moveableBalls += (ball.CanBeMovedByMouse()) ? 1 : 0;
            }
            return moveableBalls > 0;
        }

        public void ClearRegistry()
        {
            if (!CheckForRegistryExistence())
            {
                return;
            }
            _registry.Clear();
        }

        public void RegisterBall(BallController ball)
        {
            if (!CheckForRegistryExistence() || _registry.Contains(ball))
            {
                return;
            }
            _registry.Add(ball);
        }

        public void RemoveBall(BallController ball)
        {
            if (!CheckForRegistryExistence() || !_registry.Contains(ball))
            {
                return;
            }
            _registry.Remove(ball);
        }

        public List<BallController> GetBalls()
        {
            return CheckForRegistryExistence() ? _registry.Select(x => x).ToList() : null;
        }

        protected virtual void DisposeController(bool disposing)
        {
            if (_isDisposed)
            {
                return;
            }
            if (_levelManager != null)
            {
                _levelManager.LevelBeginChanged -= ClearRegistryOnLevelChange;
            }
            _isDisposed = true;
        }

        private bool CheckForRegistryExistence()
        {
            if (_registry == null)
            {
                CSDL.LogError("Balls registry list is not initialized. It means " +
                    "that Inject method never called. Call the injection method manually and " +
                    "be sure to keep it on undestroyable object. In GameEntryPoint for example.");
                return false;
            }
            return true;
        }

        private void ClearRegistryOnLevelChange(int level) => ClearRegistry();
    }
}