namespace ThirdPartiesIntegrations.AgeGateSystem
{
    public interface IAgeGate
    {
        event AgeGateEventHandler UserAgeGroupObtained;
        bool IsUserAgeGroupObtained { get; }
        bool IsUserAdult { get; }
        void ChageUserAgeGroupFromOutside();
        void SubscribeToAgeGroupObtainingEvent(AgeGateEventHandler callback);
    }
}
