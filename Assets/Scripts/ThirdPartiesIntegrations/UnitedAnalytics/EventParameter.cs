namespace ThirdPartiesIntegrations.UnitedAnalytics
{
    public class EventParameter
    {
        public string Name { get; }
        public object Value { get; }

        public EventParameter(string name, object value)
        {
            Name = name;
            Value = value;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
