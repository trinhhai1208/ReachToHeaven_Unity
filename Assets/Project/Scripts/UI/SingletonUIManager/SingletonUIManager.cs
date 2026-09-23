using UnityEngine;

public class SingletonUIManager : Singleton<SingletonUIManager>
{
    //
    [SerializeField] private SettingPanel m_settingPanel;
    [SerializeField] private CursorController m_cursorController;
    [SerializeField] private GameOverPanel m_gameOverPanel;
    [SerializeField] private WinPanel m_winPanel;

    public SettingPanel SettingPanel { get => m_settingPanel; }
    public CursorController CursorController { get => m_cursorController; }
    public GameOverPanel GameOverPanel { get => m_gameOverPanel; }
    public WinPanel WinPanel { get => m_winPanel; }

    protected override void Awake()
    {
        m_isPersisted = true;
        base.Awake();
    }

    private void OnEnable()
    {
        GameEvents.OnPlayerDied += HandlePlayerDied;
        GameEvents.OnLevelCompleted += HandleLevelCompleted;
    }

    private void OnDisable()
    {
        GameEvents.OnPlayerDied -= HandlePlayerDied;
        GameEvents.OnLevelCompleted -= HandleLevelCompleted;
    }

    private void HandlePlayerDied()
    {
        if (m_gameOverPanel != null) m_gameOverPanel.Active();
    }

    private void HandleLevelCompleted()
    {
        if (m_winPanel != null) m_winPanel.Active();
    }
}
