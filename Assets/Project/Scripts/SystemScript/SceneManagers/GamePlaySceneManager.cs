using UnityEngine;
using UnityEngine.Playables;

[RequireComponent(typeof(PlayableDirector))]
public class GamePlaySceneManager : BaseSceneManager
{
    //
    [SerializeField] private AudioClip m_ambientAudio;
    [SerializeField] private AudioClip m_introAudio;
    [SerializeField] private StatLog m_statLog;
    [SerializeField] private float m_surviveDuration = 300f;

    private float m_elapsed;
    private bool m_winTriggered;
    private bool m_isEndless;

    public float Elapsed => m_elapsed;
    public float SurviveDuration => m_surviveDuration;
    public bool IsEndless => m_isEndless;
    private SettingPanel m_settingPanel;

    protected override void Start()
    {
        base.Start();

        GameStats.Reset();
        EnemyTracker.Reset();

        // Mode & Level aware: Level wins at its Duration; Mode wins at its Duration; Endless never wins on time.
        LevelDefinition level = RunConfig.Current != null ? RunConfig.Current.Level : null;
        GameModeDefinition mode = RunConfig.Current != null ? RunConfig.Current.Mode : null;

        if (level != null)
        {
            m_isEndless = false;
            m_surviveDuration = level.Duration;
        }
        else if (mode != null)
        {
            m_isEndless = mode.ModeType == GameModeType.Endless || mode.Duration <= 0f;
            if (mode.Duration > 0f) m_surviveDuration = mode.Duration;
        }

        m_settingPanel = SingletonUIManager.Instance.SettingPanel;

        AmbientAudioManager.Instance.PlayAmbientAudio(m_ambientAudio);
        EventAudioManager.Instance.PlayEventSFX(m_introAudio);

        CursorController.CanPlaySFX = false;
        m_settingPanel.SetSceneLayout(null, false, true);

        m_statLog.gameObject.SetActive(m_settingPanel.StatLogToggle.IsActive);
        m_settingPanel.StatLogToggle.OnToggleOn += () => m_statLog.gameObject.SetActive(true);
        m_settingPanel.StatLogToggle.OnToggleOff += () => m_statLog.gameObject.SetActive(false);

        GameEvents.TriggerTimerUpdated(0f, m_surviveDuration, m_surviveDuration);
    }

    private int m_lastReportedSecond = -1;

    private void Update()
    {
        if (m_winTriggered) return;

        m_elapsed += Time.deltaTime;
        GameStats.SetElapsed(m_elapsed);

        int currentSec = (int)m_elapsed;
        if (currentSec != m_lastReportedSecond)
        {
            m_lastReportedSecond = currentSec;
            float remaining = Mathf.Max(0f, m_surviveDuration - m_elapsed);
            GameEvents.TriggerTimerUpdated(m_elapsed, remaining, m_surviveDuration);
        }

        if (!m_isEndless && m_elapsed >= m_surviveDuration)
        {
            m_winTriggered = true;
            if (RunConfig.Current != null && RunConfig.Current.Level != null)
                LevelProgress.Unlock(RunConfig.Current.Level.LevelNumber + 1);
            GameEvents.TriggerLevelCompleted();
        }
    }

    private void OnDisable()
    {
        if (m_settingPanel != null)
            m_settingPanel.StatLogToggle.ResetAction();
    }
}
