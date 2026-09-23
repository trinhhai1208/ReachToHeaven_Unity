using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/// <summary>
/// Drives the map and level selection screen.
/// Supports both data-driven LevelCatalog (grid layout, 10 levels per page, pagination arrows,
/// border image + level number) and legacy GameContentCatalog mode list.
/// </summary>
public class MapModeSelectController : MonoBehaviour
{
    private const int LevelsPerPage = 10;

    [Header("Level Data (Preferred)")]
    [SerializeField] private LevelCatalog m_levelCatalog;
    [SerializeField] private Sprite m_borderSprite;
    [SerializeField] private Button m_prevPageButton;
    [SerializeField] private Button m_nextPageButton;
    [SerializeField] private TMP_Text m_pageText;
    [SerializeField] private Button m_backButton;

    [Header("Legacy Mode Data (Fallback)")]
    [SerializeField] private GameContentCatalog m_catalog;

    [Header("Container & Template")]
    [SerializeField] private Transform m_modeContainer;
    [SerializeField] private Button m_modeButtonTemplate;

    [Header("Misc UI")]
    [SerializeField] private TMP_Text m_descriptionText;
    [SerializeField] private Button m_startButton;

    [Header("Highlight colors")]
    [SerializeField] private Color m_selectedColor = new Color(0.95f, 0.75f, 0.2f, 1f);
    [SerializeField] private Color m_normalColor = new Color(0.22f, 0.22f, 0.26f, 1f);

    private readonly List<Button> m_spawnedButtons = new List<Button>();
    private int m_currentPage;
    private int m_selectedLevelIndex;
    private int m_selectedMode;

    private int TotalLevelPages => m_levelCatalog != null && m_levelCatalog.Count > 0
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
        // Try auto-loading LevelCatalog from Resources/Data if not wired in Inspector
        if (m_levelCatalog == null)
        {
            m_levelCatalog = Resources.Load<LevelCatalog>("Framework/LevelCatalog");
            if (m_levelCatalog == null)
            {
                // Try asset search via Resources or keep catalog
            }
        }

