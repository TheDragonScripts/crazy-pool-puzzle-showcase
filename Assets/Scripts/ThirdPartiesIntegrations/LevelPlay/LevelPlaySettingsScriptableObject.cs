using UnityEngine;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    [CreateAssetMenu(fileName = "LevelPlaySettingsScriptableObject", menuName = "Scriptable Objects/LevelPlaySettings")]
    public class LevelPlaySettingsScriptableObject : ScriptableObject
    {
        public string AppKey;
        public AdUnitInfo[] Ads;
        public bool IsTestSuiteEnabled;

        public void Deconstruct(out string appKey, out AdUnitInfo[] ads, out bool isTestSuiteEnabled)
        {
            appKey = AppKey;
            ads = Ads;
            isTestSuiteEnabled = IsTestSuiteEnabled;
        }
    }
}