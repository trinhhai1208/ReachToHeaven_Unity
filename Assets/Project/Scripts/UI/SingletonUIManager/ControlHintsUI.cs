using DG.Tweening;
using TMPro;
using UnityEngine;

public class ControlHintsUI : MonoBehaviour
{
    [SerializeField] private float m_displayDuration = 5f;
    private TextMeshProUGUI m_text;

    private void Start()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        DOTween.To(() => m_text.alpha, v => m_text.alpha = v, 0f, 0.8f)
            .SetDelay(m_displayDuration);
    }
}