        if (m_levelCatalog != null && m_levelCatalog.Count > 0)
        {
            SetupLevelGridView();
            m_currentPage = 0;
            RenderLevelPage(0);
            SelectLevel(0);
        }
        else if (m_catalog != null && m_catalog.Modes.Count > 0)
        {
            BuildModeButtons();
            SelectMode(0);
        }
        else
        {
            Debug.LogError($"{this}: Neither LevelCatalog nor GameContentCatalog is assigned.");
        }
    }

    private void Update()
    {
        // Keyboard navigation shortcuts
        if (m_levelCatalog != null && m_levelCatalog.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow) || Input.GetKeyDown(KeyCode.A)) OnPrevPage();
            else if (Input.GetKeyDown(KeyCode.RightArrow) || Input.GetKeyDown(KeyCode.D)) OnNextPage();
        }
    }

    #region Level Select Grid & Pagination
    private void SetupLevelGridView()
    {
        if (m_modeContainer == null || m_modeButtonTemplate == null) return;

        // Ensure GridLayoutGroup is on container for 10 levels per page (e.g. 5 columns x 2 rows)
        GridLayoutGroup grid = m_modeContainer.GetComponent<GridLayoutGroup>();
        if (grid == null)
        {
            VerticalLayoutGroup vertical = m_modeContainer.GetComponent<VerticalLayoutGroup>();
            if (vertical != null) Destroy(vertical);

            grid = m_modeContainer.gameObject.AddComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(90f, 90f);
            grid.spacing = new Vector2(16f, 16f);
            grid.constraint = GridLayoutGroup.Constraint.FixedColumnCount;
            grid.constraintCount = 5;
            grid.childAlignment = TextAnchor.MiddleCenter;
        }

        // Adjust rect transform size if needed to fit 5x2 grid
        RectTransform rect = m_modeContainer.GetComponent<RectTransform>();
        if (rect != null)
        {
            rect.sizeDelta = new Vector2(560f, 220f);
        }

        m_modeButtonTemplate.gameObject.SetActive(false);

        // Pre-instantiate 10 slot buttons
        while (m_spawnedButtons.Count < LevelsPerPage)
        {
            Button btn = Instantiate(m_modeButtonTemplate, m_modeContainer);
            btn.gameObject.SetActive(false);
            btn.gameObject.name = $"LevelSlot_{m_spawnedButtons.Count}";

            Image img = btn.GetComponent<Image>();
            if (img != null)
            {
                if (m_borderSprite != null)
                {
                    img.sprite = m_borderSprite;
                    img.type = Image.Type.Sliced;
                }
            }

            TMP_Text txt = btn.GetComponentInChildren<TMP_Text>(true);
            if (txt != null)
            {
                txt.fontSize = 32f;
                txt.alignment = TextAlignmentOptions.Center;
            }

            int slot = m_spawnedButtons.Count;
            btn.onClick.AddListener(() => OnLevelSlotClicked(slot));
            m_spawnedButtons.Add(btn);
        }

        // Auto-create pagination controls if unassigned
        EnsurePaginationUI();
    }

    private void EnsurePaginationUI()
    {
        if (m_pageText == null && m_modeContainer != null)
        {
            var pageObj = new GameObject("PageText");
            pageObj.transform.SetParent(m_modeContainer.parent, false);
            var rt = pageObj.AddComponent<RectTransform>();
            rt.anchorMin = new Vector2(0.5f, 0.5f);
            rt.anchorMax = new Vector2(0.5f, 0.5f);
            rt.anchoredPosition = new Vector2(0, 80);
            rt.sizeDelta = new Vector2(300, 40);
            m_pageText = pageObj.AddComponent<TextMeshProUGUI>();
            m_pageText.fontSize = 20;
            m_pageText.alignment = TextAlignmentOptions.Center;
            m_pageText.color = Color.white;
        }
    }

    private void RenderLevelPage(int page)
    {
        m_currentPage = Mathf.Clamp(page, 0, Mathf.Max(0, TotalLevelPages - 1));
        int startIndex = m_currentPage * LevelsPerPage;

        for (int i = 0; i < LevelsPerPage; ++i)
        {
            int levelIdx = startIndex + i;
            if (i < m_spawnedButtons.Count)
            {
                Button btn = m_spawnedButtons[i];
                if (m_levelCatalog != null && levelIdx < m_levelCatalog.Count)
                {
                    LevelDefinition level = m_levelCatalog.GetLevel(levelIdx);
                    btn.gameObject.SetActive(true);

                    TMP_Text txt = btn.GetComponentInChildren<TMP_Text>(true);
                    if (txt != null && level != null)
                    {
                        txt.text = level.LevelNumber.ToString();
                    }

                    Image img = btn.GetComponent<Image>();
                    if (img != null)
                    {
                        img.color = (levelIdx == m_selectedLevelIndex) ? m_selectedColor : m_normalColor;
                    }
                }
                else
                {
                    btn.gameObject.SetActive(false);
                }
            }
        }

        if (m_pageText != null)
        {
            m_pageText.text = $"Trang {m_currentPage + 1} / {TotalLevelPages}";
        }

        if (m_prevPageButton != null) m_prevPageButton.interactable = m_currentPage > 0;
        if (m_nextPageButton != null) m_nextPageButton.interactable = m_currentPage < TotalLevelPages - 1;
    }

    private void OnLevelSlotClicked(int slotIndex)
    {
        int levelIdx = (m_currentPage * LevelsPerPage) + slotIndex;
        if (m_levelCatalog != null && levelIdx < m_levelCatalog.Count)
        {
            SelectLevel(levelIdx);
        }
    }

    private void SelectLevel(int index)
    {
        m_selectedLevelIndex = index;

        // Highlight slot
        int startIndex = m_currentPage * LevelsPerPage;
        for (int i = 0; i < m_spawnedButtons.Count; ++i)
        {
            int levelIdx = startIndex + i;
            Image img = m_spawnedButtons[i].GetComponent<Image>();
            if (img != null)
            {
                img.color = (levelIdx == m_selectedLevelIndex) ? m_selectedColor : m_normalColor;
            }
        }

        if (m_descriptionText != null && m_levelCatalog != null && index < m_levelCatalog.Count)
        {
            LevelDefinition level = m_levelCatalog.GetLevel(index);
            m_descriptionText.text = DescribeLevel(level);
        }
    }

    public void OnPrevPage()
    {
        if (m_currentPage > 0)
        {
            RenderLevelPage(m_currentPage - 1);
        }
    }

    public void OnNextPage()
    {
        if (m_currentPage < TotalLevelPages - 1)
        {
            RenderLevelPage(m_currentPage + 1);
        }
    }

    private static string DescribeLevel(LevelDefinition level)
    {
        if (level == null) return string.Empty;
        int min = Mathf.FloorToInt(level.Duration / 60f);
        int sec = Mathf.FloorToInt(level.Duration % 60f);
        return $"<b>{level.DisplayName.ToUpper()}</b>\n" +
               $"Thời gian sinh tồn: {min:D2}:{sec:D2} | Tổng số wave: {level.TotalWaves}\n" +
               $"Khoảng cách wave: {level.WaveInterval:0}s\n\n" +
               $"{level.Description}";
    }
    #endregion

    #region Legacy Mode List Fallback
    private void BuildModeButtons()
    {
        m_spawnedButtons.Clear();
        if (m_modeContainer == null || m_modeButtonTemplate == null) return;

        m_modeButtonTemplate.gameObject.SetActive(false);
        for (int i = 0; i < m_catalog.Modes.Count; ++i)
        {
            int index = i;
            GameModeDefinition mode = m_catalog.Modes[i];

            Button button = Instantiate(m_modeButtonTemplate, m_modeContainer);
            button.gameObject.SetActive(true);
            button.gameObject.name = $"{m_modeButtonTemplate.name}_{i}";

            TMP_Text text = button.GetComponentInChildren<TMP_Text>(true);
            if (text != null) text.text = Label(mode);

            button.onClick.AddListener(() => SelectMode(index));
            m_spawnedButtons.Add(button);
        }
    }

    private void SelectMode(int index)
    {
        m_selectedMode = index;

        for (int i = 0; i < m_spawnedButtons.Count; ++i)
        {
            Image image = m_spawnedButtons[i].GetComponent<Image>();
            if (image != null) image.color = i == index ? m_selectedColor : m_normalColor;
        }

        if (m_descriptionText != null && m_catalog.Modes.Count > 0)
        {
            GameModeDefinition mode = m_catalog.Modes[Mathf.Clamp(index, 0, m_catalog.Modes.Count - 1)];
            m_descriptionText.text = DescribeMode(mode);
        }
    }

    private static string Label(GameModeDefinition mode)
    {
        if (mode == null) return "<null>";
        return string.IsNullOrEmpty(mode.DisplayName) ? mode.name : mode.DisplayName;
    }

    private static string DescribeMode(GameModeDefinition mode)
    {
        if (mode == null) return string.Empty;
        if (mode.ModeType == GameModeType.Endless || mode.Duration <= 0f)
            return $"{Label(mode)} - survive endlessly, difficulty rises each cycle.";

        int minutes = Mathf.FloorToInt(mode.Duration / 60f);
        int seconds = Mathf.FloorToInt(mode.Duration % 60f);
        return $"{Label(mode)} - survive {minutes:0}:{seconds:00}, then win.";
    }
    #endregion

    /// <summary>
    /// Confirm the selection: store it in RunConfig and load GamePlay via LoadingScene.
    /// </summary>
    public void StartRun()
    {
        MapDefinition map = null;
        LevelDefinition level = null;
        GameModeDefinition mode = null;

        if (m_levelCatalog != null && m_levelCatalog.Count > 0)
        {
            level = m_levelCatalog.GetLevel(m_selectedLevelIndex);
            map = level != null ? level.Map : null;
        }
        else if (m_catalog != null && m_catalog.Modes.Count > 0)
        {
            mode = m_catalog.Modes[Mathf.Clamp(m_selectedMode, 0, m_catalog.Modes.Count - 1)];
            map = m_catalog.Maps.Count > 0 ? m_catalog.Maps[0] : null;
        }

        RunConfig.Set(new RunConfig
        {
            Map = map,
            Mode = mode,
            Level = level,
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
