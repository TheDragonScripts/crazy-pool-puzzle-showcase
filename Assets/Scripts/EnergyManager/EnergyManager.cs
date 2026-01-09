using Cysharp.Threading.Tasks;
using ModificatedUISystem;
using ModificatedUISystem.UIElements;
using SDI;
using System;
using System.Collections.Generic;
using ThirdPartiesIntegrations.IAP;
using UnityEngine;

namespace EnergySystem
{
    public class EnergyManager : IEnergyManager
    {
        private readonly int _defaultEnergyCapacity;
        private readonly int _defaultLevelEnergyCost;
        private readonly int _hoursToRestoreFullEnergy;
        private readonly LevelEnergyCostException[] _levelsEnergyCostExceptions;
        private readonly bool _debugMode;

        private IUIFactory _uiFactory;
        private IInAppPurchasingIntegration _inAppPurchasingIntegration;
        private Dictionary<int, int> _levelsEnergyCostExceptionsDictionary = new Dictionary<int, int>();
        private bool _consumptionFinallyDisabled;

        private const int NextRestoreTimeInSeconds = 3660;
        public static int EnergyCapacity { get; private set; }

        public event Action<int> EnergyConsumed;
        public event Action EnergyFullyRestored;
        public event Action EnergyHalfRestored;

        public EnergyManager(EnergyManagerSettingsScriptableObject settings)
        {
            settings.Deconstruct(out _defaultEnergyCapacity,
                out _defaultLevelEnergyCost, out _hoursToRestoreFullEnergy, out _levelsEnergyCostExceptions,
                out _debugMode);
        }

        [InjectionMethod]
        public void Inject(IUIFactory uiFactory, IInAppPurchasingIntegration inAppPurchasingIntegration)
        {
            _uiFactory = uiFactory;
            _inAppPurchasingIntegration = inAppPurchasingIntegration;

            SetupExceptionsDictionary();
            SetupEnergyCapacityStaticProperty();
            NormalizeEnergyCount();
            HandleRestoreTime();
        }

        public bool ConsumeEnergy(int level)
        {
            if (_debugMode)
            {
                CSDL.LogWarning("BE ADVISED, DEBUG MOD ENABLED IN ENERGY MANAGER");
                return true;
            }
            if (_consumptionFinallyDisabled || _inAppPurchasingIntegration.IsAnyProductPurchased())
            {
                CSDL.Log("Theme is purchased, no need to consume energy.");
                _consumptionFinallyDisabled = true;
                return true;
            }

            int cost = GetLevelCost(level);
            int energy = ActualPlayerData.Data.Energy;
            if (cost <= energy)
            {
                ShowDecreaserBoxAsync(cost);
                ActualPlayerData.Data.Energy -= cost;
                EnergyConsumed?.Invoke(cost);
                return true;
            }
            else
            {
                _uiFactory.GetAsync<ModificatedUISystem.UIElements.EnergyRestoreOfferScreen>();
                return false;
            }
        }

        public void RestoreHalfEnergy()
        {
            int toRestore = EnergyCapacity / 2;
            ActualPlayerData.Data.Energy = toRestore;
            EnergyHalfRestored?.Invoke();
        }

        public int GetLevelCost(int level)
        {
            int levelCost = _defaultLevelEnergyCost;
            _levelsEnergyCostExceptionsDictionary.TryGetValue(level, out levelCost);
            return levelCost;
        }

        private void SetupEnergyCapacityStaticProperty()
        {
            EnergyCapacity = _defaultEnergyCapacity;
        }

        private void NormalizeEnergyCount()
        {
            if (ActualPlayerData.Data.Energy > EnergyCapacity)
                ActualPlayerData.Data.Energy = EnergyCapacity;
        }

        private void SetupExceptionsDictionary()
        {
            foreach (LevelEnergyCostException ex in _levelsEnergyCostExceptions)
                _levelsEnergyCostExceptionsDictionary.Add(ex.Level, ex.Cost);
        }

        private void HandleRestoreTime()
        {
            long nextEnergyRestoreTime = string.IsNullOrEmpty(ActualPlayerData.Data.NextEnergyRestoreTime)
                ? 0
                : Convert.ToInt64(ActualPlayerData.Data.NextEnergyRestoreTime);
            long unixNow = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
            _ = EnergyAutoRestorationAsync(Math.Clamp(nextEnergyRestoreTime - unixNow, 0, long.MaxValue));
        }

        private async UniTaskVoid EnergyAutoRestorationAsync(long timeToWait)
        {
            await UniTask.Delay(TimeSpan.FromSeconds(timeToWait), ignoreTimeScale : true);
            if (ActualPlayerData.Data == null || this == null)
            {
                return;
            }
            DateTimeOffset now = DateTimeOffset.UtcNow;
            long unixNow = now.ToUnixTimeSeconds();
            long newNextRestoreTime = now
                    .AddSeconds(NextRestoreTimeInSeconds)
                    .ToUnixTimeSeconds();
            int energyToRestore = EnergyCapacity / _hoursToRestoreFullEnergy;
            long timeDifference = Math.Abs(unixNow - Convert.ToInt64(ActualPlayerData.Data.NextEnergyRestoreTime));
            float hoursPassed = (timeDifference / 60) / 60;
            hoursPassed = Mathf.Clamp(hoursPassed, 1, float.MaxValue);
            CSDL.Log($"Difference in hours {hoursPassed}");

            ActualPlayerData.Data.NextEnergyRestoreTime = newNextRestoreTime.ToString();
            ActualPlayerData.Data.Energy += energyToRestore * Convert.ToInt32(hoursPassed);
            NormalizeEnergyCount();
            _ = EnergyAutoRestorationAsync(newNextRestoreTime - unixNow);
            EnergyFullyRestored?.Invoke();
        }

        private async void ShowDecreaserBoxAsync(int decreaser)
        {
            if (_uiFactory.IsUIOpened<Menu>() || _uiFactory.IsUIOpened<Levels>())
            {
                return;
            }
            ModificatedUISystem.UIElements.EnergyDecreaseBox box = await _uiFactory.GetAsync<ModificatedUISystem.UIElements.EnergyDecreaseBox>();
            box.Show(ActualPlayerData.Data.Energy, decreaser);
        }
    }
}