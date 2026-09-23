using DG.Tweening;
using TMPro;
using UnityEngine;

/// <summary>
/// Displays animated Wave banner on screen when a new wave begins.
/// Listens to GameEvents.OnWaveStarted without any direct spawner coupling.
/// </summary>
[RequireComponent(typeof(TextMeshProUGUI))]
public class WaveAnnouncer : MonoBehaviour
{
    private TextMeshProUGUI m_text;
    private Sequence m_sequence;

    private void Awake()
    {
        m_text = GetComponent<TextMeshProUGUI>();
        if (m_text != null) m_text.alpha = 0f;
    }

    private void OnEnable()
    {
        GameEvents.OnWaveStarted += HandleWaveStarted;
    }

    private void OnDisable()
    {
        GameEvents.OnWaveStarted -= HandleWaveStarted;
        m_sequence?.Kill();
    }

    private void HandleWaveStarted(int waveIndex, int totalWaves, WaveDefinition wave)
    {
        ShowWave(waveIndex);
    }

    private void ShowWave(int wave)
    {
        if (m_text == null) return;

        m_text.text = $"WAVE  {wave}";
        m_sequence?.Kill();
        m_text.alpha = 0f;
        m_sequence = DOTween.Sequence()
            .Append(DOTween.To(() => m_text.alpha, v => m_text.alpha = v, 1f, 0.3f))
            .AppendInterval(1.5f)
            .Append(DOTween.To(() => m_text.alpha, v => m_text.alpha = v, 0f, 0.5f));
    }
}
