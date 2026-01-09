namespace ThirdPartiesIntegrations.UnitedAnalytics.Vendors
{
    public partial class UnityAnalyticsVendor
    {
        /*
         * NOTE:
         * Attribute names must match event names received from IAnalyticsEventsInvoker
         */

        [UnityEventName("unitedAnalytics_sharedVendorsDebugString")]
        private class SharedVendorsDebugStringEvent : Unity.Services.Analytics.Event
        {
            public SharedVendorsDebugStringEvent(string debugString) : base("sharedVendorsDebugString")
            {
                SetParameter("debugString", debugString);
            }
        }

        [UnityEventName("unitedAnalytics_levelCompleted")]
        private class LevelCompletedEvent : Unity.Services.Analytics.Event
        {
            public LevelCompletedEvent(int levelId, int usedBalls) : base("levelCompleted")
            {
                SetParameter("gameLevelID", levelId);
                SetParameter("usedBalls", usedBalls);
            }
        }

        [UnityEventName("unitedAnalytics_levelFailed")]
        private class LevelFailureEvent : Unity.Services.Analytics.Event
        {
            public LevelFailureEvent(int levelId, int usedBalls, string failureReason) : base("levelFailed")
            {
                SetParameter("gameLevelID", levelId);
                SetParameter("usedBalls", usedBalls);
                SetParameter("levelFailureReason", failureReason);
            }
        }

        [UnityEventName("unitedAnalytics_levelStarted")]
        private class LevelStartedEvent : Unity.Services.Analytics.Event
        {
            public LevelStartedEvent(int levelId) : base("levelStarted")
            {
                SetParameter("gameLevelID", levelId);
            }
        }

        [UnityEventName("unitedAnalytics_uiOpened")]
        private class UIOpenedEvent : Unity.Services.Analytics.Event
        {
            public UIOpenedEvent(string uiName) : base("uiOpened")
            {
                SetParameter("uiName", uiName);
            }
        }

        [UnityEventName("unitedAnalytics_parameterInGameSettingsChanged")]
        private class ParameterInGameSettingsChangedEvent : Unity.Services.Analytics.Event
        {
            public ParameterInGameSettingsChangedEvent(string paramName, int paramValue) : base("parameterInGameSettingsChanged")
            {
                SetParameter("parameterNameInSettings", paramName);
                SetParameter("parameterValueinSettings", paramValue);
            }
        }

        [UnityEventName("unitedAnalytics_themeChanged")]
        private class ThemeChanged : Unity.Services.Analytics.Event
        {
            public ThemeChanged(string themeName) : base("themeChanged")
            {
                SetParameter("themeName", themeName);
            }
        }

        [UnityEventName("unitedAnalytics_tutorialBegin")]
        private class TutorialBegin : Unity.Services.Analytics.Event
        {
            public TutorialBegin(string tutorialName) : base("tutorialBegin")
            {
                SetParameter("tutorialName", tutorialName);
            }
        }

        [UnityEventName("unitedAnalytics_tutorialCompleted")]
        private class TutorialCompleted : Unity.Services.Analytics.Event
        {
            public TutorialCompleted(string tutorialName) : base("tutorialCompleted")
            {
                SetParameter("tutorialName", tutorialName);
            }
        }
    }
}
