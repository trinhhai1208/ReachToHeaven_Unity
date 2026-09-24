using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

///<summary>
///Owns run progression: reads the GameModeDefinition, runs the wave timeline and
///raises OnWaveStarted when each wave becomes active. It decides WHAT/WHEN to spawn;
///a spawner (executor) listens and performs the actual spawning. This keeps
///progression out of PortalSpawner/EnemySpawner.
/// </summary>
public class WaveDirector : MonoBehaviour
{
    //
    [Serializable] public class WaveEvent : UnityEvent<WaveDefinition> { }

    [Header("Config (falls back to RunConfig.Current.Mode when empty)")]
    [SerializeField] private GameModeDefinition m_modeOverride;
    [SerializeField] private LevelCatalog m_levelCatalog;

    [Header("Executor — the spawner that performs each wave")]
    [SerializeField] private PortalSpawner m_portalSpawner;

    [Header("Integration seam — extra listeners (HUD, boss, audio...)")]
    public WaveEvent OnWaveStarted = new WaveEvent();

    public event Action OnAllWavesDispatched;

    private struct ScheduledWave
    {
        public WaveDefinition Wave;
        public float StartTime;
    }

    private GameModeDefinition m_mode;
    private LevelDefinition m_level;
    private readonly List<ScheduledWave> m_scheduledWaves = new List<ScheduledWave>();
    private float m_elapsed;
    private int m_nextWaveIndex;
    private bool m_running;

    private bool m_loop;
    private float m_cycleStart;
    private int m_cycle;
    private bool m_exhausted;

    public float Elapsed => m_elapsed;
    public GameModeDefinition Mode => m_mode;
    public LevelDefinition Level => m_level;
    public int CurrentCycle => m_cycle;

    private GameModeDefinition ResolveMode()
    {
        if (RunConfig.Current != null && RunConfig.Current.Mode != null)
            return RunConfig.Current.Mode;
        return m_modeOverride;
    }

    private LevelDefinition ResolveLevel()
    {
        if (RunConfig.Current != null && RunConfig.Current.Level != null)
            return RunConfig.Current.Level;
        if (m_levelCatalog != null && m_levelCatalog.Count > 0)
            return m_levelCatalog.GetLevel(0);
        return null;
    }

    private void Start()
    {
        m_level = ResolveLevel();
        m_mode = ResolveMode();

        m_scheduledWaves.Clear();

        if (m_level != null && m_level.Waves != null && m_level.Waves.Count > 0)
        {
            float interval = m_level.WaveInterval > 0f ? m_level.WaveInterval : 20f;
            float initialDelay = 2f;
            for (int i = 0; i < m_level.Waves.Count; ++i)
            {
                WaveDefinition wave = m_level.Waves[i];
                if (wave == null) continue;

                float start = initialDelay + (i * interval);
                m_scheduledWaves.Add(new ScheduledWave { Wave = wave, StartTime = start });
            }
        }
        else if (m_mode != null)
        {
            foreach (WaveDefinition wave in m_mode.Waves)
            {
                if (wave != null)
                    m_scheduledWaves.Add(new ScheduledWave { Wave = wave, StartTime = wave.StartTime });
            }
        }
        else
        {
            Debug.LogError($"{this}: No LevelDefinition or GameModeDefinition assigned.");
            return;
        }

        m_scheduledWaves.Sort((a, b) => a.StartTime.CompareTo(b.StartTime));

        m_elapsed = 0f;
        m_nextWaveIndex = 0;
        m_cycleStart = 0f;
        m_cycle = 0;
        m_exhausted = false;

        float runDuration = m_level != null ? m_level.Duration : (m_mode != null ? m_mode.Duration : 0f);
        m_loop = (m_mode != null && m_mode.ModeType == GameModeType.Endless) || runDuration <= 0f;
        m_running = true;
    }

    private void Update()
    {
        if (!m_running || m_scheduledWaves.Count == 0) return;

        m_elapsed += Time.deltaTime;
        float local = m_elapsed - m_cycleStart;

        while (m_nextWaveIndex < m_scheduledWaves.Count
               && local >= m_scheduledWaves[m_nextWaveIndex].StartTime)
        {
            int waveIndex = m_nextWaveIndex + 1 + (m_cycle * m_scheduledWaves.Count);
            int totalWaves = m_level != null ? m_level.TotalWaves : m_scheduledWaves.Count;
            DispatchWave(m_scheduledWaves[m_nextWaveIndex].Wave, waveIndex, totalWaves);
            ++m_nextWaveIndex;
        }

        if (m_nextWaveIndex < m_scheduledWaves.Count) return;

        if (m_loop)
        {
            ScheduledWave last = m_scheduledWaves[m_scheduledWaves.Count - 1];
            float scheduleEnd = last.StartTime + Mathf.Max(1f, last.Wave.Duration);
            if (local >= scheduleEnd)
            {
                m_cycleStart = m_elapsed;
                m_nextWaveIndex = 0;
                ++m_cycle;
            }
        }
        else if (!m_exhausted)
        {
            m_exhausted = true;
            OnAllWavesDispatched?.Invoke();
        }
    }

    private void DispatchWave(WaveDefinition wave, int waveNumber, int totalWaves)
    {
        if (m_portalSpawner != null) m_portalSpawner.ExecuteWave(wave);
        else Debug.LogWarning($"{this}: No PortalSpawner assigned; wave '{wave?.name}' dispatched to listeners only.");

        OnWaveStarted?.Invoke(wave);
        GameEvents.TriggerWaveStarted(waveNumber, totalWaves, wave);
    }

    ///<summary>
    ///True while the wave is still allowed to push more enemies (alive-cap gate).
    ///A spawner calls this before each spawn.
    /// </summary>
    public bool CanSpawnMore(WaveDefinition wave)
        => wave != null && EnemyTracker.LiveCount < wave.AliveCap;

    public void StopDirecting() => m_running = false;
}
