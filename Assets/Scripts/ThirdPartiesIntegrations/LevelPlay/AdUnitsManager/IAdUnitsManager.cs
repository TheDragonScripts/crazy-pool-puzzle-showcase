namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public interface IAdUnitsManager
    {
        event AdUnitsManangerEventHandler AdClosed;
        event AdUnitsManangerEventHandler AdDisplayed;
        event AdUnitsManangerEventHandler RewardGranted;

        void RequestAd(string id);
    }
}