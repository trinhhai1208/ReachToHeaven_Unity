using TMPro;
using UnityEngine;

/// <summary>
/// Event-driven HUD. Mode-aware:
///  - Level / Survival: TIME counts DOWN (remaining), WAVE shows current / total, shows ALIVE enemies.
///  - Endless:          TIME counts UP (elapsed),   WAVE shows current only,     shows KILLED enemies.
/// Pure view: subscribes to GameEvents, no Update() polling and no FindAnyObjectByType.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class SurviveHUD : MonoBehaviour
{
    private TextMeshProUGUI m_text;

    private string m_modeLabel = "SURVIVE";
    private bool m_isEndless;

    private int m_displayMinutes;
    private int m_displaySeconds;
    private int m_currentWave;
    private int m_totalWaves;
    private int m_liveEnemies;
    private int m_kills;

    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
    }

    private void Start()
    {
        if (RunConfig.Current != null && RunConfig.Current.Level != null)
        {
            m_modeLabel = RunConfig.Current.Level.DisplayName.ToUpper();
            m_totalWaves = RunConfig.Current.Level.TotalWaves;
            m_isEndless = false;
        }
        else if (RunConfig.Current != null && RunConfig.Current.Mode != null)
        {
            GameModeDefinition mode = RunConfig.Current.Mode;
            m_modeLabel = mode.DisplayName.ToUpper();
            m_isEndless = mode.ModeType == GameModeType.Endless || mode.Duration <= 0f;
        }

        m_liveEnemies = EnemyTracker.LiveCount;
        m_kills = GameStats.KillCount;
        RenderHUD();
    }

    private void OnEnable()
    {
        GameEvents.OnTimerUpdated += HandleTimerUpdated;
        GameEvents.OnWaveStarted += HandleWaveStarted;
        GameEvents.OnEnemyCountChanged += HandleEnemyCountChanged;
    }

    private void OnDisable()
    {
        GameEvents.OnTimerUpdated -= HandleTimerUpdated;
        GameEvents.OnWaveStarted -= HandleWaveStarted;
        GameEvents.OnEnemyCountChanged -= HandleEnemyCountChanged;
    }

    private void HandleTimerUpdated(float elapsed, float remaining, float total)
    {
        // Endless counts up; timed modes count down.
        float t = m_isEndless ? elapsed : remaining;
        int min = (int)t / 60;
        int sec = (int)t % 60;

        if (min != m_displayMinutes || sec != m_displaySeconds)
        {
            m_displayMinutes = min;
            m_displaySeconds = sec;
            RenderHUD();
        }
    }

    private void HandleWaveStarted(int waveIndex, int totalWaves, WaveDefinition wave)
    {
        m_currentWave = waveIndex;
        if (totalWaves > 0) m_totalWaves = totalWaves;
        RenderHUD();
    }

    private void HandleEnemyCountChanged(int liveCount)
    {
        bool changed = false;
        if (m_liveEnemies != liveCount) { m_liveEnemies = liveCount; changed = true; }
        int kills = GameStats.KillCount;
        if (m_kills != kills) { m_kills = kills; changed = true; }
        if (changed) RenderHUD();
    }

    private void RenderHUD()
    {
        if (m_text == null) return;

        string waveText = m_isEndless
            ? $"{m_currentWave}"
            : (m_totalWaves > 0 ? $"{m_currentWave} / {m_totalWaves}" : $"{m_currentWave}");

        string lastLine = m_isEndless
            ? $"KILLED  {m_kills}"
            : $"ENEMIES  {m_liveEnemies}";

        m_text.text =
            $"MODE  {m_modeLabel}\n" +
            $"TIME  {m_displayMinutes:D2}:{m_displaySeconds:D2}\n" +
            $"WAVE  {waveText}\n" +
            lastLine;
    }
}
