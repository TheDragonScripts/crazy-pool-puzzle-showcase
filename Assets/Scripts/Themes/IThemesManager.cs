using System;

namespace ThemesManagement
{
    public interface IThemesManager
    {
        ThemeData CurrentThemeData { get; }
        /// <summary>
        /// IMPORTANT NOTE: Do not change any reference types here!
        /// This data and all references contained therein are provided only on read only terms
        /// </summary>
        string DefaultThemeId { get; }
        ThemeData[] ThemeDatas { get; }
        event Action<ThemeData> ThemeChanged;
        void ApplyTheme(string themeId);
        void PreviewTheme(string themeId);
        void SubscribeToThemeChangeEvent(Action<ThemeData> callback);
    }
}
