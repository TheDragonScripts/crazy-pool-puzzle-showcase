using UnityEngine;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    [CreateAssetMenu(fileName = "AdsInvokerSettingsScriptableObject", menuName = "Scriptable Objects/AdsInvokerSettings")]
    public class AdsInvokerSettingsScriptableObject : ScriptableObject
    {
        public int InterstitialAdsCooldown = 138;
        public int LastLevelWithoutAds = 10;
        public string InterstitialInGameAdId;

        public void Deconstruct(out int interstitialAdsCooldown, out int lastLevelWithoutAds, out string interstitialInGameAdId)
        {
            interstitialAdsCooldown = InterstitialAdsCooldown;
            lastLevelWithoutAds = LastLevelWithoutAds;
            interstitialInGameAdId = InterstitialInGameAdId;
        }
    }
}