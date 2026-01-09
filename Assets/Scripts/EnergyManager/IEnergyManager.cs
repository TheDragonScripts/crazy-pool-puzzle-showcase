using System;

namespace EnergySystem
{
    public interface IEnergyManager
    {
        event Action<int> EnergyConsumed;
        event Action EnergyFullyRestored;
        event Action EnergyHalfRestored;

        bool ConsumeEnergy(int level);
        void RestoreHalfEnergy();
        int GetLevelCost(int level);
    }
}
