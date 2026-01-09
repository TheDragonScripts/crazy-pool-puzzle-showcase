using System;
using UnityEngine;

namespace EnergySystem
{
    [Serializable]
    public struct LevelEnergyCostException
    {
        public int Level;
        public int Cost;
    }

    [CreateAssetMenu(fileName = "EnergyManagerSettings", menuName = "Scriptable Objects/EnergyManagerSettings", order = 1)]
    public class EnergyManagerSettingsScriptableObject : ScriptableObject
    {
        [SerializeField, Range(0, 100000)] private int _defaultEnergyCapacity = 300;
        [SerializeField, Range(0, 100000)] private int _defaultLevelEnergyCost = 20;
        [SerializeField, Range(0, 100000)] private int _hoursToRestoreFullEnergy = 5;
        [SerializeField] private LevelEnergyCostException[] _levelsEnergyCostExceptions;
        [SerializeField] private bool _debugMode;

        public void Deconstruct(out int defaultEnergyCapacity,
            out int defaultLevelEnergyCost, out int hoursToRestoreFullEnergy,
            out LevelEnergyCostException[] levelsEnergyCostExceptions,
            out bool debugMode)
        {
            defaultEnergyCapacity = _defaultEnergyCapacity;
            defaultLevelEnergyCost = _defaultLevelEnergyCost;
            hoursToRestoreFullEnergy = _hoursToRestoreFullEnergy;
            levelsEnergyCostExceptions = _levelsEnergyCostExceptions;
            debugMode = _debugMode;
        }
    }
}
