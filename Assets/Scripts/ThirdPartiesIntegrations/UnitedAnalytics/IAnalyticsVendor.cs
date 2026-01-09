namespace ThirdPartiesIntegrations.UnitedAnalytics
{
    public interface IAnalyticsVendor
    {
        void LogEvent(string name, params EventParameter[] parameters);
    }
}
