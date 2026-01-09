using UnityEngine;

namespace ThemesManagement
{
    [CreateAssetMenu(fileName = "ThemesSettingsScriptableObject", menuName = "Scriptable Objects/ThemesSettings")]
    public class ThemesSettingsScriptableObject : ScriptableObject
    {
        [SerializeField] private string _defaultThemeId = "standart";
        [SerializeField] private ThemeData[] _themesData;

        public void Deconstruct(out string defaultThemeId, out ThemeData[] themesData)
        {
            defaultThemeId = _defaultThemeId;
            themesData = _themesData;
        }
    }
}
