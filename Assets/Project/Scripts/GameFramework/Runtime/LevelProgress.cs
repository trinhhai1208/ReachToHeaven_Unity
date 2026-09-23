using UnityEngine;

/// <summary>
/// Persistent level-unlock progression, stored in PlayerPrefs.
/// Level 1 is always unlocked; completing level N unlocks level N+1.
/// </summary>
public static class LevelProgress
{
    private const string Key = "rth_unlocked_level";

    /// <summary>Highest level number the player has unlocked (>= 1).</summary>
    public static int UnlockedUpTo => Mathf.Max(1, PlayerPrefs.GetInt(Key, 1));

    public static bool IsUnlocked(int levelNumber) => levelNumber <= UnlockedUpTo;

    /// <summary>Unlocks up to the given level number (no-op if already unlocked further).</summary>
    public static void Unlock(int levelNumber)
    {
        if (levelNumber > UnlockedUpTo)
        {
            PlayerPrefs.SetInt(Key, levelNumber);
            PlayerPrefs.Save();
        }
    }

    /// <summary>Resets all progress (only level 1 unlocked). For testing / new game.</summary>
    public static void ResetProgress()
    {
        PlayerPrefs.DeleteKey(Key);
        PlayerPrefs.Save();
    }
}
