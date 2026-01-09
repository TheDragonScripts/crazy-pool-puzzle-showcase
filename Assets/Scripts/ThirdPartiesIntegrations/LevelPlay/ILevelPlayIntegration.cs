using System;

namespace ThirdPartiesIntegrations.LevelPlaySystem
{
    /// <summary>
    /// Represents a LevelPlay Integration. PLEASE DO NOT USE this class to directly show ads.
    /// Use default <seealso cref="AdsInvoker"/> class or write your own layer that
    /// corresponding to your ads displaying rules and principles.
    /// </summary>
    public interface ILevelPlayIntegration
    {
        event Action<string> AdClosed;
        event Action<string> AdDisplayed;
        event Action<string> RewardGranted;

        /// <summary>
        /// Use it to request ad show.
        /// </summary>
        /// <remarks>NOTE that <paramref name="inGameAdId"/> is not unit id from LevelPlay dashboard! This id is  
        /// from <see cref="LevelPlaySettingsScriptableObject"/> Ads.Name field of the specific <seealso cref="AdUnitInfo"/></remarks>
        /// <param name="inGameAdId"></param>
        void ShowAd(string inGameAdId);
    }
}