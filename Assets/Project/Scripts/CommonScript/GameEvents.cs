using System;
using UnityEngine;

/// <summary>
/// Central decoupled event broker for communicating between gameplay systems and UI/audio.
/// Eliminates the need for UI components to poll in Update() or use FindAnyObjectByType.
/// </summary>
public static class GameEvents
{
    // --- Run / Timer Events ---
    /// <summary>
    /// Raised every second (or upon run start) with (elapsedTime, remainingTime, totalDuration).
    /// </summary>
    public static event Action<float, float, float> OnTimerUpdated;

    /// <summary>
    /// Raised when the player survives the level duration or beats the objective.
    /// </summary>
    public static event Action OnLevelCompleted;

    // --- Wave Events ---
    /// <summary>
    /// Raised when a new wave starts with (waveIndex, totalWaves, waveDefinition).
    /// </summary>
    public static event Action<int, int, WaveDefinition> OnWaveStarted;

    // --- Enemy Events ---
    /// <summary>
    /// Raised whenever the number of active alive enemies changes.
    /// </summary>
    public static event Action<int> OnEnemyCountChanged;

    // --- Player State Events ---
    /// <summary>
    /// Raised when player HP changes with (currentHP, maxHP).
    /// </summary>
    public static event Action<float, float> OnPlayerHPChanged;

    /// <summary>
    /// Raised when the player dies (Game Over).
    /// </summary>
    public static event Action OnPlayerDied;

    // --- Dispatch Helpers ---
    public static void TriggerTimerUpdated(float elapsed, float remaining, float total)
        => OnTimerUpdated?.Invoke(elapsed, remaining, total);

    public static void TriggerLevelCompleted()
        => OnLevelCompleted?.Invoke();

    public static void TriggerWaveStarted(int waveIndex, int totalWaves, WaveDefinition wave)
        => OnWaveStarted?.Invoke(waveIndex, totalWaves, wave);

    public static void TriggerEnemyCountChanged(int liveCount)
        => OnEnemyCountChanged?.Invoke(liveCount);

    public static void TriggerPlayerHPChanged(float currentHP, float maxHP)
        => OnPlayerHPChanged?.Invoke(currentHP, maxHP);

    public static void TriggerPlayerDied()
        => OnPlayerDied?.Invoke();

    /// <summary>
    /// Clears all event listeners (useful on scene changes or run resets to prevent leaks).
    /// </summary>
    public static void ResetAllEvents()
    {
        OnTimerUpdated = null;
        OnLevelCompleted = null;
        OnWaveStarted = null;
        OnEnemyCountChanged = null;
        OnPlayerHPChanged = null;
        OnPlayerDied = null;
    }
}
