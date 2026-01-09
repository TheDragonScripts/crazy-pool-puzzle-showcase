using System;
using UnityEngine;

namespace EntryPoint.GameData
{
    public class GameSaverAndLoader : MonoBehaviour, IGameSaverAndLoader
    {
        private bool _isInitialized;

        public event Action GameBeginSaved;

        public void Initialize()
        {
            if (_isInitialized)
            {
                return;
            }
            ActualPlayerData.LoadGame();
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus)
            {
                SaveGame();
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                SaveGame();
            }
        }

        private void OnApplicationQuit() => SaveGame();

        private void SaveGame()
        {
            GameBeginSaved?.Invoke();
            ActualPlayerData.SaveGame();
        }
    }
}