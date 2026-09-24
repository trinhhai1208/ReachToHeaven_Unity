using TMPro;
using UnityEngine;

using UnityEngine.UI;

public class GameOverPanel : MonoBehaviour
{
    private TextMeshProUGUI m_statsText;

    private TextMeshProUGUI AddStatsText()
    {
        var go = new GameObject("Stats");
        go.transform.SetParent(transform, false);
        var rt = go.AddComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.pivot = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = new Vector2(0, 60);
        rt.sizeDelta = new Vector2(280, 40);
        var tmp = go.AddComponent<TextMeshProUGUI>();
        tmp.fontSize = 16;
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.color = Color.white;
        return tmp;
    }

    public void Active()
    {
        GameManager.FreezeScreen();

        foreach (var btn in GetComponentsInChildren<Button>(true))
        {
            btn.interactable = true;
            btn.onClick.RemoveListener(OnButtonClicked);
            btn.onClick.AddListener(OnButtonClicked);
        }

        // Safety guard: ensure title text is never victory text
        foreach (var tmp in GetComponentsInChildren<TextMeshProUGUI>(true))
        {
            if (tmp != m_statsText && tmp.text != null && tmp.text.Contains("NAILED IT"))
            {
                tmp.text = "GAME OVER";
            }
        }

        if (m_statsText == null) m_statsText = AddStatsText();

        int min = (int)GameStats.ElapsedTime / 60;
        int sec = (int)GameStats.ElapsedTime % 60;
        m_statsText.text = $"SURVIVED  {min:D2}:{sec:D2}   KILLED  {GameStats.KillCount}";

        gameObject.SetActive(true);
    }

    private void OnButtonClicked()
    {
        foreach (var btn in GetComponentsInChildren<Button>(true))
            btn.interactable = false;
    }

    public void Deactive()
    {
        GameManager.UnFreezeScreen();
        gameObject.SetActive(false);
    }
}
