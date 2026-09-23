using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Controller for the Level/Map Selection screen.
/// Dynamically renders levels from LevelCatalog into a 10-item GridLayoutGroup with pagination arrows.
/// </summary>
public class LevelSelectUI : MonoBehaviour
{
    private const int LevelsPerPage = 10;

    [Header("Data Source")]
    [SerializeField] private LevelCatalog m_levelCatalog;

    [Header("Grid & Template")]
    [SerializeField] private Transform m_gridContainer;
    [SerializeField] private LevelButtonUI m_levelButtonTemplate;

    [Header("Pagination Controls")]
    [SerializeField] private Button m_prevPageButton;
    [SerializeField] private Button m_nextPageButton;
    [SerializeField] private TextMeshProUGUI m_pageText;

    [Header("Level Info Preview")]
    [SerializeField] private TextMeshProUGUI m_levelTitleText;
    [SerializeField] private TextMeshProUGUI m_levelDetailsText;
    [SerializeField] private Button m_startButton;
    [SerializeField] private Button m_backButton;

    private readonly List<LevelButtonUI> m_spawnedButtons = new List<LevelButtonUI>();
    private int m_currentPage;
    private LevelDefinition m_selectedLevel;

    private int TotalPages => m_levelCatalog != null && m_levelCatalog.Count > 0
        ? Mathf.CeilToInt((float)m_levelCatalog.Count / LevelsPerPage)
        : 1;

    private void Awake()
    {
        if (m_prevPageButton != null) m_prevPageButton.onClick.AddListener(OnPrevPage);
        if (m_nextPageButton != null) m_nextPageButton.onClick.AddListener(OnNextPage);
        if (m_startButton != null) m_startButton.onClick.AddListener(StartRun);
        if (m_backButton != null) m_backButton.onClick.AddListener(BackToMainMenu);
    }

    private void Start()
    {
        if (m_levelCatalog == null || m_levelCatalog.Count == 0)
        {
            Debug.LogWarning($"{this}: LevelCatalog is empty or unassigned.");
            return;
        }

        PrepareGridButtons();
        m_currentPage = 0;
        RenderPage(m_currentPage);

        if (m_levelCatalog.Count > 0)
        {
            SelectLevel(m_levelCatalog.GetLevel(0));
        }
    }

    private void PrepareGridButtons()
    {
        if (m_levelButtonTemplate == null || m_gridContainer == null) return;

        m_levelButtonTemplate.gameObject.SetActive(false);

        // Pre-instantiate 10 slot buttons for the grid page
        while (m_spawnedButtons.Count < LevelsPerPage)
        {
            LevelButtonUI button = Instantiate(m_levelButtonTemplate, m_gridContainer);
            button.gameObject.SetActive(false);
            m_spawnedButtons.Add(button);
        }
    }

    public void RenderPage(int pageIndex)
    {
        m_currentPage = Mathf.Clamp(pageIndex, 0, Mathf.Max(0, TotalPages - 1));
        int startIndex = m_currentPage * LevelsPerPage;

        for (int i = 0; i < LevelsPerPage; ++i)
        {
            int levelIndex = startIndex + i;
            if (i < m_spawnedButtons.Count)
            {
                LevelButtonUI btn = m_spawnedButtons[i];
                if (m_levelCatalog != null && levelIndex < m_levelCatalog.Count)
                {
                    LevelDefinition level = m_levelCatalog.GetLevel(levelIndex);
                    btn.gameObject.SetActive(true);
                    btn.Init(level, SelectLevel);
                    btn.SetLocked(!LevelProgress.IsUnlocked(level.LevelNumber));
                    btn.SetSelected(level == m_selectedLevel);
                }
                else
                {
                    btn.gameObject.SetActive(false);
                }
            }
        }

        UpdatePaginationUI();
    }

    private void UpdatePaginationUI()
    {
        if (m_pageText != null)
        {
            m_pageText.text = $"Page {m_currentPage + 1} / {TotalPages}";
        }

        if (m_prevPageButton != null)
        {
            m_prevPageButton.interactable = m_currentPage > 0;
        }

        if (m_nextPageButton != null)
        {
            m_nextPageButton.interactable = m_currentPage < TotalPages - 1;
        }
    }

    public void OnPrevPage()
    {
        if (m_currentPage > 0)
        {
            RenderPage(m_currentPage - 1);
        }
    }

    public void OnNextPage()
    {
        if (m_currentPage < TotalPages - 1)
        {
            RenderPage(m_currentPage + 1);
        }
    }

    public void SelectLevel(LevelDefinition level)
    {
        if (level != null && !LevelProgress.IsUnlocked(level.LevelNumber)) return;
        m_selectedLevel = level;

        // Update button visual states
        for (int i = 0; i < m_spawnedButtons.Count; ++i)
        {
            if (m_spawnedButtons[i].gameObject.activeSelf)
            {
                int levelIndex = (m_currentPage * LevelsPerPage) + i;
                bool isSelected = m_levelCatalog != null
                    && levelIndex < m_levelCatalog.Count
                    && m_levelCatalog.GetLevel(levelIndex) == m_selectedLevel;
                m_spawnedButtons[i].SetSelected(isSelected);
            }
        }

        // Update preview text
        if (m_selectedLevel != null)
        {
            if (m_levelTitleText != null)
            {
                m_levelTitleText.text = $"{m_selectedLevel.DisplayName} (Lv {m_selectedLevel.LevelNumber})";
            }

            if (m_levelDetailsText != null)
            {
                int min = Mathf.FloorToInt(m_selectedLevel.Duration / 60f);
                int sec = Mathf.FloorToInt(m_selectedLevel.Duration % 60f);
                m_levelDetailsText.text =
                    $"Survival Time: {min:D2}:{sec:D2}\n" +
                    $"Total Waves: {m_selectedLevel.TotalWaves}\n" +
                    $"Wave Interval: {m_selectedLevel.WaveInterval:0}s\n" +
                    $"{m_selectedLevel.Description}";
            }
        }
    }

    public void StartRun()
    {
        if (m_selectedLevel == null)
        {
            Debug.LogWarning($"{this}: No level selected to start.");
            return;
        }

        RunConfig.Set(new RunConfig
        {
            Map = m_selectedLevel.Map,
            Mode = null,
            Level = m_selectedLevel,
            DifficultyMultiplier = 1f,
            Seed = Random.Range(int.MinValue, int.MaxValue)
        });

        LoadingSceneData.NextSceneToLoad = SceneLibrary.GamePlay;
        SceneManager.LoadScene((int)SceneLibrary.LoadingScene, LoadSceneMode.Additive);
    }

    public void BackToMainMenu()
    {
        SceneManager.LoadScene((int)SceneLibrary.MainMenu);
    }
}
