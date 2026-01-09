using UnityEngine;

namespace EntryPoint.GameState
{
    public class GamePauseController : IGamePauseController
    {
        private int _pauseRequests;

        public bool IsGamePaused => _pauseRequests > 0;

        public void Pause()
        {
            _pauseRequests++;
            HandlePause();
        }

        public void Unpause()
        {
            _pauseRequests--;
            HandlePause();
        }

        private void HandlePause()
        {
            Time.timeScale = Mathf.Clamp01(_pauseRequests);
        }
    }
}
