using System;

namespace ThirdPartiesIntegrations.UnitedAnalytics.Vendors
{
    [AttributeUsage(AttributeTargets.Class)]
    public class UnityEventNameAttribute : Attribute
    {
        public string EventName { get; private set; }

        public UnityEventNameAttribute(string eventName)
        {
            EventName = eventName;
        }
    }
}
