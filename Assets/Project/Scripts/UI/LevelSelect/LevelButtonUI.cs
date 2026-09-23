using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Individual level button inside the Level Select grid.
/// Displays a decorative border image, the level number, and a locked state.
/// </summary>
[RequireComponent(typeof(Button))]
public class LevelButtonUI : MonoBehaviour
{
    [Header("UI Elements")]
    [SerializeField] private Image m_borderImage;
    [SerializeField] private TextMeshProUGUI m_levelNumberText;
    [SerializeField] private Button m_button;
    [Tooltip("Optional object shown only when the level is locked (e.g. a padlock icon).")]
    [SerializeField] private GameObject m_lockOverlay;

    [Header("Selection Colors")]
    [SerializeField] private Color m_normalColor = new Color(0.16f, 0.17f, 0.24f, 0.92f);
    [SerializeField] private Color m_selectedColor = new Color(0.95f, 0.72f, 0.18f, 1f);
    [SerializeField] private Color m_lockedColor = new Color(0.10f, 0.10f, 0.12f, 0.85f);

    private LevelDefinition m_level;
    private Action<LevelDefinition> m_onClicked;
    private bool m_locked;

    private void Awake()
    {
        if (m_button == null) m_button = GetComponent<Button>();
        if (m_borderImage == null) m_borderImage = GetComponent<Image>();
        if (m_levelNumberText == null) m_levelNumberText = GetComponentInChildren<TextMeshProUGUI>(true);

        if (m_button != null)
        {
            m_button.onClick.AddListener(OnClick);
        }
    }

    public void Init(LevelDefinition level, Action<LevelDefinition> onClicked)
    {
        m_level = level;
        m_onClicked = onClicked;

        if (m_levelNumberText != null && level != null)
        {
            m_levelNumberText.text = level.LevelNumber.ToString();
        }
    }

    /// <summary>Locks/unlocks the button: locked levels are dimmed and non-interactable.</summary>
    public void SetLocked(bool locked)
    {
        m_locked = locked;
        if (m_button != null) m_button.interactable = !locked;
        if (m_lockOverlay != null) m_lockOverlay.SetActive(locked);
        if (m_levelNumberText != null) m_levelNumberText.alpha = locked ? 0.30f : 1f;
        if (m_borderImage != null && locked) m_borderImage.color = m_lockedColor;
    }

    public void SetSelected(bool selected)
    {
        if (m_locked)
        {
            if (m_borderImage != null) m_borderImage.color = m_lockedColor;
            return;
        }
        if (m_borderImage != null)
        {
            m_borderImage.color = selected ? m_selectedColor : m_normalColor;
        }
    }

    private void OnClick()
    {
        if (m_locked || m_level == null) return;
        m_onClicked?.Invoke(m_level);
    }
}
