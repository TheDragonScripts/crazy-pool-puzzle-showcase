using ModificatedUISystem;
using SDI;
using System;
using System.Linq;
using ThirdPartiesIntegrations.IAP;

namespace ThemesManagement
{
    public class ThemesManager : IThemesManager
    {
        private readonly string _defaultThemeId;
        private readonly ThemeData[] _themes;
        private IInAppPurchasingIntegration _inAppPurchasingIntegration;
        private IUIFactory _uiFactory;
        private string _weakAppliedTheme;

        public static string DefaultThemeIdStatic { get; private set; } = "standart";
        public string DefaultThemeId => _defaultThemeId;
        public ThemeData CurrentThemeData { get; private set; }
        public ThemeData[] ThemeDatas => _themes;

        public event Action<ThemeData> ThemeChanged;

        public ThemesManager(ThemesSettingsScriptableObject settings)
        {
            settings.Deconstruct(out _defaultThemeId, out _themes);
            DefaultThemeIdStatic = _defaultThemeId;
        }

        [InjectionMethod]
        public void Inject(IInAppPurchasingIntegration inAppPurchasingIntegration, IUIFactory uiFactory)
        {
            _uiFactory = uiFactory;
            _inAppPurchasingIntegration = inAppPurchasingIntegration;
            _inAppPurchasingIntegration.SubscribeToFetchEvent(OnProductsFetched);
            _uiFactory.UIWasHidden += OnUIWasHidden;
        }

        public void SubscribeToThemeChangeEvent(Action<ThemeData> callback)
        {
            ThemeChanged += callback;
            if (CurrentThemeData != null)
            {
                callback(CurrentThemeData);
            }
        }

        public void ApplyTheme(string themeId)
        {
            ThemeData themeData = GetThemeDataById(themeId);
            if (themeData == null || !IsThemePurchased(themeId))
            {
                CSDL.LogError($"Theme {themeId} is " +
                    $"{(themeData == null ? "not valid" : "not purchased")}");
                return;
            }

            ActualPlayerData.Data.SelectedTheme = themeId;
            CurrentThemeData = themeData;

            if (string.IsNullOrEmpty(_weakAppliedTheme) || _weakAppliedTheme != themeId)
            {
                ThemeChanged?.Invoke(themeData);
            }
            _weakAppliedTheme = themeId;
        }

        public void PreviewTheme(string themeId)
        {
            ThemeData themeData = GetThemeDataById(themeId);
            if (themeData == null)
            {
                CSDL.LogError($"{nameof(ThemesManager)} can't find theme with id {themeId}");
                return;
            }
            _weakAppliedTheme = themeId;
            ThemeChanged?.Invoke(themeData);
        }

        private void OnProductsFetched()
        {
            string themeId = string.IsNullOrEmpty(ActualPlayerData.Data.SelectedTheme)
                ? _defaultThemeId
                : ActualPlayerData.Data.SelectedTheme;
            ApplyTheme(themeId);
        }

        private void OnUIWasHidden(string id, bool isTemporaryHidden)
        {
            ApplyTheme(ActualPlayerData.Data.SelectedTheme);
        }

        private bool IsThemePurchased(string themeId)
        {
            return themeId == _defaultThemeId || _inAppPurchasingIntegration.IsSpecificProductPurchased(themeId);
        }

        private ThemeData GetThemeDataById(string themeId)
        {
            return _themes.FirstOrDefault(d => d.ThemeProductId == themeId);
        }
    }
}
