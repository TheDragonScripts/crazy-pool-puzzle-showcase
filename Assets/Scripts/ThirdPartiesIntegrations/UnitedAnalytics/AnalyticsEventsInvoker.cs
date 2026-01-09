using EntryPoint.GameRuler;
using EntryPoint.Levels;
using ModificatedUISystem;
using ModificatedUISystem.UIElements;
using SDI;
using ThemesManagement;

namespace ThirdPartiesIntegrations.UnitedAnalytics
{
    /*
     * - NOTE -
     * Parameter names are written in the same way as in the Google Analytics Dashboard (Firebase).
     * That's why they're written in the snake_case format. But it's not necessary, just make sure
     * you've correctly configured Custom Definitions in the GA Dashboard.
     * 
     * - Possible events -
     * unitedAnalytics_sharedVendorsDebugString
     *  string debugString
     *  
     * unitedAnalytics_levelCompleted
     *  int game_level_id
     *  int used_balls
     * 
     * unitedAnalytics_levelFailed
     *  int game_level_id
     *  int used_balls
     *  string failure_reason
     * 
     * unitedAnalytics_levelStarted
     *  int game_level_id
     * 
     * unitedAnalytics_uiOpened
     *  string ui_name
     * 
     * unitedAnalytics_parameterInGameSettingsChanged
     *  string parameter_in_game_settings
     *  int parameter_value
     * 
     * unitedAnalytics_themeChanged
     *  string themeName
     * 
     * unitedAnalytics_tutorialBegin
     *  string tutorial_name
     * 
     * unitedAnalytics_tutorialCompleted
     *  string tutorial_name
     */
    /// <summary>
    /// This class must be unique for each single project!
    /// </summary>
    public class AnalyticsEventsInvoker : IAnalyticsEventsInvoker
    {
        private IAnalyticsVendor[] _vendors;

        public AnalyticsEventsInvoker(params IAnalyticsVendor[] vendorsToUse)
        {
            _vendors = vendorsToUse;
        }

        [InjectionMethod]
        public void Inject(ILevelManager levelManager, IGameRuler gameRuler, IUIFactory uiFactory, IThemesManager themeManager)
        {
            levelManager.PlayableLevelLoaded += OnPlayableLevelLoaded;
            gameRuler.GameWon += OnGameLost;
            gameRuler.GameWon += OnGameWon;
            uiFactory.UIWasShown += OnUIWasShown;
            themeManager.ThemeChanged += OnThemeChanged;

            CSDL.LogWarning($"{nameof(AnalyticsEventsInvoker)} events " +
                $"unitedAnalytics_parameterInGameSettingsChanged, " +
                $"unitedAnalytics_tutorialBegin, " +
                $"unitedAnalytics_tutorialCompleted are not implemented yet because corresponding modules have not yet been refactored");
        }

        private void OnThemeChanged(ThemeData data)
        {
            LogEvent("unitedAnalytics_themeChanged", new EventParameter("themeName", data.ThemeProductId));
        }

        private void OnUIWasShown(string id, IUIElement uiElement)
        {
            if (uiElement is IUIOfType<MenuType>)
            {
                LogEvent("unitedAnalytics_uiOpened", new EventParameter("ui_name", id));
            }
        }

        private void OnGameWon(string reason, int level, int usedBalls)
        {
            LogEvent("unitedAnalytics_levelCompleted",
                new EventParameter("game_level_id", level),
                new EventParameter("used_balls", usedBalls));
        }

        private void OnGameLost(string reason, int level, int usedBalls)
        {
            LogEvent("unitedAnalytics_levelFailed", 
                new EventParameter("game_level_id", level),
                new EventParameter("used_balls", usedBalls),
                new EventParameter("failure_reason", reason));
        }

        private void OnPlayableLevelLoaded(int level)
        {
            LogEvent("unitedAnalytics_levelStarted", new EventParameter("game_level_id", level));
        }

        private void LogEvent(string name, params EventParameter[] parameters)
        {
            foreach (IAnalyticsVendor vendor in _vendors)
            {
                vendor.LogEvent(name, parameters);
            }
        }
    }
}
