using System;
using System.Text;
using UnityEngine.SceneManagement;
using ResolutionScale;
using EnergySystem;
using EntryPoint;
using ThemesManagement;

[Serializable]
public class PlayerData
{
    public int MaxLevelUnlocked;
    public int LastLevel;
    public int[] UsedBallsOnLevel;
    public string[] CompletedTutorials;
    public int Energy;
    public string NextEnergyRestoreTime;
    public string NextAdsFreeShowTime;
    public float ResolutionScale;

    public int MuteMusic;
    public int MuteSFX;
    public int MuteUIClick;
    public int RandomBallsInStash;
    public string SelectedTheme;
    public int DataCollectionConsent;
    public int PersonalizedAds;
    public string AnalyticsPolicyVersion;
    public string PersonalizedAdsPolicyVersion;

    public int IsUserAdult;
    public int IsAgeGateShown;
    
    public PlayerData()
    {
        MaxLevelUnlocked = GameEntryPoint.Instance.LevelManager.FirstPlayableLevelID;
        LastLevel = GameEntryPoint.Instance.LevelManager.FirstPlayableLevelID;
        Energy = EnergyManager.EnergyCapacity;
        UsedBallsOnLevel = new int[SceneManager.sceneCountInBuildSettings];
        CompletedTutorials = new string[0];
        SelectedTheme = ThemesManager.DefaultThemeIdStatic;
        ResolutionScale = ResolutionScaleManager.DefaultResolutionScale;
    }

    public override string ToString()
    {
        string restoreTime = (NextEnergyRestoreTime == "" || NextEnergyRestoreTime == null)
            ? "NOT INITIALIZED"
            : DateTime.UnixEpoch.AddSeconds(Convert.ToInt64(NextEnergyRestoreTime)).ToString();

        return new StringBuilder()
            .Append($"Max level unlocked: {MaxLevelUnlocked} | ")
            .Append($"Energy left: {Energy} | ")
            .Append($"Next energy restore time is {restoreTime} | ")
            .Append($"Used balls on levels size: {UsedBallsOnLevel.Length} | ")
            .Append($"Last Level {LastLevel} | ")
            .Append($"Mute music {MuteMusic} | ")
            .Append($"Mute SFX {MuteSFX} | ")
            .Append($"Mute UI click sound {MuteUIClick} | ")
            .Append($"Random balls in stash {RandomBallsInStash} | ")
            .Append($"Selected theme {SelectedTheme} | ")
            .Append($"Data collection cosent {DataCollectionConsent} | ")
            .Append($"Analytics Policy Version {AnalyticsPolicyVersion} | ")
            .Append($"Personalized Ads Policy Version {PersonalizedAdsPolicyVersion} | ")
            .Append($"Analytics Policy Consent {DataCollectionConsent} | ")
            .Append($"Personalized Ads Consent {PersonalizedAds} | ")
            .Append($"Resolution scale {ResolutionScale}")
            .ToString();
    }
}
