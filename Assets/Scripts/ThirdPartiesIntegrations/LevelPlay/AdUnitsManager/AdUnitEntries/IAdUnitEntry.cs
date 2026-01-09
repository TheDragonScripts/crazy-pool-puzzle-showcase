namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    public partial class AdUnitsManager
    {
        private interface IAdUnitEntry
        {
            string AdUnitId { get; }
            bool IsRequested { get; set; }
            bool IsAdReady();
            void LoadAd();
            void ShowAd();
        }
    }
}
